using AspNetCore.Base.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;
using System;
using System.Linq;

namespace AspNetCore.Base.Data.Converters
{
    public static class JsonConverterExtensions
    {
        public static PropertyBuilder<T> HasJsonValueConversion<T>(this PropertyBuilder<T> propertyBuilder) where T : class
        {
            propertyBuilder
            .HasConversion(new JsonValueConverter<T>())
            .Metadata.SetValueComparer(new JsonValueComparer<T>());

            return propertyBuilder;
        }

        public static void AddJsonValues(this ModelBuilder modelBuilder)
        {
            if (modelBuilder == null)
                throw new ArgumentNullException(nameof(modelBuilder));

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    var attributes = property.PropertyInfo?.GetCustomAttributes(typeof(JsonAttribute), false);
                    if ((attributes != null && attributes.Any()))
                    {
                        var modelType = property.PropertyInfo.PropertyType;
                        var converterType = typeof(JsonValueConverter<>).MakeGenericType(modelType);
                        var converter = (ValueConverter)Activator.CreateInstance(converterType, new object[] { null });
                        property.SetValueConverter(converter);
                        var valueComparer = typeof(JsonValueComparer<>).MakeGenericType(modelType);
                        property.SetValueComparer((ValueComparer)Activator.CreateInstance(valueComparer, new object[0]));
                    }
                }
            }
        }

        public static void AddMultiLangaugeStringValues(this ModelBuilder modelBuilder)
        {
            if (modelBuilder == null)
                throw new ArgumentNullException(nameof(modelBuilder));

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    var multiLanguage = property.ClrType == typeof(MultiLanguageString);
                    if (multiLanguage)
                    {
                        var modelType = property.PropertyInfo.PropertyType;
                        var converterType = typeof(JsonValueConverter<>).MakeGenericType(modelType);
                        var converter = (ValueConverter)Activator.CreateInstance(converterType, new object[] { null });
                        property.SetValueConverter(converter);
                        var valueComparer = typeof(JsonValueComparer<>).MakeGenericType(modelType);
                        property.SetValueComparer((ValueComparer)Activator.CreateInstance(valueComparer, new object[0]));
                    }
                }
            }
        }
    }

    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    sealed class JsonAttribute : Attribute
    { }

    public class JsonValueConverter<T> : ValueConverter<T, string> where T : class
    {
        public JsonValueConverter(ConverterMappingHints hints = default) :
          base(v => JsonHelper.Serialize(v), v => JsonHelper.Deserialize<T>(v), hints)
        { }
    }

    internal class JsonValueComparer<T> : ValueComparer<T>
    {

        private static string Json(T instance)
        {
            return JsonConvert.SerializeObject(instance);
        }

        private static T DoGetSnapshot(T instance)
        {

            if (instance is ICloneable cloneable)
                return (T)cloneable.Clone();

            var result = (T)JsonConvert.DeserializeObject(Json(instance), typeof(T));
            return result;

        }

        private static int DoGetHashCode(T instance)
        {

            if (instance is IEquatable<T>)
                return instance.GetHashCode();

            return Json(instance).GetHashCode();

        }

        private static bool DoEquals(T left, T right)
        {

            if (left is IEquatable<T> equatable)
                return equatable.Equals(right);

            var result = Json(left).Equals(Json(right));
            return result;

        }

        public JsonValueComparer() : base(
          (t1, t2) => DoEquals(t1, t2),
          t => DoGetHashCode(t),
          t => DoGetSnapshot(t))
        {
        }

    }

    internal static class JsonHelper
    {
        public static T Deserialize<T>(string json) where T : class
        {
            return string.IsNullOrWhiteSpace(json) ? null : JsonConvert.DeserializeObject<T>(json);
        }

        public static string Serialize<T>(T obj) where T : class
        {
            return obj == null ? null : JsonConvert.SerializeObject(obj);
        }
    }
}
