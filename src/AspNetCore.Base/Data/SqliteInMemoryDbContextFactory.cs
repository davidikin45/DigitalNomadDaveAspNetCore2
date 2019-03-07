using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetCore.Base.Data
{
    //https://www.meziantou.net/2017/09/11/testing-ef-core-in-memory-using-sqlite
    public class SqliteInMemoryDbContextFactory<TDbContext> : IDisposable
        where TDbContext : DbContext
    {
        private DbConnection _connection;

        private DbContextOptions<TDbContext> CreateOptions()
        {
            return new DbContextOptionsBuilder<TDbContext>()
                .UseSqlite(_connection)
                .EnableSensitiveDataLogging()
                .Options;
        }

        //cant create and seed using the same context
        public async Task<TDbContext> CreateContextAsync(bool create = true, CancellationToken cancellationToken = default)
        {
            if (_connection == null)
            {
                _connection = new SqliteConnection("DataSource=:memory:");
                await _connection.OpenAsync(cancellationToken);

                if(create)
                {
                    using (var context = (TDbContext)Activator.CreateInstance(typeof(TDbContext), CreateOptions()))
                    {
                        await context.Database.EnsureCreatedAsync(cancellationToken);
                    }
                }
            }

            return (TDbContext)Activator.CreateInstance(typeof(TDbContext), CreateOptions());
        }

        public void Dispose()
        {
            if (_connection != null)
            {
                _connection.Dispose();
                _connection = null;
            }
        }
    }
}
