using System;
using System.Data.SqlClient;
using System.Net;
using static JBToolkit.Global.DatabaseConfiguration;

namespace JBToolkit.Logger
{
    /// <summary>
    /// Adds log entries (error or information) to USR_AG_Shared_Log. Defaults to standing file logger (current working directory) on DB connection failure (fallback)
    /// </summary>
    public class DBLogger
    {
        private static string TableName { get; set; } = "[dbo].[USR_AG_Shared_Log]";

        public string DBName { get; set; }
        public int UserId { get; set; }
        public string ConnectionString { get; set; }
        public string ApplicatioName { get; set; }
        private bool TableExistanceChecked { get; set; } = false;

        public DBLogger(
            string dbName,
            DatabaseEnvironmentType environmentType,
            int userId = 0,
            string applicationName = null)
        {
            Initialise(dbName, new NetworkCredential("", Global.DatabaseConfiguration.Database.GetEnvironmentConnectionString(environmentType)).Password, userId, applicationName);
        }

        public DBLogger(
            string dbName,
            string connectionString,
            int userId = 0,
            string applicationName = null)
        {
            Initialise(dbName, connectionString, userId, applicationName);
        }

        private void Initialise(
            string dbName,
            string connectionString,
            int userId = 0,
            string applicationName = null)
        {
            DBName = dbName;
            ConnectionString = connectionString;
            UserId = userId;
            ApplicatioName = applicationName;
            CreateIfNoTableExists();
        }

        /// <summary>
        /// Logs an exception to the database
        /// </summary>
        public bool LogToDB(Exception e, string additional = null)
        {
            return LogToDB(true, e.Source, e.Message + (additional == null ? "" : " " + additional), e.StackTrace);
        }

        /// <summary>
        /// Logs an error to the database
        /// </summary>
        public bool LogToDB(
            bool isError,
            string source,
            string message,
            string stackTrace)
        {
            string dbName = DBName;

            // if DB 'still' empty: ---
            if (string.IsNullOrEmpty(dbName))
            {
                // log the initial error in Windows Event log
                FileLogger.LogError("Source: " + source + "-- Message: " + message + "-- Stack Trace: " + stackTrace);

                // log the fact that we couldn't write the error to the DB in Windows Event log
                FileLogger.LogError("Source: " + "DBlogger" + "-- Message: " + "DB instance name not set");

                return false;
            }
            else // go ahead and log the error to the DB
            {
                try
                {
                    string command = string.Format(
                        "INSERT INTO {8} (Logged_DT, IsError, UserID, Username, Application, Area, Description, StackTrace) VALUES('{0}', {1}, {2}, '{3}', '{4}', '{5}', '{6}', {7})",
                        DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"),
                        Convert.ToInt32(isError),
                        UserId,
                        GetUsername(UserId),
                        (string.IsNullOrEmpty(ApplicatioName) ? "NULL" : ApplicatioName),
                        source.GetSQLAcceptableString(),
                        message.GetSQLAcceptableString(),
                        (string.IsNullOrEmpty(stackTrace) ? "NULL" : "'" + stackTrace.GetSQLAcceptableString() + "'"),
                        TableName);

                    bool success = false;

                    using (SqlConnection conn = new SqlConnection(ConnectionString))
                    {
                        conn.Open();

                        using (var sqlCommand = new SqlCommand(@"EXECUTE sp_executesql @cmd", conn))
                        {
                            sqlCommand.Parameters.AddWithValue("@cmd", string.Format("use {0}", dbName) + " " + command);
                            sqlCommand.CommandTimeout = 5;
                            sqlCommand.ExecuteNonQuery();
                            conn.Close();

                            success = true;
                        }
                    }

                    return success;
                }
                catch (Exception e)
                {
                    // log the initial error in Windows Event log
                    FileLogger.LogError("Source: " + source + "-- Message: " + message + "-- Stack Trace: " + stackTrace);

                    // log the fact that we couldn't write the error to the DB in Windows Event log
                    FileLogger.LogError(e);

                    return false;
                }
            }
        }

        private string GetUsername(int userId)
        {
            if (userId != 0)
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(ConnectionString))
                    {
                        string command = string.Format(
                            @"SELECT TOP 1 '" + Global.ADConfiguration.AdDomain + "\' + Username_VC [Username_VC] FROM {0}.dbo.Shared_Users_T (NOLOCK) WHERE User_ID = {1}",
                            DBName,
                            userId);

                        conn.Open();

                        using (var sqlCommand = new SqlCommand(@"EXECUTE sp_executesql @cmd", conn))
                        {
                            sqlCommand.Parameters.AddWithValue("@cmd", command);
                            SqlDataReader reader = sqlCommand.ExecuteReader();

                            if (reader.HasRows)
                            {
                                if (reader.Read())
                                {
                                    try
                                    {
                                        return reader.GetString(0);
                                    }
                                    catch { }
                                }
                            }

                            reader.Close();
                            reader.Dispose();
                        }

                        conn.Close();
                    }
                }
                catch { }
            }

            return System.Security.Principal.WindowsIdentity.GetCurrent().Name;
        }

        private void CreateIfNoTableExists()
        {
            if (!TableExistanceChecked)
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(ConnectionString))
                    {
                        string command = string.Format(@"
                            IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[{0}].{1}') AND type in (N'U'))
                            BEGIN
                            CREATE TABLE [{0}].{1} (
	                            [ID] [int] IDENTITY(1,1) NOT NULL,
	                            [Logged_DT] [datetime] NOT NULL,
	                            [IsError] [bit] NOT NULL,
	                            [UserID] [int] NULL,
	                            [Username] [varchar](100) NULL,
	                            [Application] [varchar](100) NOT NULL,
	                            [Area] [varchar](500) NOT NULL,
	                            [Description] [varchar](max) NOT NULL,
	                            [StackTrace] [varchar](max) NULL,
                             CONSTRAINT [PK_USR_AG_Shared_Log] PRIMARY KEY CLUSTERED 
                            (
	                            [ID] ASC
                            )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                            ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
                            END",
                            DBName, TableName);

                        conn.Open();

                        using (var sqlUpdateCommand = new SqlCommand(@"EXECUTE sp_executesql @cmd", conn))
                        {
                            sqlUpdateCommand.Parameters.AddWithValue("@cmd", command);
                            sqlUpdateCommand.ExecuteNonQuery();
                            TableExistanceChecked = true;
                        }

                        conn.Close();
                    }
                }
                catch { }
            }
        }
    }
}