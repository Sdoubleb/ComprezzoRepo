using System.IO;
using System.IO.Compression;

namespace GZipper
{
    public abstract class FileCompressorBase : IFileCompressor
    {
        protected FileCompressorBase(ICompressionFileOpener fileOpener)
        {
            FileOpener = fileOpener;
        }

        public ICompressionFileOpener FileOpener { get; set; }

        public void Compress(string inputFilePath, string outputFilePath)
        {
            using (Stream source = FileOpener.OpenSource(inputFilePath, CompressionMode.Compress))
            using (Stream target = FileOpener.OpenTarget(inputFilePath, CompressionMode.Compress))
            {
                Compress(source, target);
            }
        }

        public void Decompress(string inputFilePath, string outputFilePath)
        {
            using (Stream source = FileOpener.OpenSource(inputFilePath, CompressionMode.Decompress))
            using (Stream target = FileOpener.OpenTarget(inputFilePath, CompressionMode.Decompress))
            {
                Decompress(source, target);
            }
        }

        protected abstract void Compress(Stream source, Stream target);

        protected abstract void Decompress(Stream source, Stream target);
    }
}