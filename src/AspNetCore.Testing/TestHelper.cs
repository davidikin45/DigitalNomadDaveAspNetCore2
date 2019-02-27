using AspNetCore.Base.Data.Repository;
using AspNetCore.Base.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;
using System.Security.Claims;

namespace AspNetCore.Testing
{
    public static class TestHelper
    {
        public static void MockCurrentUser(this Controller controller, string userId, string username, string authenticationType)
        {
            controller.MockHttpContext(userId, username, authenticationType);
        }

        private static ClaimsPrincipal CreateClaimsPrincipal(string userId, string username, string authenticationType)
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
           {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.NameIdentifier, userId),
            }, authenticationType));

            return user;
        }

        public static void MockHttpContext(this Controller controller, string userId, string username, string authenticationType)
        {
            var httpContext = FakeAuthenticatedHttpContext(userId, username, authenticationType);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
        }

        private static HttpContext FakeAuthenticatedHttpContext(string userId, string username, string authenticationType)
        {
            var context = new DefaultHttpContext();
            context.User = CreateClaimsPrincipal(userId, username, authenticationType);
          
            return context;
        }

        public static IConfigurationRoot GetConfiguration(string environmentName = "")
        {
            var environmentNamePart = !string.IsNullOrEmpty(environmentName) ? "." + environmentName : "";
            var fileName = $"appsettings{environmentNamePart}.json";
            var basePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase.Substring(8));

            return new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile(fileName, false)
                .AddEnvironmentVariables()
                .Build();
        }

        public static GenericRepository<TEntity> GetRepository<TContext, TEntity>(string connectionString, bool beginTransaction)
            where TContext : DbContext
            where TEntity : class
        {
            var context = GetContext<TContext>(connectionString, beginTransaction);
            return new GenericRepository<TEntity>(context);
        }

        public static TContext GetContext<TContext>(string connectionString, bool beginTransaction)
          where TContext : DbContext
        {

            DbContextOptions options;
            var builder = new DbContextOptionsBuilder();
            builder.SetConnectionString<TContext>(connectionString);
            builder.EnableSensitiveDataLogging();
            options = builder.Options;

            Type type = typeof(TContext);
            ConstructorInfo ctor = type.GetConstructor(new[] { typeof(DbContextOptions) });
            object instance = ctor.Invoke(new object[] { options });

            TContext context = (TContext)ctor.Invoke(new object[] { options });

            if (beginTransaction)
            {
                context.Database.BeginTransaction();
            }

            return context;
        }

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

        public static DbContextOptions<TContext> DbContextOptionsSqlLocalDb<TContext>(string dbName)
             where TContext : DbContext
        {
            var connectionString = new SqlConnectionStringBuilder() {
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
