using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace AspNetCore.Base.Data.Converters
{
    public static class EncryptedConverterExtensions
    {
        public static void AddEncryptedValues(this ModelBuilder modelBuilder)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    var attributes = property.PropertyInfo.GetCustomAttributes(typeof(EncryptedAttribute), false);
                    if (attributes != null && attributes.Any())
                    {
                        property.SetValueConverter(new EncryptedConverter());
                    }
                }
            }
        }
    }

     [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    sealed class EncryptedAttribute : Attribute
    { }

    public class EncryptedConverter : ValueConverter<string, string>
    {
        public EncryptedConverter(ConverterMappingHints mappingHints = default)
            : base(EncryptExpr, DecryptExpr, mappingHints)
        { }

        static Expression<Func<string, string>> DecryptExpr = x => new string(x.Reverse().ToArray());
        static Expression<Func<string, string>> EncryptExpr = x => new string(x.Reverse().ToArray());
    }
}
