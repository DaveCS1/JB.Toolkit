using JBToolkit.Logger;

namespace JBToolkit.Database
{
    /// <summary>
    /// Abstract database provider class
    /// </summary>
    public abstract partial class DbConnection
    {
        /// <summary>
        /// What kind of SQL script - a normal query or a stored procedure?
        /// </summary>
        public enum CommandType
        {
            DEFAULT,
            STORED_PROCEDURE
        }

        public string DbName { get; set; }
        public string ConnectionString { get; set; }
        public bool EnableLogging { get; set; }
        public bool UseSessionForSharedObjects { get; set; }

        /// <summary>
        /// Include logger class for logging of errors
        /// </summary>
        public DbLogger Logger { get; set; }

        public DbConnection(
            string dbName,
            string connectionString,
            int userId = 0,
            bool enableLogging = true,
            string applicationName = null)
        {
            Initialise(
                dbName,
                connectionString,
                userId,
                enableLogging,
                applicationName);
        }

        private void Initialise(string dbName, string connectionString,
            int userId = 0, bool enableLogging = true, string applicationName = null)
        {
            DbName = dbName;
            ConnectionString = connectionString;
            EnableLogging = enableLogging;

            Logger = new DbLogger(dbName, ConnectionString, userId, applicationName);
        }
    }

    /// <summary>
    /// Use to initialise the simplest DB class
    /// </summary>
    public class DbGeneric : DbConnection
    {
        public DbGeneric(string dbname, string connectionString, string applicationName) :
            base(dbname, connectionString, applicationName: applicationName)
        { }
    }
}
