using System.Diagnostics;

namespace Sbb.Compression
{
    static class Utils
    {
        public static long CalculateCountOfBlocks(long totalLength, long blockLength)
        {
            return totalLength / blockLength + (totalLength % blockLength == 0 ? 0 : 1);
        }

        public static float GetAvailableMemory()
        {
            using (var performance = new PerformanceCounter("Memory", "Available MBytes"))
            {
                float availableMBytes = performance.NextValue();
                return availableMBytes;
            }
        }
    }
}