namespace ValueType.Extensions.Types
{
    using System;
    using System.Collections;
    using System.Reflection;

    public static class PropertyExtensions
    {
        /// <summary>
        /// Determines whether a property is collection. 
        /// </summary>
        /// <param name="property">Property Info</param>
        /// <returns>true if collection</returns>
        public static bool IsCollection(this PropertyInfo property)
        {
            return typeof(IEnumerable).IsAssignableFrom(property.PropertyType);
        }

        /// <summary>
        /// Determines whether a type is an enum.
        /// </summary>
        /// <param name="t">Property Type</param>
        /// <returns>true if enum</returns>
        public static bool IsEnum(this Type t)
        {
            return t.IsEnum || (t.IsNullable() && !IsString(t) && Nullable.GetUnderlyingType(t).IsEnum);
        }

        /// <summary>
        /// Determines whether a type is a number.
        /// </summary>
        /// <param name="t">Property Type</param>
        /// <returns>true if numeric</returns>
        public static bool IsNumber(this Type t)
        {
            var underlyingType = t.GetUnderlyingTypeIfNullable();
            switch (Type.GetTypeCode(t))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Determines whether a type is a string.
        /// </summary>
        /// <param name="t">Property Type</param>
        /// <returns>true if string</returns>
        public static bool IsString(this Type t)
        {
            return string.Equals(t.Name, nameof(String), StringComparison.CurrentCultureIgnoreCase);
        }
    }
}
