namespace ValueType.Extensions.IO
{
    using System.IO;

    public static class StreamExtensions
    {
        /// <summary>
        /// Copies a stream in to a new memory stream,
        /// resets the streams position,
        /// and then returns the copied bytes.
        /// </summary>
        /// <param name="stream">The source stream</param>
        /// <returns>The copied bytes</returns>
        public static byte[] ToByteArray(this Stream stream)
        {
            var resetPosition = stream.Position;
            using (var ms = new MemoryStream())
            {
                stream.Position = 0;
                stream.CopyTo(ms);
                stream.Position = resetPosition;
                return ms.ToArray();
            }
        }

        /// <summary>
        /// Copies a stream in to a new memory stream
        /// </summary>
        /// <param name="stream">The source stream</param>
        /// <param name="close">Whether to close the original stream</param>
        /// <returns>The copied stream</returns>
        public static MemoryStream ToMemoryStream(this Stream stream, bool close = false)
        {
            const int readSize = 256;
            var buffer = new byte[readSize];
            var memoryStream = new MemoryStream();

            var count = stream.Read(buffer, 0, readSize);

            while (count > 0)
            {
                memoryStream.Write(buffer, 0, count);
                count = stream.Read(buffer, 0, readSize);
            }

            memoryStream.Position = 0;

            if (close)
            {
                stream.Close();
            }

            return memoryStream;
        }
    }
}
