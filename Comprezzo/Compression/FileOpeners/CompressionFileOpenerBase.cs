using System;
using System.IO;
using System.IO.Compression;

namespace Sbb.Compression.FileOpeners
{
    /// <summary>
    /// Реализует базовую логику операций открытия и создания файлов для сжатия и разжатия.
    /// </summary>
    public abstract class CompressionFileOpenerBase : ICompressionFileOpener
    {
        // настройки по умолчанию создания файловых потоков
        // ------------------------------------------------

        public const int DEFAULT_BUFFER_SIZE = 1 * 1024 * 1024; // 1 МБ

        public const FileShare DEFAULT_SOURCE_FILE_SHARE = FileShare.Read;
        public const FileShare DEFAULT_TARGET_FILE_SHARE = FileShare.None;

        public const FileOptions DEFAULT_FILE_OPTIONS = FileOptions.SequentialScan;

        // ------------------------------------------------


        protected CompressionFileOpenerBase() : this(DEFAULT_BUFFER_SIZE) { }

        protected CompressionFileOpenerBase(int bufferSize = DEFAULT_BUFFER_SIZE,
            FileShare sourceFileShare = DEFAULT_SOURCE_FILE_SHARE,
            FileShare targetFileShare = DEFAULT_TARGET_FILE_SHARE,
            FileOptions sourceFileOptions = DEFAULT_FILE_OPTIONS,
            FileOptions targetFileOptions = DEFAULT_FILE_OPTIONS)
        {
            BufferSize = bufferSize;
            SourceFileShare = sourceFileShare;
            TargetFileShare = targetFileShare;
            SourceFileOptions = sourceFileOptions;
            TargetFileOptions = targetFileOptions;
        }


        // настройки создания файловых потоков
        // ------------------------------------------------

        public virtual int BufferSize { get; set; }

        public virtual FileShare SourceFileShare { get; set; }
        public virtual FileShare TargetFileShare { get; set; }

        public virtual FileOptions SourceFileOptions { get; set; }
        public virtual FileOptions TargetFileOptions { get; set; }

        // ------------------------------------------------


        /// <summary>
        /// Расширение, добавляемое к пути сжатого файла.
        /// </summary>
        public abstract string CompressionExtension { get; }

        /// <summary>
        /// Открывает исходный файл по заданному пути для сжатия или разжатия.
        /// </summary>
        public Stream OpenSource(string sourcePath, CompressionMode mode)
        {
            Stream source = OpenSource(sourcePath);
            return mode == CompressionMode.Compress ? source
                : CreateCompressionStream(source, mode);
        }

        /// <summary>
        /// Создаёт целевой файл по заданному пути для сжатия или разжатия.
        /// </summary>
        public Stream CreateTarget(string targetPath, CompressionMode mode)
        {
            CorrectCompressionPath(ref targetPath, mode);
            Stream target = CreateTarget(targetPath);
            return mode == CompressionMode.Compress
                ? CreateCompressionStream(target, mode) : target;
        }

        /// <summary>
        /// Дополняет нужным расширением путь к целевому файлу при операции сжатия.
        /// </summary>
        protected virtual void CorrectCompressionPath(ref string initialPath, CompressionMode mode)
        {
            if (mode == CompressionMode.Compress && !String.Equals(Path.GetExtension(initialPath),
                CompressionExtension, StringComparison.InvariantCultureIgnoreCase))
            {
                initialPath = initialPath + CompressionExtension;
            }
        }

        private Stream OpenSource(string sourcePath)
        {
            Func<string, FileStream> opener = path => new FileStream(path, FileMode.Open, FileAccess.Read,
                SourceFileShare, BufferSize, SourceFileOptions);
            return CreateFileStream(sourcePath, opener);
        }

        private Stream CreateTarget(string targetPath)
        {
            Func<string, FileStream> creator = path => new FileStream(path, FileMode.Create, FileAccess.Write,
                TargetFileShare, BufferSize, TargetFileOptions);
            return CreateFileStream(targetPath, creator);
        }

        private FileStream CreateFileStream(string path, Func<string, FileStream> creator)
        {
            try
            {
                return creator(path);
            }
            catch (ArgumentException exception)
            {
                throw new CompressionException($"Неправильно задан путь к файлу '{path}'"
                    + " либо другие настройки открытия файлового потока.", exception);
            }
            catch (IOException exception)
            {
                throw new CompressionException($"Не найден файл '{path}'"
                    + " либо длина пути к файлу превышает допустимый размер.", exception);
            }
            catch (UnauthorizedAccessException exception)
            {
                throw new CompressionException($"Ошибка доступа к файлу '{path}'.", exception);
            }
        }

        /// <summary>
        /// Определяет абстрактную операцию создания потока для сжатия или разжатия.
        /// </summary>
        protected abstract Stream CreateCompressionStream(Stream stream, CompressionMode mode);
    }
}