using Dapper;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace JBToolkit.Database
{
    /// <summary>
    /// Abstract database provider class
    /// </summary>
    public abstract partial class DBConnection
    {
        /// <summary>
        /// Uses Dapper to query the database, returning an IEnumerable of the object
        /// </summary>
        /// <typeparam name="T">Type of object</typeparam>
        /// <param name="sql">SQL command</param>
        /// <param name="sqlParameters">Parameters in the form @parameterName = valueOrObject - @paremeterName is Annoymous type (i.e. the @) </param>
        /// <returns>IEnumerable of type of oject</returns>
        protected async Task<IEnumerable<T>> QueryAsync<T>(string sql, object sqlParameters = null)
        {
            using (var conn = new SqlConnection(ApplyInitialCatalogToConnectionString(DBName, ConnectionString)))
            {
                return (await conn.QueryAsync<T>(sql, sqlParameters));
            }
        }

        /// <summary>
        /// Uses Dapper to query the database, returning an object of a given type - returning object of a single row
        /// </summary>
        /// <typeparam name="T">Type of object</typeparam>
        /// <param name="sql">SQL command</param>
        /// <param name="sqlParameters">Parameters in the form @parameterName = valueOrObject - @paremeterName is Annoymous type (i.e. the @) </param>
        /// <returns>object of a given type</returns>
        protected async Task<T> FirstOrDefaultAsync<T>(string sql, object sqlParameters)
        {
            using (var conn = new SqlConnection(ApplyInitialCatalogToConnectionString(DBName, ConnectionString)))
            {
                return (await conn.QueryFirstOrDefaultAsync<T>(sql, sqlParameters));
            }
        }

        /// <summary>
        /// Uses Dapper to query the database, returning an object of a given type - returning object of a single row
        /// </summary>
        /// <typeparam name="T">Type of object</typeparam>
        /// <param name="sql">SQL command</param>
        /// <param name="sqlParameters">Parameters in the form @parameterName = valueOrObject - @paremeterName is Annoymous type (i.e. the @) </param>
        /// <returns>object of a given type</returns>
        protected async Task<T> SingleOrDefaultAsync<T>(string sql, object sqlParameters = null)
        {
            using (var conn = new SqlConnection(ApplyInitialCatalogToConnectionString(DBName, ConnectionString)))
            {
                return (await conn.QuerySingleOrDefaultAsync<T>(sql, sqlParameters));
            }
        }

        /// <summary>
        /// Uses Dapper to execute a command (i.e. insert or udpate statement) - Returning an integer if a row update has been performed with a primary key or increment in the table
        /// </summary>
        /// <param name="sql">SQL command</param>
        /// <param name="sqlParameters">Parameters in the form @parameterName = valueOrObject - @paremeterName is Annoymous type (i.e. the @) </param>
        /// <returns>Integer of row update primary key or increment</returns>
        protected async Task<int> ExecuteAsync(string sql, object sqlParameters)
        {
            using (var conn = new SqlConnection(ApplyInitialCatalogToConnectionString(DBName, ConnectionString)))
            {
                return await conn.ExecuteAsync(sql, sqlParameters);
            }
        }

        /// <summary>
        /// Uses Dapper to query the database, returning an object of a given type - returning object of a single row
        /// </summary>
        /// <typeparam name="T">Type of object</typeparam>
        /// <param name="sql">SQL command</param>
        /// <param name="sqlParameters">Parameters in the form @parameterName = valueOrObject - @paremeterName is Annoymous type (i.e. the @) </param>
        /// <returns>object of a given type</returns>
        protected async Task<T> ExecuteScalarAsync<T>(string sql, object sqlParameters)
        {
            using (var conn = new SqlConnection(ApplyInitialCatalogToConnectionString(DBName, ConnectionString)))
            {
                return (await conn.ExecuteScalarAsync<T>(sql, sqlParameters));
            }
        }

        public static string ApplyInitialCatalogToConnectionString(string databaseName, string connectionString)
        {
            var splits = connectionString.Split(';');
            var actual = "";
            foreach (var split in splits)
            {
                if (!string.IsNullOrWhiteSpace(split))
                {
                    var secondSplit = split.Split('=');
                    var key = secondSplit[0];
                    var value = secondSplit[1];
                    if (key == "Initial Catalog")
                    {
                        value = databaseName;
                    }
                    actual += $"{key}={value};";
                }
            }
            return actual;
        }
    }
}
