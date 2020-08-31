using System;
using System.Diagnostics;
using System.ServiceProcess;
using System.Threading;

namespace JBToolkit.Windows
{
    /// <summary>
    /// Contain method for remote reliably manipulating Windows Services. You will likely need to use impersonatation when calling i.e:
    /// 
    ///     using (new Impersonator(ServerType.DOMAIN))
    //      {
    //          GetServiceSatus("EPCInterfaceService", "HMS-DAME01-D", out errMsg);
    //      }
    /// 
    /// 
    /// </summary>
    public class ServiceHelper
    {
        /// <summary>
        /// Gets remote service status as a string. You will likely need to use impersonatation when calling.
        /// </summary>
        /// <param name="serviceName">Name of Windows service</param>
        /// <param name="serverName">Host machine to query</param>
        /// <param name="errMsg">Output error message (if any)</param>
        /// <returns>String status</returns>
        public static string GetServiceSatus(string serviceName, string serverName, ref string errMsg)
        {
            try
            {
                ServiceController sc = new ServiceController(serviceName, serverName);
                return sc.Status.ToString();
            }
            catch (Exception e)
            {
                errMsg = e.Message;
                return "Error";
            }
        }

        /// <summary>
        /// Stop a remote Windows service. You will likely need to use impersonatation when calling.
        /// </summary>
        /// <param name="serviceName">Name of Windows service</param>
        /// <param name="serverName">Host machine to query</param>
        /// <param name="errMsg">Output error message (if any)</param>
        /// <returns>True if successful, false otherwise</returns>
        public static bool StopService(string serviceName, string serverName, ref string errMsg)
        {
            try
            {
                ServiceController sc = new ServiceController(serviceName, serverName);
                sc.Stop();
                Thread.Sleep(3000); // allow 3 seconds initially

                for (int i = 0; i < 10; i++)
                {
                    try
                    {
                        if (sc.Status == ServiceControllerStatus.Stopped)
                        {
                            return true;
                        }
                    }
                    catch
                    {
                        sc = new ServiceController(serviceName, serverName);
                    }

                    Thread.Sleep(3000); // check in 3 second intervals
                }

                return false;
            }
            catch (Exception e)
            {
                errMsg = e.Message;
                return false;
            }
        }

        /// <summary>
        /// Start a remote Windows service. You will likely need to use impersonatation when calling.
        /// </summary>
        /// <param name="serviceName">Name of Windows service</param>
        /// <param name="serverName">Host machine to query</param>
        /// <param name="errMsg">Output error message (if any)</param>
        /// <returns>True if successful, false otherwise</returns>
        public static bool StartService(string serviceName, string serverName, ref string errMsg)
        {
            try
            {
                ServiceController sc = new ServiceController(serviceName, serverName);

                sc.Start();
                Thread.Sleep(3000); // allow 3 seconds to stop the service

                for (int i = 0; i < 10; i++)
                {
                    try
                    {
                        if (sc.Status == ServiceControllerStatus.Running)
                        {
                            return true;
                        }
                    }
                    catch
                    {
                        sc = new ServiceController(serviceName, serverName);
                    }

                    Thread.Sleep(3000); // check in 3 second intervals
                }

                return false;
            }
            catch (Exception e)
            {
                errMsg = e.Message;
                return false;
            }
        }

        /// <summary>
        /// Restarts a service using multiple attemps if failing. You will likely need to use impersonatation when calling. If you wish agressivly restart the service (i.e. kill the actual process if failing), you 
        /// will need to pass the impersonator object.
        /// </summary>
        /// <param name="serviceName">Name of the service to restart</param>
        /// <param name="serverName">Server host name</param>
        /// <param name="errMsg">Reference an error message string</param>
        /// <param name="processNameOptional">Optional process name</param>
        /// <param name="impersonatorOptional">Optional impersonator object</param>
        /// <returns>True if successful, false otherwise</returns>
        public static bool RestartService(string serviceName, string serverName, ref string errMsg, string processNameOptional = null, string authUsername = null, string authPassword = null)
        {
            return AgressiveRestartService(serviceName, processNameOptional, serverName, authUsername, authPassword, ref errMsg);
        }

        /// <summary>
        /// Performs an aggressive restart of a service  (i.e. kill the actual process if failing). You will likely need to use impersonatation when calling.
        /// </summary>
        /// <param name="serviceName">Name of the service to restart</param>
        /// <param name="processName">Name of the actual process of the service</param>
        /// <param name="serverName">Server host name</param>
        /// <param name="impersonator">Impersonator object</param>
        /// <param name="errMsg">Reference error message string</param>
        /// <returns>True if successful, false otherwise</returns>
        public static bool AgressiveRestartService(string serviceName, string processName, string serverName, string authUsername, string authPassword, ref string errMsg)
        {
            ServiceController sc;

            // First attempt safe stop

            try
            {
                sc = new ServiceController(serviceName, serverName);

                sc.Stop();
                Thread.Sleep(3000); // allow 3 seconds to restart first time around
            }
            catch { }

            // Kill the process just in case it's lingering and won't stop

            if (!string.IsNullOrEmpty(processName) && authUsername != null && authPassword != null)
            {
                try
                {
                    Process[] procs = ProcessHelper.GetRemoteProcess(processName, serviceName);

                    if (procs != null)
                    {
                        for (int j = 0; j < procs.Length; j++)
                        {
                            procs[j].KillRemoteProcess(authUsername, authPassword);
                        }

                        Thread.Sleep(1000);
                    }
                }
                catch { }
            }

            try
            {
                sc = new ServiceController(serviceName, serverName);

                sc.Start();
                Thread.Sleep(2000);

                // Try a couple more times

                if (sc.Status != ServiceControllerStatus.Running)
                {
                    sc.Start();
                    Thread.Sleep(3000);

                    if (sc.Status != ServiceControllerStatus.Running)
                    {
                        sc.Start();
                        Thread.Sleep(6000);

                        if (sc.Status != ServiceControllerStatus.Running)
                        {
                            sc.Start();
                            Thread.Sleep(9000);

                            if (sc.Status != ServiceControllerStatus.Running)
                            {
                                sc.Start();
                                Thread.Sleep(12000);

                                if (sc.Status != ServiceControllerStatus.Running)
                                {
                                    //### FORCE KILL #################################################################################################################################

                                    #region Force kill process

                                    // Not coming back up - Try to kill the process and try again

                                    if (!string.IsNullOrEmpty(processName) && authUsername != null && authPassword != null)
                                    {
                                        try
                                        {
                                            Process[] procs = ProcessHelper.GetRemoteProcess(processName, serverName);

                                            for (int i = 0; i < procs.Length; i++)
                                            {
                                                procs[i].KillRemoteProcess(authUsername, authPassword);
                                            }

                                            Thread.Sleep(6000);

                                            sc.Start();
                                            Thread.Sleep(9000);

                                            if (sc.Status != ServiceControllerStatus.Running)
                                            {
                                                sc.Start();
                                                Thread.Sleep(15000);

                                                if (sc.Status != ServiceControllerStatus.Running)
                                                {
                                                    errMsg = "Service status never reached 'running' after mulitple attempts and an attempt to force kill the process.";
                                                    return false;
                                                }
                                            }
                                        }
                                        catch
                                        {
                                            errMsg = "Service status never reached 'running' after mulitple attempts and an attempt to force kill the process.";
                                            return false;
                                        }
                                    }
                                    else
                                    {
                                        errMsg = "Service status never reached 'running' after mulitple attempts";
                                        return false;
                                    }

                                    #endregion

                                    //### FORCE KILL #################################################################################################################################
                                }
                            }
                        }
                    }
                }

                return true;
            }
            catch
            {
                // error occured from the 'starting' of the service - use try-catches for more attempts

                try
                {
                    sc = new ServiceController(serviceName, serverName);

                    if (sc.Status != ServiceControllerStatus.Running)
                    {
                        try
                        {
                            sc.Start();
                            Thread.Sleep(3000);
                        }
                        catch
                        {
                            Thread.Sleep(3000);
                            sc = new ServiceController(serviceName, serverName);
                        }

                        if (sc.Status != ServiceControllerStatus.Running)
                        {
                            try
                            {
                                sc.Start();
                                Thread.Sleep(6000);
                            }
                            catch
                            {
                                Thread.Sleep(6000);
                                sc = new ServiceController(serviceName, serverName);
                            }

                            if (sc.Status != ServiceControllerStatus.Running)
                            {
                                try
                                {
                                    sc.Start();
                                    Thread.Sleep(9000);
                                }
                                catch
                                {
                                    Thread.Sleep(9000);
                                    sc = new ServiceController(serviceName, serverName);
                                }

                                if (sc.Status != ServiceControllerStatus.Running)
                                {
                                    try
                                    {
                                        sc.Start();
                                        Thread.Sleep(12000);
                                    }
                                    catch
                                    {
                                        Thread.Sleep(12000);
                                        sc = new ServiceController(serviceName, serverName);
                                    }

                                    if (sc.Status != ServiceControllerStatus.Running)
                                    {
                                        //### FORCE KILL #################################################################################################################################

                                        #region Force kill process

                                        // Not coming back up - Try to kill the process and try again

                                        if (!string.IsNullOrEmpty(processName) && authUsername != null && authPassword != null)
                                        {
                                            try
                                            {
                                                Process[] procs = ProcessHelper.GetRemoteProcess(processName, serverName);

                                                for (int i = 0; i < procs.Length; i++)
                                                {
                                                    procs[i].KillRemoteProcess(authUsername, authPassword);
                                                }

                                                Thread.Sleep(6000);

                                                try
                                                {
                                                    sc.Start();
                                                    Thread.Sleep(6000);
                                                }
                                                catch
                                                {
                                                    Thread.Sleep(6000);
                                                    sc = new ServiceController(serviceName, serverName);
                                                }

                                                if (sc.Status != ServiceControllerStatus.Running)
                                                {
                                                    sc.Start();
                                                    Thread.Sleep(15000);

                                                    if (sc.Status != ServiceControllerStatus.Running)
                                                    {
                                                        errMsg = "Service status never reached 'running' after mulitple attempts and an attempt to force kill the process.";
                                                        return false;
                                                    }
                                                }
                                            }
                                            catch
                                            {
                                                errMsg = "Service status never reached 'running' after mulitple attempts and an attempt to force kill the process.";
                                                return false;
                                            }
                                        }
                                        else
                                        {
                                            errMsg = "Service status never reached 'running' after mulitple attempts";
                                            return false;
                                        }

                                        #endregion

                                        //### FORCE KILL #################################################################################################################################
                                    }
                                }
                            }
                        }
                    }

                    return true;
                }
                catch (Exception e)
                {
                    errMsg = e.Message;
                    return false;
                }
            }
        }
    }
}