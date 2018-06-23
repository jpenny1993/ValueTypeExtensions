namespace ValueType.Extensions.Numbers
{
    public static class IntegerExtensions
    {
        /// <summary>
        /// Returns a suffix depending on the input number (i.e. nd, rd, th)
        /// </summary>
        /// <param name="i">A number</param>
        /// <returns>The suffix string</returns>
        public static string WithTwoLetterSuffix(this int i)
        {
            var iMod10 = i % 10;
            var suffix = (iMod10 == 1 && i != 11) ? "st"
            : (iMod10 == 2 && i != 12) ? "nd"
            : (iMod10 == 3 && i != 13) ? "rd"
            : "th";
            return $"{i}{suffix}";
        }
    }
}
