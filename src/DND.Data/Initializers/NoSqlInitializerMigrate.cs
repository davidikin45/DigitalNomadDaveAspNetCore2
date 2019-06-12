using AspNetCore.Base.Data.NoSql.Initializers;

namespace DND.Data.Initializers
{
    public class NoSqlContextInitializerMigrate : ContextInitializerNoSqlMigrate<NoSqlContext>
    {
        public override void Seed(NoSqlContext context, string tenantId)
        {
            context.Seed();
        }
    }
}
