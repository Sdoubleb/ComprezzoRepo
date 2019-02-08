using System.IO;
using System.IO.Compression;

namespace Sbb.Compression.FileOpeners
{
    /// <summary>
    /// Интерфейс, определяющий операции открытия и создания файлов для сжатия и разжатия.
    /// </summary>
    public interface ICompressionFileOpener
    {
        /// <summary>
        /// Открывает исходный файл по заданному пути для сжатия или разжатия.
        /// </summary>
        Stream OpenSource(string sourcePath, CompressionMode mode);

        /// <summary>
        /// Создаёт целевой файл по заданному пути для сжатия или разжатия.
        /// </summary>
        Stream CreateTarget(string targetPath, CompressionMode mode);
    }
}