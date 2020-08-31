using System;
using System.Data.SqlClient;

namespace JBToolkit.Database
{
    /// <summary>
    /// For saving JSTree states (hierarchy, expanded nodes) - Saves in a provided DataBase. If the standard table doesn't exist this class will create it
    /// </summary>
    public class TreeViewCache
    {
        private static string TableName { get; set; } = "[dbo].[USR_AG_SS_TreeViewCache_T]";

        private string DBName { get; set; }
        private string ConnectionString { get; set; }
        private int TreeViewCacheTimeout { get; set; }
        private bool TableExistanceChecked { get; set; } = false;

        public TreeViewCache(string dbName, string connectionString, int treeViewCacheTimeout)
        {
            Initialise(dbName, connectionString, treeViewCacheTimeout);
        }

        private void Initialise(string dbName, string connectionString, int treeViewCacheTimeout)
        {
            DBName = dbName;
            ConnectionString = connectionString;
            TreeViewCacheTimeout = treeViewCacheTimeout;
            CreateIfNoTableExists();
        }

        public string GetTreeViewData(string linkId, string linkType, int userId)
        {
            string result = GetTreeViewDataUserSpecifc(linkId, linkType, userId);

            if (string.IsNullOrEmpty(result))
            {
                // This method is basically using SQL as a cache for performace purpses as we're storing serialised tree data in there. 
                // We're also sharing data between users where the data is not marked as sensitve        

                string altResult = GetTreeViewDataGeneric(linkId, linkType, out int cachedUserId, out bool cachedIncludesSensitveData);

                if (cachedUserId == userId || !cachedIncludesSensitveData)
                {
                    return altResult;
                }
            }

            return result;
        }

        public bool ClearCacheEntry(string linkId, string linkType, out string errMsg)
        {
            errMsg = string.Empty;

            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    using (var sqlUpdateCommand = new SqlCommand(@"EXECUTE sp_executesql @cmd", conn))
                    {
                        sqlUpdateCommand.Parameters.AddWithValue(
                                                        "@cmd",
                                                        string.Format(
                                                                "DELETE FROM {0}.{3} WHERE LinkID = '{1}' AND LinkType = '{2}'",
                                                                DBName,
                                                                linkId,
                                                                linkType,
                                                                TableName));

                        sqlUpdateCommand.ExecuteNonQuery();
                        conn.Close();

                        return true;
                    }
                }
            }
            catch (Exception e)
            {
                errMsg = e.Message;
                return false;
            }
        }

        private string GetTreeViewDataGeneric(string linkId, string linkType, out int userId, out bool includesSentiveData)
        {
            string result = string.Empty;
            includesSentiveData = true;
            userId = 0;

            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    using (var sqlCommand = new SqlCommand(@"EXECUTE sp_executesql @cmd", conn))
                    {
                        sqlCommand.Parameters.AddWithValue(
                                                        "@cmd",
                                                        string.Format(
                                                                @"SELECT TOP 1 TreeData, UserID, IncludesSensitiveData FROM {0}.{4} (NOLOCK) 
                                                                WHERE LinkID = '{1}' AND LinkType = '{2}' AND LastModifiedDT > DATEADD(hour, -{3}, GETDATE())",
                                                                DBName,
                                                                linkId,
                                                                linkType,
                                                                TreeViewCacheTimeout,
                                                                TableName));

                        SqlDataReader reader = sqlCommand.ExecuteReader();

                        if (reader.HasRows)
                        {
                            if (reader.Read())
                            {
                                result = reader["TreeData"].ToString();

                                if (!reader.IsDBNull(2))
                                {
                                    includesSentiveData = Convert.ToBoolean(reader["IncludesSensitiveData"].ToString());
                                }

                                userId = Convert.ToInt32(reader["UserID"]);
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

        private string GetTreeViewDataUserSpecifc(string linkId, string linkType, int userId)
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
                                                        string.Format(
                                                                @"SELECT TOP 1 TreeData FROM {0}.{5} (NOLOCK) 
                                                                WHERE LinkID = '{1}' AND LinkType = '{2}' AND UserID = {3} AND LastModifiedDT > DATEADD(hour, -{4}, GETDATE())",
                                                                DBName,
                                                                linkId,
                                                                linkType,
                                                                userId,
                                                                TreeViewCacheTimeout,
                                                                TableName));

                        SqlDataReader reader = sqlCommand.ExecuteReader();
                        if (reader.HasRows)
                        {
                            if (reader.Read())
                            {
                                result = reader["TreeData"].ToString();
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

        public bool UpdateTreeViewData(string linkId, string linkType, int userId, string treeData, bool includesSensitveData)
        {
            try
            {
                string content = string.Empty;

                // Tree view data exists

                bool tableStateExists = false;

                // CHECK FOR EXISTING ROWS ------------------------------------------------------------------------------
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    using (var sqlCommand = new SqlCommand(@"EXECUTE sp_executesql @cmd", conn))
                    {
                        sqlCommand.Parameters.AddWithValue(
                                                        "@cmd",
                                                        string.Format(
                                                                @"SELECT TOP 1 TreeData FROM {0}.{4} (NOLOCK) 
                                                                WHERE LinkID = '{1}' AND LinkType = '{2}' AND UserID = {3}",
                                                                DBName,
                                                                linkId,
                                                                linkType,
                                                                userId,
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
                        using (var sqlCommand = new SqlCommand(@"EXECUTE sp_executesql @cmd", conn))
                        {
                            sqlCommand.Parameters.AddWithValue(
                                                            "@cmd",
                                                            string.Format(
                                                                    "UPDATE {0}.{6} SET TreeData = N'{1}', LastModifiedDT = GETDATE(), IncludesSensitiveData = {2} WHERE LinkID = '{3}' AND LinkType = '{4}' AND UserID = {5}",
                                                                    DBName,
                                                                    treeData.GetSQLAcceptableString(),
                                                                    Convert.ToInt32(includesSensitveData),
                                                                    linkId,
                                                                    linkType,
                                                                    userId,
                                                                    TableName));

                            sqlCommand.ExecuteNonQuery();
                        }

                        conn.Close();
                    }

                }
                else
                {
                    // ADD

                    using (SqlConnection conn = new SqlConnection(ConnectionString))
                    {
                        conn.Open();
                        using (var sqlCommand = new SqlCommand(@"EXECUTE sp_executesql @cmd", conn))
                        {
                            sqlCommand.Parameters.AddWithValue(
                                                            "@cmd",
                                                            string.Format(
                                                                    "INSERT INTO {0}.{6} (LinkID, LinkType, LastModifiedDT, TreeData, UserID, IncludesSensitiveData) VALUES ('{1}', '{2}', GETDATE(), N'{3}', {4}, {5})",
                                                                    DBName,
                                                                    linkId,
                                                                    linkType,
                                                                    treeData.GetSQLAcceptableString(),
                                                                    userId,
                                                                    Convert.ToInt32(includesSensitveData),
                                                                    TableName));
                            sqlCommand.ExecuteNonQuery();
                        }

                        conn.Close();
                    }
                }

                return true; // success
            }
            catch (Exception)
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
	                            [LinkID] [varchar](20) NULL,
	                            [LinkType] [varchar](100) NULL,
	                            [UserID] [int] NULL,
	                            [LastModifiedDT] [datetime] NULL,
	                            [TreeData] [nvarchar](max) NULL,
	                            [IncludesSensitiveData] [bit] NULL
                            ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
                            END
                            SET ANSI_PADDING ON
                            IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[{0}].{1}') AND name = N'ClusteredIndex-20190922-193804')
                            CREATE CLUSTERED INDEX [ClusteredIndex-20190922-193804] ON [{0}].{1}
                            (
	                            [LinkID] ASC,
	                            [LinkType] ASC,
	                            [UserID] ASC
                            )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]",
                            DBName, TableName);

                        conn.Open();
                        using (var sqlCommand = new SqlCommand(@"EXECUTE sp_executesql @cmd", conn))
                        {
                            sqlCommand.Parameters.AddWithValue("@cmd", command);
                            sqlCommand.ExecuteNonQuery();
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