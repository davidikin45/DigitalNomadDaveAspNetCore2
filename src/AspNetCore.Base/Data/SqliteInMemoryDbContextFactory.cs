using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data.Common;

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
        public TDbContext CreateContext(bool create = true)
        {
            if (_connection == null)
            {
                _connection = new SqliteConnection("DataSource=:memory:");
                _connection.Open();

                if(create)
                {
                    using (var context = (TDbContext)Activator.CreateInstance(typeof(TDbContext), CreateOptions()))
                    {
                        context.Database.EnsureCreated();
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
