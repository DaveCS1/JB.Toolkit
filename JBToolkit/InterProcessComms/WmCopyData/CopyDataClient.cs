using System;
using System.Runtime.InteropServices;

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
    public class CopyDataClient : IIpcClient
#pragma warning restore CA1060 // Move pinvokes to native methods class
    {
        private string _wcName = typeof(IIpcClient).Name;

        public CopyDataClient() { }

        public CopyDataClient(string wcName)
        {
            _wcName = wcName;
        }

        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int uMsg, IntPtr wparam, IntPtr lparam);

#pragma warning disable CA2101 // Specify marshaling for P/Invoke string arguments
        [DllImport("user32.dll")]
#pragma warning restore CA2101 // Specify marshaling for P/Invoke string arguments
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        private const int WM_COPY_DATA = 0x004A;

        public void Send(string data)
        {
            var cds = new COPYDATASTRUCT();
            cds.dwData = (IntPtr)Marshal.SizeOf(cds);
            cds.cbData = (IntPtr)data.Length;
            cds.lpData = Marshal.StringToHGlobalAnsi(data);

            var ptr = Marshal.AllocCoTaskMem(Marshal.SizeOf(cds));

            Marshal.StructureToPtr(cds, ptr, true);

            var target = FindWindow(null, _wcName);  //(IntPtr)HWND_BROADCAST;
            _ = SendMessage(target, WM_COPY_DATA, IntPtr.Zero, ptr);

            Marshal.FreeHGlobal(cds.lpData);
            Marshal.FreeCoTaskMem(ptr);
        }
    }
}
