using System.IO;
using System.IO.Compression;

namespace Sbb.Compression.FileOpeners
{
    public class GZipCompressionFileOpener : CompressionFileOpenerBase
    {
        public GZipCompressionFileOpener() : base() { }

        public GZipCompressionFileOpener(int bufferSize = DEFAULT_BUFFER_SIZE,
            FileShare sourceFileShare = DEFAULT_SOURCE_FILE_SHARE,
            FileShare targetFileShare = DEFAULT_TARGET_FILE_SHARE,
            FileOptions sourceFileOptions = DEFAULT_FILE_OPTIONS,
            FileOptions targetFileOptions = DEFAULT_FILE_OPTIONS)
            : base(bufferSize, sourceFileShare, targetFileShare, sourceFileOptions, targetFileOptions) { }

        public override string CompressionExtension => ".gz";

        protected override Stream CreateCompressionStream(Stream stream, CompressionMode mode)
            => new GZipStream(stream, mode);
    }
}