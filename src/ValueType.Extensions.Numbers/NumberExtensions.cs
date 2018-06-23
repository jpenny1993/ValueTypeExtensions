namespace ValueType.Extensions.Numbers
{
    using System;
    using System.ComponentModel;

    public static class NumberExtensions
    {
        public static object GetRoundedValue(this object amount, Type type, int decimalPlaces)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Decimal:
                    return Math.Round(ChangeType<decimal>(amount), decimalPlaces);
                case TypeCode.Double:
                    return Math.Round(ChangeType<double>(amount), decimalPlaces);
                case TypeCode.Single:
                    return Math.Round(ChangeType<float>(amount), decimalPlaces);
                default:
                    return amount;
            }
        }

        public static TValue GetRoundedValue<TValue>(this TValue amount, int decimalPlaces) where TValue : IComparable<TValue>
        {
            var obj = GetRoundedValue(amount, typeof(TValue), decimalPlaces);
            return ChangeType<TValue>(obj);
        }

        private static T ChangeType<T>(this object value)
        {
            return (T)ChangeType(value, typeof(T));
        }

        private static object ChangeType(this object value, Type t)
        {
            TypeConverter tc = TypeDescriptor.GetConverter(t);
            return value.GetType() == t ? value : tc.ConvertFrom(value);
        }
    }
}
