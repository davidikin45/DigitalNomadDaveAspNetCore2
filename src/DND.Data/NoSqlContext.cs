using AspNetCore.Base.Data.NoSql;
using System.IO;

namespace DND.Data
{
    public class NoSqlContext : DbContextNoSql
    {
        public NoSqlContext(string connectionString)
            : base(connectionString)
        {

        }

        public NoSqlContext(MemoryStream memoryStream)
           : base(memoryStream)
        {

        }

        public override void Seed()
        {
          
        }
    }
}
