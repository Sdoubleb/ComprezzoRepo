namespace GZipper
{
    public interface IFileCompressor
    {
        void Compress(string inputFilePath, string outputFilePath);

        void Decompress(string inputFilePath, string outputFilePath);
    }
}