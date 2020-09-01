using System.IO.MemoryMappedFiles;
using System.Text;
using System.Threading;

/// <summary>
/// Memory-mapped files in Windows allow us to either map a “window” of a large file on the filesystem, or to create a named memory area that 
/// can be shared among processes. A shared area on the fly is created and uses named AutoResetEvents to control access to it.
/// </summary>
namespace JBToolkit.InterProcessComms.MemoryMappedFiles
{
    public class SharedMemoryClient : IIpcClient
    {
        string _mapFilename = typeof(IIpcClient).Name;

        public SharedMemoryClient() { }

        public SharedMemoryClient(string mapFilename)
        {
            _mapFilename = mapFilename;
        }

        public void Send(string data)
        {
            if (EventWaitHandle.TryOpenExisting(typeof(IIpcClient).Name, out EventWaitHandle evt) == false)
            {
                evt = new EventWaitHandle(false, EventResetMode.AutoReset, _mapFilename);
            }

            using (evt)
            using (var file = MemoryMappedFile.CreateOrOpen(_mapFilename + "File", 1024))
            using (var view = file.CreateViewAccessor())
            {
                var bytes = Encoding.Default.GetBytes(data);

                view.WriteArray(0, bytes, 0, bytes.Length);

                evt.Set();
            }
        }
    }
}
