using System;
using System.Data.SqlClient;
using System.Net;
using static JBToolkit.Global.DatabaseConfiguration;

namespace JBToolkit.Database
{
    public class DataTableState
    {
        public string TableID { get; set; }
        public string TableSettings { get; set; }
    }

    /// <summary>
    /// For saving JQuery DataTable states (column order, column visibility, filters etc) - Saves in a provided DataBase. If the standard table doesn't exist this class will create it
    /// </summary>
    public class DataTableStates
    {
        private static string TableName { get; set; } = "[dbo].[USR_AG_SS_DataTableStates_T]";

        private string DBName { get; set; }
        private string ConnectionString { get; set; }
        private bool TableExistanceChecked { get; set; } = false;

        public DataTableStates(string dbName, DatabaseEnvironmentType environmentType)
        {
            Initialise(dbName, new NetworkCredential("", Global.DatabaseConfiguration.Database.GetEnvironmentConnectionString(environmentType)).Password);
        }

        public DataTableStates(string dbName, string connectionString)
        {
            Initialise(dbName, connectionString);
        }

        private void Initialise(string dbName, string connectionString)
        {
            DBName = dbName;
            ConnectionString = connectionString;
            CreateIfNoTableExists();
        }

        public string GetTableState(string tableId)
        {
            string result = string.Empty;

            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();

                    using (var sqlCommand = new SqlCommand(@"EXECUTE sp_executesql @cmd", conn))
                    {
                        sqlCommand.Parameters.AddWithValue(
                                                   "@cmd",
                                                   string.Format(@"SELECT TOP 1 TableID, TableSettings FROM {0}.{2} (NOLOCK) WHERE TableID = '{1}'",
                                                            DBName,
                                                            tableId,
                                                            TableName));

                        sqlCommand.CommandTimeout = 240;
                        sqlCommand.ExecuteNonQuery();

                        SqlDataReader reader = sqlCommand.ExecuteReader();
                        if (reader.HasRows)
                        {
                            if (reader.Read())
                            {
                                result = reader["TableSettings"].ToString();
                            }
                        }

                        reader.Close();
                        reader.Dispose();
                    }

                    conn.Close();
                }
            }
            catch (Exception e)
            {
                result = "SQL_ERROR: " + e.Message;
            }

            return result;
        }

        public bool UpdateTableState(string tableId, string tableSettings)
        {
            try
            {
                string content = string.Empty;

                // Table state exists

                bool tableStateExists = false;

                // CHECK FOR EXISTING ROWS ------------------------------------------------------------------------------
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();

                    using (var sqlCommand = new SqlCommand(@"EXECUTE sp_executesql @cmd", conn))
                    {
                        sqlCommand.Parameters.AddWithValue(
                                                "@cmd",
                                                string.Format(@"SELECT TOP 1 TableID, TableSettings FROM {0}.{2} (NOLOCK) WHERE TableID = '{1}'",
                                                        DBName,
                                                        tableId,
                                                        TableName));

                        SqlDataReader reader = sqlCommand.ExecuteReader();

                        if (reader.HasRows)
                        {
                            tableStateExists = true;
                        }
                        else
                        {
                            tableStateExists = false;
                        }

                        reader.Close();
                        reader.Dispose();
                    }

                    conn.Close();
                }

                if (tableStateExists)
                {
                    // UPDATE

                    using (SqlConnection conn = new SqlConnection(ConnectionString))
                    {
                        conn.Open();

                        using (var sqlUpdateCommand = new SqlCommand(@"EXECUTE sp_executesql @cmd", conn))
                        {
                            sqlUpdateCommand.Parameters.AddWithValue(
                                                            "@cmd",
                                                            string.Format("UPDATE {0}.{3} SET TableSettings = N'{1}' WHERE TableID = '{2}'",
                                                                    DBName,
                                                                    tableSettings,
                                                                    tableId,
                                                                    TableName));

                            sqlUpdateCommand.ExecuteNonQuery();
                            conn.Close();
                        }
                    }
                }
                else
                {
                    // ADD

                    using (SqlConnection conn = new SqlConnection(ConnectionString))
                    {
                        conn.Open();

                        using (var sqlUpdateCommand = new SqlCommand(@"EXECUTE sp_executesql @cmd", conn))
                        {
                            sqlUpdateCommand.Parameters.AddWithValue(
                                                            "@cmd",
                                                            string.Format("INSERT INTO {0}.{3} (TableID, TableSettings) VALUES ('{1}', N'{2}')",
                                                                    DBName,
                                                                    tableId,
                                                                    tableSettings,
                                                                    TableName));

                            sqlUpdateCommand.ExecuteNonQuery();
                            conn.Close();
                        }
                    }
                }

                return true; // success
            }
            catch
            {
                return false;
            }
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
                            CREATE TABLE [{0}].{1}(
	                            [TableID] [varchar](200) NULL,
	                            [TableSettings] [nvarchar](max) NULL
                            ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
                            END
                            SET ANSI_PADDING ON
                            IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[{0}].{1}') AND name = N'ClusteredIndex-20190922-174523')
                            CREATE CLUSTERED INDEX [ClusteredIndex-20190922-174523] ON [{0}].{1}
                            (
	                            [TableID] ASC
                            )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]",
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