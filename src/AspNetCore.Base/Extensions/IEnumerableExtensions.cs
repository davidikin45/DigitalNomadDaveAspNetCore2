using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Reflection;

namespace AspNetCore.Base.Extensions
{
    public static class IEnumerableExtensions
    {
        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> collection, Action<T> action)
        {
            foreach (T item in collection)
            {
                action(item);
            }
            return collection;
        }

        public static List<T> MergeLists<T>(
          this IEnumerable<List<T>> source)
        {
            var newList = new List<T>();
            foreach (var list in source)
            {
                foreach (var item in list)
                {
                    newList.Add(item);
                }
            }
            return newList;
        }

        public static IEnumerable<ExpandoObject> ShapeData<TSource>(
            this IEnumerable<TSource> source,
            string fields)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            // create a list to hold our ExpandoObjects
            var expandoObjectList = new List<ExpandoObject>();

            // create a list with PropertyInfo objects on TSource.  Reflection is
            // expensive, so rather than doing it for each object in the list, we do 
            // it once and reuse the results.  After all, part of the reflection is on the 
            // type of the object (TSource), not on the instance
            var propertyInfoList = new List<PropertyInfo>();

            if (string.IsNullOrWhiteSpace(fields))
            {
                // all public properties should be in the ExpandoObject
                var propertyInfos = typeof(TSource)
                        .GetProperties(BindingFlags.Public | BindingFlags.Instance);

                propertyInfoList.AddRange(propertyInfos);
            }
            else
            {
                // only the public properties that match the fields should be
                // in the ExpandoObject

                // the field are separated by ",", so we split it.
                var fieldsAfterSplit = fields.Split(',');

                foreach (var field in fieldsAfterSplit)
                {
                    // trim each field, as it might contain leading 
                    // or trailing spaces. Can't trim the var in foreach,
                    // so use another var.
                    var propertyName = field.Trim();

                    // use reflection to get the property on the source object
                    // we need to include public and instance, b/c specifying a binding flag overwrites the
                    // already-existing binding flags.
                    var propertyInfo = typeof(TSource)
                        .GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                    if (propertyInfo == null)
                    {
                        throw new Exception($"Property {propertyName} wasn't found on {typeof(TSource)}");
                    }

                    // add propertyInfo to list 
                    propertyInfoList.Add(propertyInfo);
                }
            }

            // run through the source objects
            foreach (TSource sourceObject in source)
            {
                // create an ExpandoObject that will hold the 
                // selected properties & values
                var dataShapedObject = new ExpandoObject();

                // Get the value of each property we have to return.  For that,
                // we run through the list
                foreach (var propertyInfo in propertyInfoList)
                {
                    // GetValue returns the value of the property on the source object
                    var propertyValue = propertyInfo.GetValue(sourceObject);

                    // add the field to the ExpandoObject
                    ((IDictionary<string, object>)dataShapedObject).Add(propertyInfo.Name, propertyValue);
                }

                // add the ExpandoObject to the list
                expandoObjectList.Add(dataShapedObject);
            }

            // return the list

            return expandoObjectList;
        }

        public static IEnumerable<ExpandoObject> ShapeData(
         this IEnumerable<Object> source,
         Type type,
         string fields)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            // create a list to hold our ExpandoObjects
            var expandoObjectList = new List<ExpandoObject>();

            // create a list with PropertyInfo objects on TSource.  Reflection is
            // expensive, so rather than doing it for each object in the list, we do 
            // it once and reuse the results.  After all, part of the reflection is on the 
            // type of the object (TSource), not on the instance
            var propertyInfoList = new List<PropertyInfo>();

            if (string.IsNullOrWhiteSpace(fields))
            {
                // all public properties should be in the ExpandoObject
                var propertyInfos = type
                        .GetProperties(BindingFlags.Public | BindingFlags.Instance);

                propertyInfoList.AddRange(propertyInfos);
            }
            else
            {
                // only the public properties that match the fields should be
                // in the ExpandoObject

                // the field are separated by ",", so we split it.
                var fieldsAfterSplit = fields.Split(',');

                foreach (var field in fieldsAfterSplit)
                {
                    // trim each field, as it might contain leading 
                    // or trailing spaces. Can't trim the var in foreach,
                    // so use another var.
                    var propertyName = field.Trim();

                    // use reflection to get the property on the source object
                    // we need to include public and instance, b/c specifying a binding flag overwrites the
                    // already-existing binding flags.
                    var propertyInfo = type
                        .GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                    if (propertyInfo == null)
                    {
                        throw new Exception($"Property {propertyName} wasn't found on {type}");
                    }

                    // add propertyInfo to list 
                    propertyInfoList.Add(propertyInfo);
                }
            }

            // run through the source objects
            foreach (Object sourceObject in source)
            {
                if (sourceObject != null)
                {
                    // create an ExpandoObject that will hold the 
                    // selected properties & values
                    var dataShapedObject = new ExpandoObject();

                    // Get the value of each property we have to return.  For that,
                    // we run through the list
                    foreach (var propertyInfo in propertyInfoList)
                    {
                        // GetValue returns the value of the property on the source object
                        var propertyValue = propertyInfo.GetValue(sourceObject);

                        // add the field to the ExpandoObject
                        ((IDictionary<string, object>)dataShapedObject).Add(propertyInfo.Name, propertyValue);
                    }

                    // add the ExpandoObject to the list
                    expandoObjectList.Add(dataShapedObject);
                }
            }

            // return the list

            return expandoObjectList;
        }
    }
}
