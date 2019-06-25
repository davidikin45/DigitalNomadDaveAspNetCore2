using AspNetCore.Base.Logging;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetCore.Base.Data
{
    //https://www.meziantou.net/2017/09/11/testing-ef-core-in-memory-using-sqlite
    public class SqliteInMemoryDbContextFactory<TDbContext> : SqliteInMemoryConnectionFactory
        where TDbContext : DbContext
    {
        private readonly Action<String> _logger;
        public SqliteInMemoryDbContextFactory()
        {

        }
        public SqliteInMemoryDbContextFactory(Action<String> logger)
        {
            _logger = logger;
        }

        private ILoggerFactory CommandLoggerFactory(Action<string> logger)
         => new ServiceCollection().AddLogging(builder =>
         {
             builder.AddAction(logger).AddFilter(DbLoggerCategory.Database.Command.Name, LogLevel.Information);
         }).BuildServiceProvider()
         .GetService<ILoggerFactory>();

        private bool _created = false;
        private DbContextOptions<TDbContext> CreateOptions()
        {
            return new DbContextOptionsBuilder<TDbContext>()
                .UseSqlite(_connection)
                .UseLoggerFactory(CommandLoggerFactory(_logger))
                .EnableSensitiveDataLogging()
                .Options;
        }

        //cant create and seed using the same context
        public async Task<TDbContext> CreateContextAsync(bool create = true, CancellationToken cancellationToken = default)
        {
            await GetConnection(cancellationToken);

            if (!_created && create)
            {
                using (var context = (TDbContext)Activator.CreateInstance(typeof(TDbContext), CreateOptions()))
                {
                    await context.Database.EnsureCreatedAsync(cancellationToken);
                }
                _created = true;
            }

            return (TDbContext)Activator.CreateInstance(typeof(TDbContext), CreateOptions());
        }
    }
}
