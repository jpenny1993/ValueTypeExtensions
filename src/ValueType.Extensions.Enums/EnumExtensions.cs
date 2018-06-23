namespace ValueType.Extensions.Enums
{
    using System;
    using ValueType.Extensions.Types;

    public static class EnumExtensions
    {
        /// <summary>
        /// Returns the display attribute on an enumeration value.
        /// </summary>
        /// <typeparam name="TEnum">The enumeration type</typeparam>
        /// <param name="enumValue">The enumeration value</param>
        /// <returns>The description, falling back to ToString()</returns>
        public static string ToEnumDisplayString<TEnum>(this TEnum enumValue) where TEnum : struct
        {
            return typeof(TEnum).ToEnumDisplayString(enumValue).Replace('_', ' ');
        }

        /// <summary>
        /// Returns the display attribute on an enumeration value.
        /// </summary>
        /// <param name="type">The enumeration type</param>
        /// <param name="enumValue">The enumeration value</param>
        /// <returns>The description, falling back to ToString()</returns>
        public static string ToEnumDisplayString(this Type type, object enumValue)
        {
            if (!type.IsEnum)
            {
                throw new NotSupportedException("An Enumeration type is required.");
            }

            var fieldInfo = enumValue.GetType().GetField(enumValue.ToString());

            return fieldInfo != null ? fieldInfo.GetDataAnnotationDisplayName() : enumValue.ToString();
        }
    }
}
