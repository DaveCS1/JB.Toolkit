using System;
using System.IO.MemoryMappedFiles;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// Memory-mapped files in Windows allow us to either map a “window” of a large file on the filesystem, or to create a named memory area that 
/// can be shared among processes. A shared area on the fly is created and uses named AutoResetEvents to control access to it.
/// </summary>
namespace JBToolkit.InterProcessComms.MemoryMappedFiles
{
    public sealed class SharedMemoryServer : IIpcServer
    {
        string _mapFilename = typeof(IIpcClient).Name;

        public SharedMemoryServer() { }

        public SharedMemoryServer(string mapFilename)
        {
            _mapFilename = mapFilename;
        }

        private readonly ManualResetEvent killer = new ManualResetEvent(false);
        private const int capacity = 1024;

        private void OnReceived(DataReceivedEventArgs e)
        {
            this.Received?.Invoke(this, e);
        }

        public event EventHandler<DataReceivedEventArgs> Received;

        public void Start()
        {
            Task.Factory.StartNew(() =>
            {
                var evt = null as EventWaitHandle;

                if (EventWaitHandle.TryOpenExisting(typeof(IIpcClient).Name, out evt) == false)
                {
                    evt = new EventWaitHandle(false, EventResetMode.AutoReset, _mapFilename);
                }

                using (evt)
                using (var file = MemoryMappedFile.CreateOrOpen(_mapFilename + "File", capacity))
                using (var view = file.CreateViewAccessor())
                {
                    var data = new byte[capacity];

                    while (WaitHandle.WaitAny(new WaitHandle[] { this.killer, evt }) == 1)
                    {
                        view.ReadArray(0, data, 0, data.Length);

                        this.OnReceived(new DataReceivedEventArgs(Encoding.Default.GetString(data)));
                    }
                }
            });
        }

        public void Stop()
        {
            this.killer.Set();
        }

#pragma warning disable CA1063 // Implement IDisposable Correctly
        void IDisposable.Dispose()
#pragma warning restore CA1063 // Implement IDisposable Correctly
        {
            this.Stop();
            this.killer.Dispose();
        }
    }
}
