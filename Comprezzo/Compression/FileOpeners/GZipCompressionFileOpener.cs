using System.IO;
using System.IO.Compression;

namespace Sbb.Compression.FileOpeners
{
    /// <summary>
    /// Реализует логику операций открытия и создания файлов
    /// для сжатия и разжатия потоком <see cref="GZipStream"/>.
    /// </summary>
    public class GZipCompressionFileOpener : CompressionFileOpenerBase
    {
        public GZipCompressionFileOpener() : base() { }

        public GZipCompressionFileOpener(int bufferSize = DEFAULT_BUFFER_SIZE,
            FileShare sourceFileShare = DEFAULT_SOURCE_FILE_SHARE,
            FileShare targetFileShare = DEFAULT_TARGET_FILE_SHARE,
            FileOptions sourceFileOptions = DEFAULT_FILE_OPTIONS,
            FileOptions targetFileOptions = DEFAULT_FILE_OPTIONS)
            : base(bufferSize, sourceFileShare, targetFileShare, sourceFileOptions, targetFileOptions) { }

        /// <summary>
        /// Расширение, добавляемое к пути сжатого файла.
        /// </summary>
        public override string CompressionExtension => ".gz";

        /// <summary>
        /// Определяет операцию создания потока <see cref="GZipStream"/> для сжатия или разжатия.
        /// </summary>
        protected override Stream CreateCompressionStream(Stream stream, CompressionMode mode)
            => new GZipStream(stream, mode);
    }
}