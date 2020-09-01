using System;

namespace JBToolkit.Database
{
    /// <summary>
    /// Results object from a DB command
    /// </summary>
    [Serializable]
    public class DbResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public object Result { get; set; } = string.Empty;
        public string ElapsedTime { get; set; } = null;

        public class SuccessDBCommandResult : DbResult
        {
            public SuccessDBCommandResult(object result, TimeSpan? timeTaken = null)
            {
                Success = true;
                Message = "Command executed successfully";
                Result = result;
                ElapsedTime = FormatElapsedTime(timeTaken);
            }
        }

        public class FailureDBCommandResult : DbResult
        {
            public FailureDBCommandResult(string exceptionMessage, TimeSpan? timeTaken = null)
            {
                Success = false;
                Message = $"Error executing command: {exceptionMessage}";
                ElapsedTime = FormatElapsedTime(timeTaken);
            }
        }

        public static string FormatElapsedTime(TimeSpan? ts)
        {
            string elapsedTime = null;

            if (ts != null)
            {
                elapsedTime = string.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                    ((TimeSpan)ts).Hours, ((TimeSpan)ts).Minutes, ((TimeSpan)ts).Seconds,
                    ((TimeSpan)ts).Milliseconds / 10);
            }

            return elapsedTime;
        }
    }
}
