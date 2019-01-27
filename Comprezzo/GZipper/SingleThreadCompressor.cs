using System.IO;
using System.IO.Compression;

namespace GZipper
{
    public class SingleThreadCompressor : ICompressor
    {
        private string _inputFileName;
        private string _outputFileName;

        public SingleThreadCompressor(string inputFileName, string outputFileName)
        {
            _inputFileName = inputFileName;
            _outputFileName = outputFileName;
        }

        public void Compress()
        {
            using (FileStream source = new FileStream(_inputFileName, FileMode.Open, FileAccess.Read))
            using (FileStream target = new FileStream($"{_outputFileName}.gz", FileMode.Create, FileAccess.Write))
            using (GZipStream compression = new GZipStream(target, CompressionMode.Compress))
            {
                byte[] bytes = new byte[source.Length];
                source.Read(bytes, 0, bytes.Length);
                compression.Write(bytes, 0, bytes.Length);
            }
        }

        public void Decompress()
        {
            using (FileStream source = new FileStream(_inputFileName, FileMode.Open, FileAccess.Read))
            using (FileStream target = new FileStream(_outputFileName, FileMode.Create, FileAccess.Write))
            using (GZipStream decompression = new GZipStream(source, CompressionMode.Decompress))
            {
                checked
                {
                    int byteOrEndOfStream;
                    while ((byteOrEndOfStream = decompression.ReadByte()) >= 0)
                        target.WriteByte((byte)byteOrEndOfStream);
                }
            }
        }
    }
}