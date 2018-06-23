namespace ValueType.Extensions.Collections
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    public static class ByteExtensions
    {
        /// <summary>
        /// Creates a MemoryStream from a byte array.
        /// </summary>
        /// <param name="bytes">Enumerable of bytes</param>
        /// <returns>The MemoryStream</returns>
        public static MemoryStream ToMemoryStream(this IEnumerable<byte> bytes)
        {
            return bytes != null 
                ? new MemoryStream(bytes.ToArray())
                : new MemoryStream();
        }
    }
}
