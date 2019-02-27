using System;
using System.Collections.Generic;
using System.Reflection;

namespace AspNetCore.Base.Domain
{
    //https://docs.microsoft.com/en-us/ef/core/modeling/owned-entities
    //[Owned]
    public abstract class ValueObject<T> : IEquatable<T>
        where T : ValueObject<T>
    {
        public bool Equals(T other)
        {
            return Equals(other);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            Type t = GetType();
            Type otherType = obj.GetType();

            if (t != otherType)
                return false;

            FieldInfo[] fields = t.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

            foreach (FieldInfo field in fields)
            {
                object value1 = field.GetValue(obj);
                object value2 = field.GetValue(this);

                if (value1 == null)
                {
                    if (value2 != null)
                        return false;
                }
                else if (!value1.Equals(value2))
                    return false;
            }

            return true;
        }

        public override int GetHashCode()
        {
            IEnumerable<FieldInfo> fields = GetFields();

            int startValue = 17;
            int multiplier = 59;

            int hashCode = startValue;

            foreach (FieldInfo field in fields)
            {
                object value = field.GetValue(this);

                if (value != null)
                    hashCode = hashCode * multiplier + value.GetHashCode();
            }

            return hashCode;
        }

        private IEnumerable<FieldInfo> GetFields()
        {
            Type t = GetType();

            List<FieldInfo> fields = new List<FieldInfo>();

            while (t != typeof(object))
            {
                fields.AddRange(t.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public));

                t = t.BaseType;
            }

            return fields;
        }

        public static bool operator ==(ValueObject<T> a, ValueObject<T> b)
        {
            if (ReferenceEquals(a, null) && ReferenceEquals(b, null))
                return true;

            if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
                return false;

            return a.Equals(b);
        }

        public static bool operator !=(ValueObject<T> a, ValueObject<T> b)
        {
            return !(a == b);
        }

        public bool IsEmpty()
        {
            Type t = GetType();
            FieldInfo[] fields = t.GetFields
              (BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            foreach (FieldInfo field in fields)
            {
                object value = field.GetValue(this);
                if (value != null)
                {
                    return false;
                }
            }
            return true;
        }

        public ValueObject<T> GetCopy()
        {
            return MemberwiseClone() as ValueObject<T>;
        }

    }
}
