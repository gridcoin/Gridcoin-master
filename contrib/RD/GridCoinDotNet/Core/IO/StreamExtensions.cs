using System.IO;

namespace BitCoinSharp.IO
{
    internal static class StreamExtensions
    {
        public static int Read(this Stream stream)
        {
            var buffer = new byte[1];
            return stream.Read(buffer) == 1 ? buffer[0] : -1;
        }

        public static int Read(this Stream stream, byte[] buffer)
        {
            return stream.Read(buffer, 0, buffer.Length);
        }

        public static void Write(this Stream stream, byte data)
        {
            stream.Write(new[] {data}, 0, 1);
        }

        public static void Write(this Stream stream, byte[] buffer)
        {
            stream.Write(buffer, 0, buffer.Length);
        }
    }
}