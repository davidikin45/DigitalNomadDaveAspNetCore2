using LiteDB;
using System;
using System.IO;

namespace AspNetCore.Base.Data.NoSql
{
    //http://www.litedb.org/
    public abstract class DbContextNoSql : LiteRepository, IDisposable
    {
        public string ConnectionString { get; }
        public DbContextNoSql(string connectionString)
            :base(connectionString)
        {
            ConnectionString = connectionString;
        }

        private readonly MemoryStream _memoryStream;
        public DbContextNoSql(MemoryStream memryStream)
           : base(memryStream)
        {
            _memoryStream = memryStream;
        }

        public abstract void Seed();

        public new void Dispose()
        {
            base.Dispose();

            if (_memoryStream != null)
                _memoryStream.Dispose();
        }
    }
}
