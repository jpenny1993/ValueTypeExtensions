namespace ValueType.Extensions.Types
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Serialization;

    public static class TypeExtensions
    {
        /// <summary>
        /// Return underlying type if type is nullable otherwise return the type.
        /// </summary>
        public static Type GetUnderlyingTypeIfNullable(this Type t)
        {
            if (t != null && t.IsNullable())
            {
                return !t.IsValueType ? t : Nullable.GetUnderlyingType(t);
            }

            return t;
        }

        /// <summary>
        /// Gets public properties of an object type.
        /// </summary>
        /// <typeparam name="TEntity">Object type</typeparam>
        /// <param name="obj"></param>
        /// <param name="ignoreProperties">Properties to ignore</param>
        /// <returns>An array of public property info</returns>
        public static IEnumerable<PropertyInfo> GetPublicProperties<TEntity>(this TEntity obj, params string[] ignoreProperties)
        {
            return typeof(TEntity).GetPublicProperties(ignoreProperties);
        }

        /// <summary>
        /// Gets public properties of an object type.
        /// </summary>
        /// <param name="classType">Object type</param>
        /// <param name="ignoreProperties">Properties to ignore</param>
        /// <returns>An array of public property info</returns>
        public static IEnumerable<PropertyInfo> GetPublicProperties(this Type classType, params string[] ignoreProperties)
        {
            return classType
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Select(prop => new
                {
                    Property = prop,
                    Type = prop.PropertyType,
                    UnderlyingType = prop.PropertyType.GetUnderlyingTypeIfNullable()
                })
                .Where(item =>
                {
                    if (ignoreProperties.Contains(item.Property.Name))
                    {
                        return false;
                    }

                    var ignoreDataMemberAttribute = item.Property.GetCustomAttributes<IgnoreDataMemberAttribute>(false);

                    if (ignoreDataMemberAttribute.Any())
                    {
                        return false;
                    }

                    // Ignore any properties which are not value types or strings
                    if (!item.UnderlyingType.IsValueType && item.UnderlyingType != typeof(string))
                    {
                        return false;
                    }

                    return true;
                })
                .Select(a => a.Property)
                .ToArray();
        }

        /// <summary>
        /// Determine of specified type is nullable.
        /// </summary>
        public static bool IsNullable(this Type t)
        {
            return !t.IsValueType || (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>));
        }
    }
}
