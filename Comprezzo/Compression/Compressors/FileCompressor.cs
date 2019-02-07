using System.IO;
using System.IO.Compression;
using Sbb.Compression.FileOpeners;
using Sbb.Compression.Stream4ers.Pumps;

namespace Sbb.Compression.Compressors
{
    public class FileCompressor : IFileCompressor
    {
        private readonly IStream2StreamPump _pump;

        public FileCompressor(ICompressionFileOpener fileOpener, IStream2StreamPump pump)
        {
            _pump = pump;
            FileOpener = fileOpener;
        }

        public ICompressionFileOpener FileOpener { get; set; }

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
            _pump.Pump(source, target);
        }

        public void Dispose() => _pump.Dispose();
    }
}