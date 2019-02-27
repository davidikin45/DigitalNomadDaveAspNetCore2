using AspNetCore.Base.Helpers;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;

namespace AspNetCore.Base.Data.Helpers
{
    public static class DbInitializationHelper
    {
        public static bool Exists(string connectionString)
        {
            try
            {
                TestConnection(connectionString);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static void TestConnection(string connectionString)
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
                    conn.Open();
                }
            }
            else
            {
                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                }
            }
        }

        public static bool HasTables(string connectionString)
        {
            var count = TableCount(connectionString);
            return count != 0;
        }

        public static long TableCount(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                return 0;
            }
            else if (ConnectionStringHelper.IsSQLite(connectionString))
            {
                var builder = new SqliteConnectionStringBuilder(connectionString);
                builder.Mode = SqliteOpenMode.ReadOnly;
                var count = ExecuteSqliteScalar<long>(builder.ConnectionString,
                    "SELECT COUNT(*) FROM \"sqlite_master\" WHERE \"type\" = 'table' AND \"rootpage\" IS NOT NULL AND \"name\" != 'sqlite_sequence';");

                return count;
            }
            else
            {
                var count = ExecuteSqlScalar<int>(connectionString,
                       "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE'");

                return count;
            }
        }

        public static bool TableExists(string connectionString, string tableName)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                return false;
            }
            else if (ConnectionStringHelper.IsSQLite(connectionString))
            {
                var builder = new SqliteConnectionStringBuilder(connectionString);
                builder.Mode = SqliteOpenMode.ReadOnly;
                var exists = ExecuteSqliteScalar<long>(builder.ConnectionString,
                    $@"SELECT CASE WHEN EXISTS (
                            SELECT * FROM ""sqlite_master"" WHERE ""type"" = 'table' AND ""rootpage"" IS NOT NULL AND ""tbl_name"" = '{tableName}'
                        )
                        THEN CAST(1 AS BIT)
                        ELSE CAST(0 AS BIT) END;");

                return exists == 1;
            }
            else
            {
                var exists = ExecuteSqlScalar<bool>(connectionString,
                       $@"SELECT CASE WHEN EXISTS (
                            SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE' AND TABLE_NAME = '{tableName}'
                        )
                        THEN CAST(1 AS BIT)
                        ELSE CAST(0 AS BIT) END;");

                return exists;
            }
        }

        public static List<string> TableNames(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                return new List<string>();
            }
            else if (ConnectionStringHelper.IsSQLite(connectionString))
            {
                var builder = new SqliteConnectionStringBuilder(connectionString);
                builder.Mode = SqliteOpenMode.ReadOnly;
                var tableNames = ExecuteSqliteQuery<string>(builder.ConnectionString,
                    "SELECT * FROM \"sqlite_master\" WHERE \"type\" = 'table' AND \"rootpage\" IS NOT NULL AND \"name\" != 'sqlite_sequence';",
                    row => "[" + (string)row["tbl_name"] + "]");

                return tableNames;
            }
            else
            {
                var tableNames = ExecuteSqlQuery<string>(connectionString,
                       "SELECT '[' + TABLE_SCHEMA + '].[' + TABLE_NAME + ']' as tbl_name FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE' ORDER BY TABLE_SCHEMA, TABLE_NAME",
                       row => "[" + (string)row["tbl_name"] + "]");

                return tableNames;
            }
        }

        public static bool EnsureCreated(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                return false;
            }
            else if (ConnectionStringHelper.IsSQLite(connectionString))
            {
                bool exists = DbInitializationHelper.Exists(connectionString);

                if (!exists)
                {
                    using (var conn = new SqliteConnection(connectionString))
                    {
                        conn.Open();
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
                bool exists = DbInitializationHelper.Exists(connectionString);

                if(!exists)
                {
                    var masterConnectiongStringBuilder = new SqlConnectionStringBuilder(connectionString);
                    var dbName = masterConnectiongStringBuilder.InitialCatalog;

                    masterConnectiongStringBuilder.InitialCatalog = "master";
                    masterConnectiongStringBuilder.AttachDBFilename = "";

                    ExecuteSqlCommand(masterConnectiongStringBuilder.ConnectionString, $@"IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'{dbName}') 
                            BEGIN
                                    CREATE DATABASE [{dbName}];
                                    ALTER DATABASE [{dbName}] SET READ_COMMITTED_SNAPSHOT ON;
                            END");

                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public static void EnsureDestroyed(string connectionString)
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

                var fileNames = ExecuteSqlQuery(masterConnectionString, @"
                SELECT [physical_name] FROM [sys].[master_files]
                WHERE [database_id] = DB_ID('" + dbName + "')",
              row => (string)row["physical_name"]);

                if (fileNames.Any())
                {
                    ExecuteSqlCommand(masterConnectionString, $@"
                        ALTER DATABASE [{dbName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
                        DROP DATABASE [{dbName}];");

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
        private static TType ExecuteSqliteScalar<TType>(
        string connectionString,
        string queryText)
        {
            TType result = default(TType);
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = queryText;
                    result = (TType)command.ExecuteScalar();
                }
            }

            return result;
        }

        private static List<TType> ExecuteSqliteQuery<TType>(
            string connectionString,
            string queryText,
            Func<SqliteDataReader, TType> read)
        {
            var result = new List<TType>();
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = queryText;
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
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
        private static void ExecuteSqlCommand(
        string connectionString,
        string commandText)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = commandText;
                    command.ExecuteNonQuery();
                }
            }
        }

        private static TType ExecuteSqlScalar<TType>(
       string connectionString,
       string queryText)
        {
            TType result = default(TType);
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = queryText;
                    result = (TType)command.ExecuteScalar();
                }
            }

            return result;
        }

        private static List<TType> ExecuteSqlQuery<TType>(
            string connectionString,
            string queryText,
            Func<SqlDataReader, TType> read)
        {
            var result = new List<TType>();
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = queryText;
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
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
