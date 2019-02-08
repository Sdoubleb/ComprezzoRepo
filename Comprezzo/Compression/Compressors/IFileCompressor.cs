using System;

namespace Sbb.Compression.Compressors
{
    /// <summary>
    /// Интерфейс файлового компрессора.
    /// </summary>
    public interface IFileCompressor : IDisposable
    {
        /// <summary>
        /// Сжимает файл по заданному пути в выходной файл по заданному пути.
        /// </summary>
        void Compress(string inputFilePath, string outputFilePath);

        /// <summary>
        /// Разжимает файл по заданному пути в выходной файл по заданному пути.
        /// </summary>
        void Decompress(string inputFilePath, string outputFilePath);
    }
}