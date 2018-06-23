namespace ValueType.Extensions.Text
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;

    public static class StringExtensions
    {
        /// <summary>
        /// Adds spaces before capitals in a string.
        /// </summary>
        /// <param name="source">The source string</param>
        /// <returns>The transformed string</returns>
        public static string AddSpacesBeforeCapitals(this string source)
        {
            return Regex.Replace(source, @"(\B[A-Z]+?(?=[A-Z][^A-Z])|\B[A-Z]+?(?=[^A-Z]))", " $1");
        }
        
        /// <summary>
        /// Replaces all instances of the given kvp in a string.
        /// </summary>
        /// <param name="str">The source string</param>
        /// <param name="kvp">The find and replace pairs</param>
        /// <returns>The resultant string</returns>
        public static string FindAndReplace(this string str, IDictionary<string, string> kvp)
        {
            var stringBuilder = new StringBuilder(str);

            foreach (var item in kvp)
            {
                stringBuilder.Replace(item.Key, item.Value);
            }

            return stringBuilder.ToString();
        }

        /// <summary>
        /// Determines whether a string contains numerals
        /// </summary>
        /// <param name="input">The source string</param>
        /// <returns>The result</returns>
        public static bool HasNumeric(this string input)
        {
            return !string.IsNullOrWhiteSpace(input) && input.ToCharArray().Any(char.IsDigit);
        }

        /// <summary>
        /// Determines whether the contents of a string are numeric
        /// </summary>
        /// <param name="input">The source string</param>
        /// <returns>The result</returns>
        public static bool IsNumeric(this string input)
        {
            return !string.IsNullOrWhiteSpace(input) &&
                   input.ToCharArray().All(e => char.IsDigit(e) || e == '.' || e == '-');
        }

        /// <summary>
        /// Lowercase the first character of a string
        /// </summary>
        /// <param name="input">The source string</param>
        /// <returns>The transformed string</returns>
        public static string LowerCaseFirst(this string input)
        {
            return input.Any() ? char.ToLowerInvariant(input[0]) + input.Substring(1) : input;
        }

        /// <summary>
        /// Removes all invalid characters from a string,
        /// until only letters, numbers, and dashes remain.
        /// </summary>
        /// <param name="phrase">A string</param>
        /// <returns>A string than can be used for a url, file or directory name</returns>
        public static string Slugify(this string phrase)
        {
            string str = phrase.RemoveAccent().ToLower();
            str = Regex.Replace(str, @"[^a-z0-9\s-]", ""); // Remove all non valid chars          
            str = Regex.Replace(str, @"\s+", " ").Trim(); // convert multiple spaces into one space  
            str = Regex.Replace(str, @"\s", "-"); // //Replace spaces by dashes
            return str;
        }

        /// <summary>
        /// Split a camel case string into separate words
        /// </summary>
        /// <param name="source"></param>
        /// <returns>The split string</returns>
        public static string SplitCamelCase(this string source)
        {
            return Regex.Replace(source, "([A-Z])", " $1", RegexOptions.Compiled).Trim();
        }

        /// <summary>
        /// Takes a substring from the end of a string
        /// </summary>
        /// <param name="input">The source string</param>
        /// <param name="tailLength">The length of the substring</param>
        /// <returns>The substring</returns>
        public static string SubstringFromEnd(this string input, int tailLength)
        {
            return tailLength >= input.Length ? input : input.Substring(input.Length - tailLength);
        }

        /// <summary>
        /// Removes invalid file name characters from a string.
        /// </summary>
        /// <param name="str">A string</param>
        /// <returns>A string than can be used as a file or directory name</returns>
        public static string ToSafeFilename(this string str)
        {
            var invalidCharacters = Path.GetInvalidFileNameChars();

            return new string(str.Trim().Where(c => !invalidCharacters.Contains(c)).ToArray());
        }

        /// <summary>
        /// Converts a string to title case
        /// </summary>
        /// <param name="source">The source string</param>
        /// <returns>The converted string</returns>
        public static string ToTitleCase(this string source)
        {
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(source);
        }

        /// <summary>
        /// Trims all instances of the requested string from the start of a string
        /// </summary>
        /// <param name="source">The source string</param>
        /// <param name="trimmable">The string to strip from the start</param>
        /// <returns>The stripped string</returns>
        public static string TrimStart(this string source, string trimmable)
        {
            while (source.StartsWith(trimmable))
            {
                source = source.Substring(trimmable.Length);
            }

            return source;
        }

        /// <summary>
        /// Truncates a string by a specified number of characters, and includes an appended string if it exceeds a specified limits.
        /// NOTE: The amount of characters taken if the string exceeds the character limit is: (CharacterLimit - appendString.length).
        /// </summary>
        /// <param name="source">The source string</param>
        /// <param name="characterLimit">The amount of characters to accept before truncating</param>
        /// <param name="appendStringIfLimitExceeded">Some characters to append if limit hit</param>
        /// <returns></returns>
        public static string Truncate(this string source, int characterLimit, string appendStringIfLimitExceeded = "...")
        {
            if (source.Length <= characterLimit)
            {
                return source;
            }

            return (source.Substring(0, characterLimit - appendStringIfLimitExceeded.Length)).Trim() + appendStringIfLimitExceeded;
        }

        /// <summary>
        /// Attempts to case insensitive convert a string value to a boolean.
        /// Possible values; 1|Y|YES|TRUE or 0|N|NO|FALSE
        /// </summary>
        /// <param name="str">The input string</param>
        /// <param name="result">The result boolean</param>
        /// <returns>True if it was successful</returns>
        public static bool TryParseBoolean(this string str, out bool result)
        {
            var trueValues = new[]{ "1", "Y", "YES", "TRUE" };
            var falseValues = new[]{ "0", "N", "NO", "FALSE" };

            if (trueValues.Contains(str, StringComparer.CurrentCultureIgnoreCase))
            {
                return (result = true);
            }

            if (falseValues.Contains(str, StringComparer.CurrentCultureIgnoreCase))
            {
                result = false;
                return true;
            }

            return (result = false);
        }

        /// <summary>
        /// Removes foreign accents from a string.
        /// </summary>
        /// <param name="str">A string</param>
        /// <returns>The resultant string</returns>
        public static string RemoveAccent(this string str)
        {
            var bytes = Encoding.GetEncoding("Cyrillic").GetBytes(str);
            return Encoding.ASCII.GetString(bytes);
        }

        /// <summary>
        /// Removes non numeric values from a string
        /// </summary>
        /// <param name="source">The source string</param>
        /// <returns>The string stripped of everything except numerals, full stops and negative figures</returns>
        public static string RemoveNonNumeric(this string source)
        {
            return string.IsNullOrWhiteSpace(source)
                ? string.Empty
                : new string(source.ToCharArray().Where(e => char.IsDigit(e) || e == '.' || e == '-').ToArray());
        }

        /// <summary>
        /// Removes numeric values from a string
        /// </summary>
        /// <param name="source">The source string</param>
        /// <returns>The string stripped of numerals</returns>
        public static string RemoveNumeric(this string source)
        {
            return string.IsNullOrWhiteSpace(source)
                ? string.Empty
                : new string(source.ToCharArray().Where(e => !char.IsDigit(e)).ToArray());
        }

        /// <summary>
        /// Uppercase the first character of a string
        /// </summary>
        /// <param name="input">The source string</param>
        /// <returns>The transformed string</returns>
        public static string UpperCaseFirst(this string input)
        {
            return input.Any() ? char.ToUpperInvariant(input[0]) + input.Substring(1) : input;
        }
    }
}
