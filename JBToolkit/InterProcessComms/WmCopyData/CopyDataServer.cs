using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

/// <summary>
/// WM_COPYDATA probably doesn’t say much to .NET developers, but for old-school Win32 C/C++ developers it certainly does! Basically, 
/// it was a way by which one could send arbitrary data, including structured data, between processes (actually, strictly speaking, windows).
/// One would send a WM_COPYDATA message to a window handle, running on any process, and Windows would take care of marshalling the data so
/// that it is available outside the address space of the sending process. It is even possible to send it to all processes, using HWND_BROADCAST,
/// but that probably wouldn’t be wise, because different applications might have different interpretations of it. Also, it needs to be passed 
/// with SendMessage, PostMessage won’t work.
/// 
/// Gist from: https://weblogs.asp.net/ricardoperes/local-machine-interprocess-communication-with-net
/// </summary>
namespace JBToolkit.InterProcessComms.WM_COPYDATA
{
#pragma warning disable CA1060 // Move pinvokes to native methods class
#pragma warning disable CA1063 // Implement IDisposable Correctly
    public class CopyDataServer : IIpcServer
#pragma warning restore CA1063 // Implement IDisposable Correctly
#pragma warning restore CA1060 // Move pinvokes to native methods class
    {
        private NativeWindow messageHandler;

        internal static string _wcName = typeof(IIpcClient).Name;

        public CopyDataServer() { }

        public CopyDataServer(string wcName)
        {
            _wcName = wcName;
        }

        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int uMsg, IntPtr wparam, IntPtr lparam);

        private const int WM_COPY_DATA = 0x004A;
        private const int WM_QUIT = 0x0012;

        private sealed class MessageHandler : NativeWindow
        {
            private readonly CopyDataServer server;

            public MessageHandler(CopyDataServer server)
            {
                this.server = server;
                this.CreateHandle(new CreateParams() { Caption = _wcName });
            }

            protected override void WndProc(ref Message msg)
            {
                if (msg.Msg == WM_COPY_DATA)
                {
                    var cds = (COPYDATASTRUCT)Marshal.PtrToStructure(msg.LParam, typeof(COPYDATASTRUCT));

                    if (cds.cbData.ToInt32() > 0)
                    {
                        var bytes = new byte[cds.cbData.ToInt32()];

                        Marshal.Copy(cds.lpData, bytes, 0, cds.cbData.ToInt32());

                        var chars = Encoding.ASCII.GetChars(bytes);
                        var data = new string(chars);

                        this.server.OnReceived(new DataReceivedEventArgs(data));
                    }

                    msg.Result = (IntPtr)1;
                }

                base.WndProc(ref msg);
            }
        }

        private void OnReceived(DataReceivedEventArgs e)
        {
            this.Received?.Invoke(this, e);
        }

#pragma warning disable CA1033 // Interface methods should be callable by child types
#pragma warning disable CA1063 // Implement IDisposable Correctly
        void IDisposable.Dispose()
#pragma warning restore CA1063 // Implement IDisposable Correctly
#pragma warning restore CA1033 // Interface methods should be callable by child types
        {
            this.Stop();
        }

        public void Start()
        {
            Task.Factory.StartNew(() =>
            {
                this.messageHandler = new MessageHandler(this);

                Application.Run();
            });
        }

        public void Stop()
        {
            SendMessage(this.messageHandler.Handle, WM_QUIT, IntPtr.Zero, IntPtr.Zero);
        }

        public event EventHandler<DataReceivedEventArgs> Received;
    }
}
