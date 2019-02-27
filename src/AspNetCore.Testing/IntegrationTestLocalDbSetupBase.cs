using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;

namespace DND.Common.Testing
{
    public abstract class IntegrationTestLocalDbSetupBase<T> where T : class, IDisposable
    {
        public string DBName { get; }
        public IntegrationTestLocalDbSetupBase(string MSSQLLocalDBDbNameOrConnectionString)
        {
            if(MSSQLLocalDBDbNameOrConnectionString.Contains("Server"))
            {
                IDbConnection connection = new SqlConnection(MSSQLLocalDBDbNameOrConnectionString);
                DBName = connection.Database;
            }
            else
            {
                DBName = MSSQLLocalDBDbNameOrConnectionString;
            }
        }

        [OneTimeSetUp]
        public void DatabaseSetup()
        {
            DestroyDatabase();
            CreateDatabase();
        }

        private void CreateDatabase()
        {
            ExecuteSqlCommand(Master, $@"
                CREATE DATABASE [{DBName}]
                ON (NAME = '{DBName}',
                FILENAME = '{MdfFilename}')
                ALTER DATABASE {DBName} SET READ_COMMITTED_SNAPSHOT ON");

            MigrateDatabaseAndSeed(ConnectionString);
        }

        public abstract void MigrateDatabaseAndSeed(string connectionString);

        public void DestroyDatabase()
        {
            var fileNames = ExecuteSqlQuery(Master, @"
                SELECT [physical_name] FROM [sys].[master_files]
                WHERE [database_id] = DB_ID('"+ DBName + "')",
               row => (string)row["physical_name"]);

            if (fileNames.Any())
            {
                try
                {
                    ExecuteSqlCommand(Master, $@"
                    ALTER DATABASE [{DBName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
                    EXEC sp_detach_db '{DBName}'");
                    //To remove a database from the current server without deleting the files from the file system, use sp_detach_db.
                    fileNames.ForEach(System.IO.File.Delete);
                }
                catch
                {
                    //Dropping a database deletes the database from an instance of SQL Server and deletes the physical disk files used by the database.
                    ExecuteSqlCommand(Master, $@"
                    DROP DATABASE [{DBName}];");
                }
            }

            if(File.Exists(MdfFilename))
            {
                File.Delete(MdfFilename);
            }

            if (File.Exists(LogFilename))
            {
                File.Delete(LogFilename);
            }
        }

        private static void ExecuteSqlCommand(
            SqlConnectionStringBuilder connectionStringBuilder,
            string commandText)
        {
            using (var connection = new SqlConnection(connectionStringBuilder.ConnectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = commandText;
                    command.ExecuteNonQuery();
                }
            }
        }

        private static List<TType> ExecuteSqlQuery<TType>(
            SqlConnectionStringBuilder connectionStringBuilder,
            string queryText,
            Func<SqlDataReader, TType> read)
        {
            var result = new List<TType>();
            using (var connection = new SqlConnection(connectionStringBuilder.ConnectionString))
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

        private static SqlConnectionStringBuilder Master =>
         new SqlConnectionStringBuilder
         {
             DataSource = @"(LocalDB)\MSSQLLocalDB",
             InitialCatalog = "master",
             IntegratedSecurity = true
         };

        private string ConnectionString =>
         new SqlConnectionStringBuilder
         {
             DataSource = @"(LocalDB)\MSSQLLocalDB",
             InitialCatalog = DBName,
             IntegratedSecurity = true
         }.ConnectionString;

        private string MdfFilename => Path.Combine(
            Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath),
            DBName + ".mdf");

        private string LogFilename => Path.Combine(
           Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath),
           DBName + "_log.ldf");

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            FinalDestroy();
        }

        public void Dispose()
        {
            FinalDestroy();
        }

        bool destroyed = false;
        private void FinalDestroy()
        {
            if (!destroyed)
            {
                DestroyDatabase();
                destroyed = true;
            }
        }

    }
}
