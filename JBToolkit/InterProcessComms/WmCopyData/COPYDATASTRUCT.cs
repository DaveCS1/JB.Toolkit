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
    [StructLayout(LayoutKind.Sequential)]
    public struct COPYDATASTRUCT
    {
        public IntPtr dwData;
        public IntPtr cbData;
        public IntPtr lpData;
    }
}
