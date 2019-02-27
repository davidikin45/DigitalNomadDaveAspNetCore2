using Microsoft.EntityFrameworkCore;
using System;
using System.Data.SqlClient;

namespace AspNetCore.Base.Data.Helpers
{
    public static class DbContextConnections
    {
        public static DbContextOptions<TContext> DbContextOptionsSqlite<TContext>(string dbName)
          where TContext : DbContext
        {

            var connectionString = $"Data Source={dbName}.db;";

            var builder = new DbContextOptionsBuilder<TContext>();
            builder.UseSqlite(connectionString);
            builder.EnableSensitiveDataLogging();
            return builder.Options;
        }

        public static DbContextOptions<TContext> DbContextOptionsSqliteInMemory<TContext>()
         where TContext : DbContext
        {

            var connectionString = "DataSource=:memory:";

            var builder = new DbContextOptionsBuilder<TContext>();
            builder.UseSqlite(connectionString);
            builder.EnableSensitiveDataLogging();
            return builder.Options;
        }

        public static DbContextOptions<TContext> DbContextOptionsInMemory<TContext>(string dbName = "")
             where TContext : DbContext
        {
            if (string.IsNullOrEmpty(dbName))
            {
                dbName = Guid.NewGuid().ToString();
            }

            var builder = new DbContextOptionsBuilder<TContext>();
            builder.UseInMemoryDatabase(dbName);
            builder.EnableSensitiveDataLogging();
            return builder.Options;
        }

        public static DbContextOptions<TContext> DbContextOptionsSqlLocalDB<TContext>(string dbName)
             where TContext : DbContext
        {
            var connectionString = new SqlConnectionStringBuilder()
            {
                DataSource = @"(LocalDB)\MSSQLLocalDB",
                InitialCatalog = dbName,
                IntegratedSecurity = true,
                MultipleActiveResultSets = true
            }.ConnectionString;

            var builder = new DbContextOptionsBuilder<TContext>();
            builder.UseSqlServer(connectionString);
            builder.EnableSensitiveDataLogging();
            return builder.Options;
        }
    }
}
