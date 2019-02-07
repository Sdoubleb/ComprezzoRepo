using System.IO;
using System.IO.Compression;
using Sbb.Compression.FileOpeners;
using Sbb.Compression.Stream4ers.Pumps;

namespace Sbb.Compression.Compressors
{
    public class FileCompressor : IFileCompressor
    {
        public FileCompressor(ICompressionFileOpener fileOpener, IStream2StreamPump pump)
        {
            FileOpener = fileOpener;
            Pump = pump;
        }

        public ICompressionFileOpener FileOpener { get; set; }

        private IStream2StreamPump Pump { get; set; }

        public void Compress(string inputFilePath, string outputFilePath)
        {
            Work(inputFilePath, outputFilePath, CompressionMode.Compress);
        }

        public void Decompress(string inputFilePath, string outputFilePath)
        {
            Work(inputFilePath, outputFilePath, CompressionMode.Decompress);
        }

        private void Work(string inputFilePath, string outputFilePath, CompressionMode mode)
        {
            using (Stream source = FileOpener.OpenSource(inputFilePath, mode))
            using (Stream target = FileOpener.OpenTarget(outputFilePath, mode))
            {
                Work(source, target);
            }
        }

        protected virtual void Work(Stream source, Stream target)
        {
            Pump.Pump(source, target);
        }
    }
}