using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using static JBToolkit.Database.DbResult;

namespace JBToolkit.Database
{
    /// <summary>
    /// Abstract database provider class
    /// </summary>
    public abstract partial class DbConnection
    {
        /// <summary>
        /// Returns a datatable result from an SQL query string
        /// </summary>
        /// <param name="dbName">Instance DB name to use</param>
        /// <param name="command">The SQL query string</param>
        /// <param name="commandType">Normal query or stored procedure</param>
        /// <returns>A datatable of values</returns>
        public virtual DataTable GetDataTable(
            string command,
            out string errMsg,
            CommandType commandType = CommandType.DEFAULT,
            bool throwOnError = true)
        {
            errMsg = string.Empty;

            try
            {
                return GetDataTable(command, commandType, true);
            }
            catch (Exception e)
            {
                errMsg = e.Message;

                if (EnableLogging)
                {
                    Logger.LogToDb(e, "Command: " + command);
                }

                if (throwOnError)
                {
                    throw new ApplicationException("SQL_ERROR: " + e.Message);
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Returns a datatable result from an SQL query string
        /// </summary>
        /// <param name="jqt">The pre-created Jquery Data Table object</param>
        /// <returns>DBResult Success or failure bool and any messages and results</returns>
        public virtual DbResult GetDataTableEx(string command)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            try
            {
                DataTable result = GetDataTable(command, out string errMsg, CommandType.DEFAULT, true);
                stopWatch.Stop();

                if (string.IsNullOrEmpty(errMsg))
                {
                    return new SuccessDBCommandResult(result, stopWatch.Elapsed);
                }
                else
                {
                    return new FailureDBCommandResult(errMsg, stopWatch.Elapsed);
                }
            }
            catch (Exception e)
            {
                stopWatch.Stop();
                return new FailureDBCommandResult(e.Message, stopWatch.Elapsed);
            }
        }

        /// <summary>
        /// Returns a datatable result from an SQL query string
        /// </summary>
        /// <param name="dbName">Instance DB name to use</param>
        /// <param name="command">The SQL query string</param>
        /// <param name="commandType">Normal query or stored procedure</param>
        /// <returns>A datatable of values</returns>
        public virtual DataTable GetDataTable(
            string command,
            CommandType commandType = CommandType.DEFAULT,
            bool throwOnError = true)
        {
            if (string.IsNullOrEmpty(DbName))
            {
                throw new ApplicationException("DB instance name not set");
            }

            var tb = new DataTable();

            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();

                    using (var sqlCommand = new SqlCommand(@"EXECUTE sp_executesql @cmd", conn))
                    {
                        sqlCommand.Parameters.AddWithValue("@cmd", string.Format("use {0}", DbName) + " " + command);
                        sqlCommand.CommandTimeout = 2400;

                        switch (commandType)
                        {
                            case CommandType.STORED_PROCEDURE:
                                sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                                break;
                            case CommandType.DEFAULT:
                            default:
                                break;
                        }

                        using (SqlDataReader dr = sqlCommand.ExecuteReader())
                        {
                            tb.Load(dr);
                        }
                    }

                    conn.Close();
                }
            }
            catch (Exception e)
            {
                if (EnableLogging)
                {
                    Logger.LogToDb(e, "Command: " + command);
                }

                if (throwOnError)
                {
                    throw new ApplicationException("SQL_ERROR: " + e.Message);
                }
            }

            return tb;
        }

        /// <summary>
        /// Returns a datatable result from an SQL query string (data set  can hold multiple DataTables, for example when executing
        /// store procedures that return multiple result sets).
        /// </summary>
        /// <param name="jqt">The pre-created Jquery Data Table object</param>
        /// <returns>DBResult Success or failure bool and any messages and results</returns>
        public virtual DbResult GetDataSetEx(string command)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            try
            {
                DataSet result = GetDataSet(command, out string errMsg, CommandType.DEFAULT, true);
                stopWatch.Stop();

                if (string.IsNullOrEmpty(errMsg))
                {
                    return new SuccessDBCommandResult(result, stopWatch.Elapsed);
                }
                else
                {
                    return new FailureDBCommandResult(errMsg, stopWatch.Elapsed);
                }
            }
            catch (Exception e)
            {
                stopWatch.Stop();
                return new FailureDBCommandResult(e.Message, stopWatch.Elapsed);
            }
        }

        /// <summary>
        /// Returns a datatable result from an SQL query string (data set  can hold multiple DataTables, for example when executing
        /// store procedures that return multiple result sets).
        /// </summary>
        /// <param name="dbName">Instance DB name to use</param>
        /// <param name="command">The SQL query string</param>
        /// <param name="commandType">Normal query or stored procedure</param>
        /// <returns>A datatable of values</returns>
        public virtual DataSet GetDataSet(
            string command,
            out string errMsg,
            CommandType commandType = CommandType.DEFAULT,
            bool throwOnError = true)
        {
            errMsg = string.Empty;

            try
            {
                return GetDataSet(command, commandType, true);
            }
            catch (Exception e)
            {
                errMsg = e.Message;

                if (EnableLogging)
                {
                    Logger.LogToDb(e, "Command: " + command);
                }

                if (throwOnError)
                {
                    throw new ApplicationException("SQL_ERROR: " + e.Message);
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Returns a dataset result from an SQL query string (data set  can hold multiple DataTables, for example when executing
        /// store procedures that return multiple result sets).
        /// </summary>
        /// <param name="dbName">Instance DB name to use</param>
        /// <param name="command">The SQL query string</param>
        /// <param name="commandType">Normal query or stored procedure</param>
        /// <returns>A datatable of values</returns>
        public virtual DataSet GetDataSet(
            string command,
            CommandType commandType = CommandType.DEFAULT,
            bool throwOnError = true)
        {
            if (string.IsNullOrEmpty(DbName))
            {
                throw new ApplicationException("DB instance name not set");
            }

            var tb = new DataSet();

            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();

                    DataSet dataset = new DataSet();
                    SqlDataAdapter adapter = new SqlDataAdapter
                    {
                        SelectCommand = new SqlCommand(@"EXECUTE sp_executesql @cmd", conn)
                    };
                    adapter.SelectCommand.Parameters.AddWithValue("@cmd", string.Format("use {0}", DbName) + " " + command);
                    adapter.SelectCommand.CommandTimeout = 2400;

                    switch (commandType)
                    {
                        case CommandType.STORED_PROCEDURE:
                            adapter.SelectCommand.CommandType = System.Data.CommandType.StoredProcedure;
                            break;
                        case CommandType.DEFAULT:
                        default:
                            break;
                    }

                    adapter.Fill(dataset);
                    conn.Close();

                    return dataset;
                }
            }
            catch (Exception e)
            {
                if (EnableLogging)
                {
                    Logger.LogToDb(e, "Command: " + command);
                }

                if (throwOnError)
                {
                    throw new ApplicationException("SQL_ERROR: " + e.Message);
                }
            }

            return tb;
        }

        /// <summary>
        /// Gets a single string from the database given an SQL query (first column, first row) - Typically used to retrieve HTML processed at the database level
        /// </summary>
        /// <param name="command">The SQL query string</param>
        /// <returns>A sting string - i.e. of HTML</returns>
        public virtual string GetScalerResult(string command, out string errMsg, bool throwOnError = true)
        {
            errMsg = string.Empty;
            string result = string.Empty;

            try
            {
                result = GetScalerResult(command, true);
            }
            catch (Exception e)
            {
                errMsg = e.Message;

                if (EnableLogging)
                {
                    Logger.LogToDb(e, "Command: " + command);
                }

                if (throwOnError)
                {
                    throw new ApplicationException(e.Message);
                }
            }

            return result;
        }

        /// <summary>
        /// Gets a single string from the database given an SQL query (first column, first row) - Typically used to retrieve HTML processed at the database level
        /// </summary>
        /// <param name="command">SQL Command string</param>
        /// <returns>DBResult Success or failure bool and any messages and results</returns>
        public virtual DbResult GetScalerResultEx(string command)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            try
            {
                string result = GetScalerResult(command, true);
                stopWatch.Stop();

                if (!result.StartsWith("SQL_ERROR:"))
                {
                    return new SuccessDBCommandResult(result, stopWatch.Elapsed);
                }
                else
                {
                    return new FailureDBCommandResult(result, stopWatch.Elapsed);
                }
            }
            catch (Exception e)
            {
                stopWatch.Stop();
                return new FailureDBCommandResult(e.Message, stopWatch.Elapsed);
            }
        }

        /// <summary>
        /// Gets a single string from the database given an SQL query (first column, first row) - Typically used to retrieve HTML processed at the database level
        /// </summary>
        /// <param name="command">The SQL query string</param>
        /// <returns>A sting string - i.e. of HTML</returns>
        public virtual string GetScalerResult(string command, bool throwOnError = true)
        {
            if (string.IsNullOrEmpty(DbName))
            {
                throw new ApplicationException("DB instance name not set");
            }

            string result = string.Empty;

            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    using (var sqlCommand = new SqlCommand(@"EXECUTE sp_executesql @cmd", conn))
                    {
                        sqlCommand.Parameters.AddWithValue("@cmd", string.Format("use {0}", DbName) + " " + command);
                        sqlCommand.CommandTimeout = 240;

                        using (SqlDataReader dr = sqlCommand.ExecuteReader())
                        {
                            if (dr.HasRows)
                            {
                                if (dr.Read())
                                {
                                    result = Convert.ToString(dr[0]);
                                }
                            }
                        }

                        conn.Close();
                    }
                }
            }
            catch (Exception e)
            {
                if (EnableLogging)
                {
                    Logger.LogToDb(e, "Command: " + command);
                }

                if (throwOnError)
                {
                    throw new ApplicationException("SQL_ERROR: " + e.Message);
                }
                else
                {
                    result = "SQL_ERROR: " + e.Message;
                }
            }

            return result;
        }

        /// <summary>
        /// Executes SQL and doesn't expect a result set
        /// </summary>
        /// <param name="command"></param>
        /// <returns>DBResult Success or failure bool and any messages and results</returns>
        public virtual DbResult ExecuteNonQueryEx(string command)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            try
            {
                ExecuteNonQuery(command, out string errMsg, true);
                stopWatch.Stop();

                if (string.IsNullOrEmpty(errMsg))
                {
                    return new SuccessDBCommandResult(string.Empty, stopWatch.Elapsed);
                }
                else
                {
                    return new FailureDBCommandResult(errMsg, stopWatch.Elapsed);
                }
            }
            catch (Exception e)
            {
                stopWatch.Stop();
                return new FailureDBCommandResult(e.Message, stopWatch.Elapsed);
            }
        }

        /// <summary>
        /// Executes SQL and doesn't expect a result set
        /// </summary>
        /// <param name="command">SQL command to execute</param>
        /// <returns>True is the query executed successful, false otherwise</returns>
        public virtual bool ExecuteNonQuery(string command, out string errMsg, bool throwOnError = true)
        {
            errMsg = string.Empty;

            if (string.IsNullOrEmpty(DbName))
            {
                throw new ApplicationException("DB instance name not set");
            }

            bool success = false;

            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();

                    using (var sqlCommand = new SqlCommand(@"EXECUTE sp_executesql @cmd", conn))
                    {
                        sqlCommand.Parameters.AddWithValue("@cmd", string.Format("use {0}", DbName) + " " + command);
                        sqlCommand.CommandTimeout = 240;
                        sqlCommand.ExecuteNonQuery();
                        conn.Close();

                        success = true;
                    }
                }
            }
            catch (Exception e)
            {
                if (EnableLogging)
                {
                    Logger.LogToDb(e, "Command: " + command);
                }

                if (throwOnError)
                {
                    throw new ApplicationException("SQL_ERROR: " + e.Message);
                }
                else
                {
                    errMsg = e.Message;
                }
            }

            return success;
        }

        /// <summary>
        /// Executes SQL and will provide a scoped identity
        /// </summary>
        /// <param name="command"></param>
        /// <returns>DBResult Success or failure bool and any messages and results</returns>
        public virtual DbResult ExecuteNonQueryScopeIdentityEx(string command)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            try
            {
                ExecuteNonQueryScopeIdentity(command, out string errMsg, out string scopedIdentity, true);
                stopWatch.Stop();

                if (string.IsNullOrEmpty(errMsg))
                {
                    return new SuccessDBCommandResult(scopedIdentity, stopWatch.Elapsed);
                }
                else
                {
                    return new FailureDBCommandResult(errMsg, stopWatch.Elapsed);
                }
            }
            catch (Exception e)
            {
                stopWatch.Stop();
                return new FailureDBCommandResult(e.Message, stopWatch.Elapsed);
            }
        }

        /// <summary>
        /// Executes SQL and will provide a scoped identity
        /// </summary>
        /// <param name="command">SQL command string to execute</param>
        /// <param name="errMsg">Any error messages</param>
        /// <param name="scopedIdentity">The scoped identity after executing the command</param>
        /// <returns>True if successful, false otherwise</returns>
        public virtual bool ExecuteNonQueryScopeIdentity(
            string command,
            out string errMsg,
            out string scopedIdentity,
            bool throwOnError = true)
        {
            errMsg = string.Empty;
            scopedIdentity = "0";

            if (string.IsNullOrEmpty(DbName))
            {
                throw new ApplicationException("DB instance name not set");
            }

            bool success = false;

            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();

                    using (var sqlCommand = new SqlCommand(@"EXECUTE sp_executesql @cmd", conn))
                    {
                        sqlCommand.Parameters.AddWithValue("@cmd", string.Format("use {0}", DbName) + " " + command + "; SELECT SCOPE_IDENTITY();");
                        sqlCommand.CommandTimeout = 240;

                        scopedIdentity = sqlCommand.ExecuteScalar().ToString();
                        conn.Close();

                        success = true;
                    }
                }
            }
            catch (Exception e)
            {
                if (EnableLogging)
                {
                    Logger.LogToDb(e, "Command: " + command);
                }

                if (throwOnError)
                {
                    throw new ApplicationException("SQL_ERROR: " + e.Message);
                }
                else
                {
                    errMsg = e.Message;
                }
            }

            return success;
        }
    }
}
