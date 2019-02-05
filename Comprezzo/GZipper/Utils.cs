using System.IO;

namespace GZipper
{
    static class Utils
    {
        public static long CalculateCountOfStreamBlocks(Stream stream, int blockLength)
        {
            return stream.Length / blockLength
                + (stream.Length % blockLength == 0 ? 0 : 1);
        }
    }
}