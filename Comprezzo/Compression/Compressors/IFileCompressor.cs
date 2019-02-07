using System;

namespace Sbb.Compression.Compressors
{
    public interface IFileCompressor : IDisposable
    {
        void Compress(string inputFilePath, string outputFilePath);

        void Decompress(string inputFilePath, string outputFilePath);
    }
}