namespace ValueType.Extensions.Text
{
    using System;
    using System.Linq;

    public static class AnagramExtensions
    {
        /// <summary>
        /// Compares two strings to see if one is an anagram of the other.
        /// </summary>
        /// <param name="strA">input string</param>
        /// <param name="strB">comparison string</param>
        /// <param name="ignoreCase">ignore case</param>
        /// <returns>True if is an anagram</returns>
        public static bool IsAnagramOf(this string strA, string strB, bool ignoreCase = false)
        {
            if (string.IsNullOrEmpty(strA) ||
                string.IsNullOrEmpty(strB) ||
                strA.Length != strB.Length)
            {
                return false;
            }

            Func<string, string> orderString = (string str) => 
            {
                var casedStr = ignoreCase ? str.ToUpperInvariant(): str;
                var orderedChars = casedStr.OrderBy(c => c);
                return string.Join(string.Empty, orderedChars);
            }; 

            var orderedStrA = orderString(strA);
            var orderedStrB = orderString(strB);

            return string.Equals(orderedStrA, orderString(strB));
        }
    }
}
