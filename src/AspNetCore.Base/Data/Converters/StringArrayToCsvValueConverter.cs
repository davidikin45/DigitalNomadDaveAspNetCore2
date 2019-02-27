using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace AspNetCore.Base.Data.Converters
{
    public static class CsvConverterExtensions
    {
        public static PropertyBuilder<T> HasCsvValueConversion<T>(this PropertyBuilder<T> propertyBuilder) where T : class
        {
            propertyBuilder.HasConversion(new StringArrayToCsvValueConverter());

            return propertyBuilder;
        }

        public static void AddCsvValues(this ModelBuilder modelBuilder)
        {
            if (modelBuilder == null)
                throw new ArgumentNullException(nameof(modelBuilder));

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    var attributes = property.PropertyInfo?.GetCustomAttributes(typeof(CsvAttribute), false);
                    if (attributes != null && attributes.Any())
                    {
                        property.SetValueConverter(new StringArrayToCsvValueConverter());
                    }
                }
            }
        }
    }

    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    sealed class CsvAttribute : Attribute
    { }

    public class StringArrayToCsvValueConverter : ValueConverter<string[], string>
    {
        public StringArrayToCsvValueConverter()
        : base(Csv, Array)
        {
        }

        private static Expression<Func<string[], string>>
            Csv = v => string.Join(',', v);

        private static Expression<Func<string, string[]>>
            Array = x => x.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList().Select(h => h.Trim()).ToArray();
    }
}