namespace ValueType.Extensions.Dates
{
    using System;

    public static class DateTimeExtensions
    {
        private static readonly DateTime JsDateEpoch = new DateTime(1970, 1, 1);
        private static readonly DateTime OaDateEpoch = new DateTime(1899, 12, 30);

        /// <summary>
        /// Converts an OADate to a DateTime. (used by OpenXml)
        /// </summary>
        /// <param name="d">Ticks since 30th December 1899</param>
        /// <returns>A DateTime</returns>
        public static DateTime FromOADatePrecise(this double d)
        {
            if (!(d >= 0))
            {
                throw new ArgumentOutOfRangeException("NaN and negatives not supported.");
            }

            return OaDateEpoch + TimeSpan.FromTicks(Convert.ToInt64(d * TimeSpan.TicksPerDay));
        }

        /// <summary>
        /// Converts a DateTime to an OADate. (used by OpenXml)
        /// </summary>
        /// <param name="dt">A DateTime</param>
        /// <returns>An OADate (Ticks since 30th December 1899)</returns>
        public static double ToOADatePrecise(this DateTime dt)
        {
            if (dt < OaDateEpoch)
                throw new ArgumentOutOfRangeException();

            return Convert.ToDouble((dt - OaDateEpoch).Ticks) / TimeSpan.TicksPerDay;
        }
               
        /// <summary>
        /// Converts an Ticks since JS Epoch to a DateTime.
        /// </summary>
        /// <param name="d">Ticks since 1st January 1970</param>
        /// <returns>A DateTime</returns>
        public static DateTime FromJavascriptTicks(this double d)
        {
            if (!(d >= 0))
            {
                throw new ArgumentOutOfRangeException("NaN and negatives not supported.");
            }

            return JsDateEpoch + TimeSpan.FromTicks(Convert.ToInt64(d * TimeSpan.TicksPerDay));
        }

        /// <summary>
        /// Converts a DateTime to Milliseconds since JS Epoch.
        /// </summary>
        /// <param name="dt">A DateTime</param>
        /// <returns>Ticks since 1st January 1970</returns>
        public static double ToJavascriptTicks(this DateTime dt)
        {
            return dt.Subtract(JsDateEpoch).TotalMilliseconds;
        }

        /// <summary>
        /// Returns a suffix depending on the input date (i.e. nd, rd, th)
        /// </summary>
        /// <param name="dt">A DateTime</param>
        /// <returns>The suffix string</returns>
        public static string TwoLetterSuffix(this DateTime dt)
        {
            var dayMod10 = dt.Day % 10;
            return (dayMod10 == 1 && dt.Day != 11) ? "st"
            : (dayMod10 == 2 && dt.Day != 12) ? "nd"
            : (dayMod10 == 3 && dt.Day != 13) ? "rd"
            : "th";
        }
    }
}