namespace Sbb.Compression.Compressors
{
    public interface IFileCompressor
    {
        void Compress(string inputFilePath, string outputFilePath);

        void Decompress(string inputFilePath, string outputFilePath);
    }
}