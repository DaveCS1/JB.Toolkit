using JBToolkit.Domain.Impersonation;
using System;
using System.Diagnostics;
using System.Text;

namespace JBToolkit.Logger
{
    /// <summary>
    /// Log error to Windows' Event Log
    /// </summary>
    public static class WindowsEventLogger
    {
        public class EventLoggerResult
        {
            public bool Success { get; set; }
            public string Message { get; set; }
        }

        /// <summary>
        /// Log an event to the Windows' Application event log
        /// </summary>
        /// <param name="logType">i.e. Error, warning, information</param>
        public static EventLoggerResult LogEvent(string message, string source, EventLogEntryType logType)
        {
            var result = new EventLoggerResult();

            try
            {
                using (EventLog eventLog = new EventLog("Application"))
                {
                    if (!EventLog.SourceExists(source))
                    {
                        EventLog.CreateEventSource(source, "Application");
                    }

                    eventLog.Source = source;
                    eventLog.WriteEntry(message, logType);

                    result.Success = true;
                    result.Message = "Event logged successfully";
                }
            }
            catch
            {
                try
                {
                    if (Domain.DomainHelper.SiteOnDomain)
                    {
                        using (new Impersonator(ServerType.DOMAIN))
                        {
                            using (EventLog eventLog = new EventLog("Application"))
                            {
                                if (!EventLog.SourceExists(source))
                                {
                                    EventLog.CreateEventSource(source, "Application");
                                }

                                eventLog.Source = source;
                                eventLog.WriteEntry(message, logType);

                                result.Success = true;
                                result.Message = "Event logged successfully";
                            }
                        }
                    }
                    else
                    {
                        using (new Impersonator(ServerType.DMZ))
                        {
                            using (EventLog eventLog = new EventLog("Application"))
                            {
                                if (!EventLog.SourceExists(source))
                                {
                                    EventLog.CreateEventSource(source, "Application");
                                }

                                eventLog.Source = source;
                                eventLog.WriteEntry(message, logType);

                                result.Success = true;
                                result.Message = "Event logged successfully";
                            }
                        }
                    }
                }
                catch
                {
                    try
                    {
                        using (EventLog eventLog = new EventLog("Application"))
                        {
                            eventLog.Source = "Application";
                            eventLog.WriteEntry(message, logType);

                            result.Success = true;
                            result.Message = "Event logged but source wasn't foudn or created. Logged under 'Application' instead.";
                        }
                    }
                    catch (Exception ex)
                    {
                        result.Success = false;
                        result.Message = "Event logging failed: " + ex.Message;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Log an event to the Windows' Application event log
        /// </summary>
        /// <param name="logType">i.e. Error, warning, information</param>
        public static EventLoggerResult LogEvent(string message, EventLogEntryType logType)
        {
            return LogEvent(message, "Application", logType);
        }

        /// <summary>
        /// Log an event to the Windows' Application event log as information only (no error or warning)
        /// </summary>
        public static EventLoggerResult LogInfo(string message, string source = null)
        {
            return LogEvent(message, source ?? "Application", EventLogEntryType.Information);
        }

        /// <summary>
        /// Log an event to the Windows' Application event log as a warning
        /// </summary>
        public static EventLoggerResult LogWarning(string message, string source = null)
        {
            return LogEvent(message, source ?? "Application", EventLogEntryType.Warning);
        }

        /// <summary>
        /// Log an event to the Windows' Application event log as an error
        /// </summary>
        public static EventLoggerResult LogError(string message, string source = null)
        {
            return LogEvent(message, source ?? "Application", EventLogEntryType.Error);
        }

        /// <summary>
        /// Log an event to the Windows' Application event log as an error from an exception object... Will be formatted properly in the event log
        /// </summary>
        public static EventLoggerResult LogError(Exception e, string source = null)
        {
            return LogEvent(FormatExceptionMessage(e), source ?? "Application", EventLogEntryType.Error);
        }

        private static string FormatExceptionMessage(Exception e)
        {
            StringBuilder sbExceptionMessage = new StringBuilder();

            sbExceptionMessage.Append("An Exception has been raised:\r\n\r\n");
            sbExceptionMessage.Append("");
            sbExceptionMessage.Append(e.Message);
            sbExceptionMessage.Append("\r\n");
            sbExceptionMessage.Append(e.Source);
            sbExceptionMessage.Append("\r\n");
            sbExceptionMessage.Append(e.StackTrace);

            if (e.InnerException != null)
            {
                sbExceptionMessage.Append("\r\n\r\nAdditional Exception details:\r\n\r\n");
                sbExceptionMessage.Append(e.InnerException.Message);
                sbExceptionMessage.Append("\r\n");
                sbExceptionMessage.Append(e.InnerException.Source);
                sbExceptionMessage.Append("\r\n");
                sbExceptionMessage.Append(e.InnerException.StackTrace);

                if (e.InnerException.InnerException != null)
                {
                    sbExceptionMessage.Append("\r\n\r\nAdditional Exception details:\r\n\r\n");
                    sbExceptionMessage.Append(e.InnerException.InnerException.Message);
                    sbExceptionMessage.Append("\r\n");
                    sbExceptionMessage.Append(e.InnerException.InnerException.Source);
                    sbExceptionMessage.Append("\r\n");
                    sbExceptionMessage.Append(e.InnerException.InnerException.StackTrace);
                }
            }

            return sbExceptionMessage.ToString();
        }
    }
}
