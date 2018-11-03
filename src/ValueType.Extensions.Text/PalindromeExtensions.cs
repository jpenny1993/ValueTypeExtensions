namespace ValueType.Extensions.Text
{
    public static class PalindromeExtensions
    {
        /// <summary>
        /// Checks if string is the same both forwards & reverse.
        /// </summary>
        /// <param name="input">The source string</param>
        /// <returns>True if palindrome</returns>
        public static bool IsPalindrome(this string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return false;
            }

            var range = input.Length / 2;
            var lastIndex = input.Length - 1;
            for (var index = 0; index < range; index++)
            {
                if (input[index] != input[lastIndex - index])
                {
                    return false;
                }
            }

            return true;
        }
    }
}
