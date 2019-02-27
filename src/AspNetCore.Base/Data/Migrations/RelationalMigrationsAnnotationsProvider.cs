using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Sqlite.Migrations.Internal;
using Microsoft.EntityFrameworkCore.SqlServer.Migrations.Internal;
using System.Collections.Generic;
using System.Linq;

namespace AspNetCore.Base.Data.Migrations
{
    public class RelationalMigrationsAnnotationsProvider : SqlServerMigrationsAnnotationProvider
    {
        private readonly SqliteMigrationsAnnotationProvider _sqliteMigrationsAnnotationProvider;

        public RelationalMigrationsAnnotationsProvider(MigrationsAnnotationProviderDependencies dependencies)
           : base(dependencies)
        {
            _sqliteMigrationsAnnotationProvider = new SqliteMigrationsAnnotationProvider(dependencies);
        }

        public override IEnumerable<IAnnotation> For(IModel model)
        {
            return base.For(model).Concat(_sqliteMigrationsAnnotationProvider.For(model));
        }

        public override IEnumerable<IAnnotation> For(IProperty property)
        {
            return base.For(property).Concat(_sqliteMigrationsAnnotationProvider.For(property));
        }
    }
}
