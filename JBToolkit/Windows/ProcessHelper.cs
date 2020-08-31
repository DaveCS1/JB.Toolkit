using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Management;
using System.Runtime.InteropServices;
using System.Text;

namespace JBToolkit.Windows
{
    /// <summary>
    /// 
    /// Helper class for manipulating mainly remote processes. Most likely you will need to wrap the call with Impersonatation. I.e:
    /// 
    /// 
    //      using (new Impersonator(ServerType.DOMAIN))
    //      {
    //          GetProcessMemoryUsage("notepad", "HMS-DAME01-D");
    //      }
    /// 
    /// 
    /// </summary>
    public class ProcessHelper
    {
        [StructLayout(LayoutKind.Sequential)]
        struct RM_UNIQUE_PROCESS
        {
            public int dwProcessId;
            public System.Runtime.InteropServices.ComTypes.FILETIME ProcessStartTime;
        }

        const int RmRebootReasonNone = 0;
        const int CCH_RM_MAX_APP_NAME = 255;
        const int CCH_RM_MAX_SVC_NAME = 63;

        enum RM_APP_TYPE
        {
            RmUnknownApp = 0,
            RmMainWindow = 1,
            RmOtherWindow = 2,
            RmService = 3,
            RmExplorer = 4,
            RmConsole = 5,
            RmCritical = 1000
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        struct RM_PROCESS_INFO
        {
            public RM_UNIQUE_PROCESS Process;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = CCH_RM_MAX_APP_NAME + 1)]
            public string strAppName;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = CCH_RM_MAX_SVC_NAME + 1)]
            public string strServiceShortName;

            public RM_APP_TYPE ApplicationType;
            public uint AppStatus;
            public uint TSSessionId;
            [MarshalAs(UnmanagedType.Bool)]
            public bool bRestartable;
        }

        [DllImport("rstrtmgr.dll", CharSet = CharSet.Unicode)]
        static extern int RmRegisterResources(uint pSessionHandle,
                                            UInt32 nFiles,
                                            string[] rgsFilenames,
                                            UInt32 nApplications,
                                            [In] RM_UNIQUE_PROCESS[] rgApplications,
                                            UInt32 nServices,
                                            string[] rgsServiceNames);

        [DllImport("rstrtmgr.dll", CharSet = CharSet.Auto)]
        static extern int RmStartSession(out uint pSessionHandle, int dwSessionFlags, string strSessionKey);

        [DllImport("rstrtmgr.dll")]
        static extern int RmEndSession(uint pSessionHandle);

        [DllImport("rstrtmgr.dll")]
        static extern int RmGetList(uint dwSessionHandle,
                                    out uint pnProcInfoNeeded,
                                    ref uint pnProcInfo,
                                    [In, Out] RM_PROCESS_INFO[] rgAffectedApps,
                                    ref uint lpdwRebootReasons);


        /// <summary>
        /// Get remote process(s) by name (can also be used to get local process if you use 'localhost'). You will likely need to use impersonatation when calling
        /// </summary>
        /// <param name="processName">Name of process to get info for</param>
        /// <param name="remoteHost">Remote host where process resides</param>
        /// <returns>Process array object</returns>
        public static Process[] GetRemoteProcess(string processName, string remoteHost)
        {
            // For some reason 'GetProcessByName' for remote host doesn't always work. So get all of them and
            // return the ones matching the name ourselves

            Process[] procs = Process.GetProcesses(remoteHost);
            List<Process> returnProcs = new List<Process>();

            foreach (var p in procs)
            {
                if (p.ProcessName.ToLower() == processName.ToLower())
                {
                    returnProcs.Add(p);
                }
            }

            return returnProcs.ToArray();
        }

        /// <summary>
        /// Get the memory usage in KB of a particular processs. You will likely need to use impersonatation when calling.
        /// </summary>
        /// <param name="process">The process object to get the memory usage of</param>
        /// <param name="remoteHost">The remote host where the process resides</param>
        /// <returns>Memory usage in KB</returns>
        public static int GetProcessMemoryUsage(string processName, string remoteHost)
        {
            PerformanceCounter PC = new PerformanceCounter("Process", "Working Set - Private", processName, remoteHost);
            int memsize = Convert.ToInt32(PC.NextValue()) / 1024;
            PC.Close();
            PC.Dispose();

            return memsize;
        }

        /// <summary>
        /// Executes a process and reads the output (std out and error out) - Useful for working with command line utilties in managed code
        /// </summary>
        /// <param name="path">Path of program to execute</param>
        /// <param name="arguments">Optionally include and command line arguments</param>
        /// <param name="workingDirectory">Working folder path</param>
        /// <param name="timeoutSeconds">End process timeout. Defaults to 30 seconds</param>
        /// <param name="throwOnError">Should the method throw an error when exception caught or just ignore</param>
        /// <returns>Return STD + Err output</returns>
        public static string ExecuteProcessAndReadStdOut(
            string path,
            out string errorOutput,
            string arguments = "",
            string workingDirectory = "",
            int timeoutSeconds = 60,
            bool throwOnError = true
            )
        {
            errorOutput = null;

            Process process = new Process();
            StringBuilder outputStringBuilder = new StringBuilder();
            StringBuilder errorStringBuilder = new StringBuilder();

            int timeoutMs = timeoutSeconds * 1000;

            try
            {
                process.StartInfo.FileName = path;
                process.StartInfo.WorkingDirectory = string.IsNullOrEmpty(workingDirectory) ? Path.GetDirectoryName(path) : workingDirectory;
                process.StartInfo.Arguments = arguments;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.UseShellExecute = false;
                process.EnableRaisingEvents = false;
                process.OutputDataReceived += (sender, eventArgs) => outputStringBuilder.AppendLine(eventArgs.Data);
                process.ErrorDataReceived += (sender, eventArgs) => errorStringBuilder.AppendLine(eventArgs.Data);
                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                var processExited = process.WaitForExit(timeoutMs);

                if (processExited == false) // we timed out...
                {
                    if (throwOnError)
                    {
                        process.Kill();
                        throw new Exception("ERROR: Process took too long to finish");
                    }
                    else
                    {
                        // ignore
                    }
                }
                else if (process.ExitCode != 0)
                {
                    if (throwOnError)
                    {
                        var output = outputStringBuilder.ToString();

                        throw new Exception("Process exited with non-zero exit code of: " + process.ExitCode + Environment.NewLine +
                        "Output from process: " + outputStringBuilder.ToString());
                    }
                    else
                    {
                        // ignore
                    }
                }
            }
            catch (Exception e)
            {
                if (throwOnError)
                {
                    throw new Exception("Execution error: " + e.Message);
                }
                else
                {
                    Console.Out.WriteLine(" exec error: " + e.Message);
                }
            }
            finally
            {
                process.Close();
            }

            errorOutput = errorStringBuilder.ToString();
            return outputStringBuilder.ToString();
        }

        /// <summary>
        /// Find out what process(es) have a lock on the specified file.
        /// </summary>
        /// <param name="path">Path of the file.</param>
        /// <returns>Processes locking the file</returns>
        /// <remarks>See also:
        /// http://msdn.microsoft.com/en-us/library/windows/desktop/aa373661(v=vs.85).aspx
        /// http://wyupdate.googlecode.com/svn-history/r401/trunk/frmFilesInUse.cs (no copyright in code at time of viewing)
        /// 
        /// </remarks>
        static public List<Process> WhoIsLocking(string path)
        {
            string key = Guid.NewGuid().ToString();
            List<Process> processes = new List<Process>();

            int res = RmStartSession(out uint handle, 0, key);
            if (res != 0) throw new Exception("Could not begin restart session.  Unable to determine file locker.");

            try
            {
                const int ERROR_MORE_DATA = 234;
                uint pnProcInfo = 0,
                    lpdwRebootReasons = RmRebootReasonNone;

                string[] resources = new string[] { path }; // Just checking on one resource.

                res = RmRegisterResources(handle, (uint)resources.Length, resources, 0, null, 0, null);

                if (res != 0) throw new Exception("Could not register resource.");

                //Note: there's a race condition here -- the first call to RmGetList() returns
                //      the total number of process. However, when we call RmGetList() again to get
                //      the actual processes this number may have increased.
                res = RmGetList(handle, out uint pnProcInfoNeeded, ref pnProcInfo, null, ref lpdwRebootReasons);

                if (res == ERROR_MORE_DATA)
                {
                    // Create an array to store the process results
                    RM_PROCESS_INFO[] processInfo = new RM_PROCESS_INFO[pnProcInfoNeeded];
                    pnProcInfo = pnProcInfoNeeded;

                    // Get the list
                    res = RmGetList(handle, out pnProcInfoNeeded, ref pnProcInfo, processInfo, ref lpdwRebootReasons);
                    if (res == 0)
                    {
                        processes = new List<Process>((int)pnProcInfo);

                        // Enumerate all of the results and add them to the 
                        // list to be returned
                        for (int i = 0; i < pnProcInfo; i++)
                        {
                            try
                            {
                                processes.Add(Process.GetProcessById(processInfo[i].Process.dwProcessId));
                            }
                            // catch the error -- in case the process is no longer running
                            catch (ArgumentException) { }
                        }
                    }
                    else throw new Exception("Could not list processes locking resource.");
                }
                else if (res != 0) throw new Exception("Could not list processes locking resource. Failed to get size of result.");
            }
            finally
            {
                RmEndSession(handle);
            }

            return processes;
        }

        [DllImport("kernel32.dll")]
        public static extern bool MoveFileEx(string lpExistingFileName, string lpNewFileName, int dwFlags);

        public const int MOVEFILE_DELAY_UNTIL_REBOOT = 0x4;
    }
}

namespace System
{
    /// <summary>
    /// Process object extensions methods. You will likely need to use impersonatation when calling.
    /// </summary>
    public static class ProcessExtensions
    {
        // Attempt various means of killing process
        public static void KillRemoteProcess(this Process p, string user, string password, string domain = null)
        {
            // METHOD 1:

            try
            {
                p.Kill();
            }
            catch { }

            // METHOD 2:

            try
            {
                ConnectionOptions options = new ConnectionOptions
                {
                    Password = password,
                    Username = domain ??
                                    Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties().DomainName +
                               "\\" + user
                };

                var scope = new ManagementScope(@"\\" + p.MachineName + @"\root\cimv2", options);
                var query = new ObjectQuery("Select * from Win32_Process");
                var oSearcher = new ManagementObjectSearcher(scope, query);

                ManagementObjectCollection oReturnCollection = oSearcher.Get();

                foreach (ManagementObject oReturn in oReturnCollection)
                {
                    string[] argList = new string[] { string.Empty };

                    if (oReturn["Name"].ToString() == p.ProcessName + ".exe")
                    {
                        object[] obj = new object[] { 0 };
                        oReturn.InvokeMethod("Terminate", obj);
                    }
                }
            }
            catch { }

            // METHOD 3:

            try
            {
                new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "TaskKill.exe",
                        Arguments = string.Format("/pid {0} /s {1} /u {2} /p {3}", p.Id, p.MachineName, user, password),
                        WindowStyle = ProcessWindowStyle.Hidden,
                        CreateNoWindow = true
                    }
                }.Start();
            }
            catch { }
        }
    }
}
