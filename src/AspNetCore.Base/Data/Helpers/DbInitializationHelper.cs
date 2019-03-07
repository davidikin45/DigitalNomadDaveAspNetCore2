using AspNetCore.Base.Helpers;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetCore.Base.Data.Helpers
{
    public static class DbInitializationHelper
    {
        public static async Task<bool> ExistsAsync(string connectionString, CancellationToken cancellationToken = default)
        {
            try
            {
                await TestConnectionAsync(connectionString, cancellationToken);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static async Task TestConnectionAsync(string connectionString, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new Exception("Connection String empty");          
            }
            else if (ConnectionStringHelper.IsSQLite(connectionString))
            {
                var builder = new SqliteConnectionStringBuilder(connectionString);
                builder.Mode = SqliteOpenMode.ReadOnly;

                using (var conn = new SqliteConnection(builder.ConnectionString))
                {
                    await conn.OpenAsync(cancellationToken);
                }
            }
            else
            {
                using (var conn = new SqlConnection(connectionString))
                {
                    await conn.OpenAsync(cancellationToken);
                }
            }
        }

        public static async Task<bool> HasTablesAsync(string connectionString)
        {
            var count = await TableCountAsync(connectionString);
            return count != 0;
        }

        public static async Task<long> TableCountAsync(string connectionString, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                return 0;
            }
            else if (ConnectionStringHelper.IsSQLite(connectionString))
            {
                var builder = new SqliteConnectionStringBuilder(connectionString);
                builder.Mode = SqliteOpenMode.ReadOnly;
                var count = await ExecuteSqliteScalarAsync<long>(builder.ConnectionString,
                    "SELECT COUNT(*) FROM \"sqlite_master\" WHERE \"type\" = 'table' AND \"rootpage\" IS NOT NULL AND \"name\" != 'sqlite_sequence';", cancellationToken);

                return count;
            }
            else
            {
                var count = await ExecuteSqlScalarAsync<int>(connectionString,
                       "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE'", cancellationToken);

                return count;
            }
        }

        public static async Task<bool> TableExistsAsync(string connectionString, string tableName, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                return false;
            }
            else if (ConnectionStringHelper.IsSQLite(connectionString))
            {
                var builder = new SqliteConnectionStringBuilder(connectionString);
                builder.Mode = SqliteOpenMode.ReadOnly;
                var exists = await ExecuteSqliteScalarAsync<long>(builder.ConnectionString,
                    $@"SELECT CASE WHEN EXISTS (
                            SELECT * FROM ""sqlite_master"" WHERE ""type"" = 'table' AND ""rootpage"" IS NOT NULL AND ""tbl_name"" = '{tableName}'
                        )
                        THEN CAST(1 AS BIT)
                        ELSE CAST(0 AS BIT) END;", cancellationToken);

                return exists == 1;
            }
            else
            {
                var exists = await ExecuteSqlScalarAsync<bool>(connectionString,
                       $@"SELECT CASE WHEN EXISTS (
                            SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE' AND TABLE_NAME = '{tableName}'
                        )
                        THEN CAST(1 AS BIT)
                        ELSE CAST(0 AS BIT) END;", cancellationToken);

                return exists;
            }
        }

        public static async Task<List<string>> TableNamesAsync(string connectionString, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                return new List<string>();
            }
            else if (ConnectionStringHelper.IsSQLite(connectionString))
            {
                var builder = new SqliteConnectionStringBuilder(connectionString);
                builder.Mode = SqliteOpenMode.ReadOnly;
                var tableNames = await ExecuteSqliteQueryAsync<string>(builder.ConnectionString,
                    "SELECT * FROM \"sqlite_master\" WHERE \"type\" = 'table' AND \"rootpage\" IS NOT NULL AND \"name\" != 'sqlite_sequence';",
                    row => "[" + (string)row["tbl_name"] + "]", cancellationToken);

                return tableNames;
            }
            else
            {
                var tableNames = await ExecuteSqlQueryAsync<string>(connectionString,
                       "SELECT '[' + TABLE_SCHEMA + '].[' + TABLE_NAME + ']' as tbl_name FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE' ORDER BY TABLE_SCHEMA, TABLE_NAME",
                       row => "[" + (string)row["tbl_name"] + "]", cancellationToken);

                return tableNames;
            }
        }

        public static async Task<bool> EnsureCreatedAsync(string connectionString, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                return false;
            }
            else if (ConnectionStringHelper.IsSQLite(connectionString))
            {
                bool exists = await DbInitializationHelper.ExistsAsync(connectionString, cancellationToken);

                if (!exists)
                {
                    using (var conn = new SqliteConnection(connectionString))
                    {
                        await conn.OpenAsync(cancellationToken);
                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                bool exists = await DbInitializationHelper.ExistsAsync(connectionString, cancellationToken);

                if(!exists)
                {
                    var masterConnectiongStringBuilder = new SqlConnectionStringBuilder(connectionString);
                    var dbName = masterConnectiongStringBuilder.InitialCatalog;

                    masterConnectiongStringBuilder.InitialCatalog = "master";
                    masterConnectiongStringBuilder.AttachDBFilename = "";

                    await ExecuteSqlCommandAsync(masterConnectiongStringBuilder.ConnectionString, $@"IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'{dbName}') 
                            BEGIN
                                    CREATE DATABASE [{dbName}];

                                    IF SERVERPROPERTY('EngineEdition') <> 5
                                    BEGIN
                                        ALTER DATABASE [{dbName}] SET READ_COMMITTED_SNAPSHOT ON;
                                    END;
                            END", cancellationToken);

                    var sqlConnection = new SqlConnection(connectionString);
                    SqlConnection.ClearPool(sqlConnection);

                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public static async Task EnsureDestroyedAsync(string connectionString, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(connectionString))
            {

            }
            else if (ConnectionStringHelper.IsSQLite(connectionString))
            {
                var builder = new SqliteConnectionStringBuilder(connectionString);

                if (!string.IsNullOrEmpty(builder.DataSource))
                {
                    if (File.Exists(builder.DataSource))
                    {
                        File.Delete(builder.DataSource);
                    }
                }
            }
            else
            {
                var masterConnectiongStringBuilder = new SqlConnectionStringBuilder(connectionString);
                var dbName = masterConnectiongStringBuilder.InitialCatalog;
                masterConnectiongStringBuilder.InitialCatalog = "master";

                string mdfFileName = string.Empty;
                string logFileName = string.Empty;
                if (!string.IsNullOrEmpty(masterConnectiongStringBuilder.AttachDBFilename))
                {
                    mdfFileName = Path.GetFullPath(masterConnectiongStringBuilder.AttachDBFilename);
                    var name = Path.GetFileNameWithoutExtension(mdfFileName);
                    logFileName = Path.ChangeExtension(mdfFileName, ".ldf");
                    var logName = name + "_log";
                    // Match default naming behavior of SQL Server
                    logFileName = logFileName.Insert(logFileName.Length - ".ldf".Length, "_log");
                }

                masterConnectiongStringBuilder.AttachDBFilename = "";

                var masterConnectionString = masterConnectiongStringBuilder.ConnectionString;

                var fileNames = await ExecuteSqlQueryAsync(masterConnectionString, @"
                SELECT [physical_name] FROM [sys].[master_files]
                WHERE [database_id] = DB_ID('" + dbName + "')",
              row => (string)row["physical_name"], cancellationToken);

                if (fileNames.Any())
                {
                    await ExecuteSqlCommandAsync(masterConnectionString, $@"
                        ALTER DATABASE [{dbName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
                        DROP DATABASE [{dbName}];", cancellationToken);

                    //ExecuteSqlCommand(masterConnectiongString, $@"
                    //    ALTER DATABASE [{dbName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
                    //    EXEC sp_detach_db '{dbName}'");
                    ////To remove a database from the current server without deleting the files from the file system, use sp_detach_db.

                    foreach (var file in fileNames)
                    {
                        if (File.Exists(file))
                        {
                            File.Delete(file);
                        }
                    }
                }

                if (!string.IsNullOrEmpty(mdfFileName) && File.Exists(mdfFileName))
                {
                    File.Delete(mdfFileName);
                }

                if (!string.IsNullOrEmpty(logFileName) && File.Exists(logFileName))
                {
                    File.Delete(logFileName);
                }
            }
        }

        #region SQLite Helper Methods
        private static async Task<TType> ExecuteSqliteScalarAsync<TType>(
        string connectionString,
        string queryText,
        CancellationToken cancellationToken = default)
        {
            TType result = default(TType);
            using (var connection = new SqliteConnection(connectionString))
            {
                await connection.OpenAsync(cancellationToken);
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = queryText;
                    result = (TType)(await command.ExecuteScalarAsync(cancellationToken));
                }
            }

            return result;
        }

        private static async Task<List<TType>> ExecuteSqliteQueryAsync<TType>(
            string connectionString,
            string queryText,
            Func<SqliteDataReader, TType> read,
            CancellationToken cancellationToken = default)
        {
            var result = new List<TType>();
            using (var connection = new SqliteConnection(connectionString))
            {
                await connection.OpenAsync();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = queryText;
                    using (var reader = await command.ExecuteReaderAsync(cancellationToken))
                    {
                        while (await reader.ReadAsync(cancellationToken))
                        {
                            result.Add(read(reader));
                        }
                    }
                }
            }
            return result;
        }
        #endregion

        #region SQL Server Helper Methods
        private static async Task ExecuteSqlCommandAsync(
        string connectionString,
        string commandText,
        CancellationToken cancellationToken = default)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync(cancellationToken);
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = commandText;
                    await command.ExecuteNonQueryAsync(cancellationToken);
                }
            }
        }

        private static async Task<TType> ExecuteSqlScalarAsync<TType>(
       string connectionString,
       string queryText,
       CancellationToken cancellationToken = default)
        {
            TType result = default(TType);
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync(cancellationToken);
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = queryText;
                    result = (TType)(await command.ExecuteScalarAsync(cancellationToken));
                }
            }

            return result;
        }

        private static async Task<List<TType>> ExecuteSqlQueryAsync<TType>(
            string connectionString,
            string queryText,
            Func<SqlDataReader, TType> read,
           CancellationToken cancellationToken = default)
        {
            var result = new List<TType>();
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync(cancellationToken);
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = queryText;
                    using (var reader = await command.ExecuteReaderAsync(cancellationToken))
                    {
                        while (await reader.ReadAsync(cancellationToken))
                        {
                            result.Add(read(reader));
                        }
                    }
                }
            }
            return result;
        }
        #endregion
    }
}
