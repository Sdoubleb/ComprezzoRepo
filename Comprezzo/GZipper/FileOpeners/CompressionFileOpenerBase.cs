using System;
using System.IO;
using System.IO.Compression;

namespace Sbb.Compression.FileOpeners
{
    public abstract class CompressionFileOpenerBase : ICompressionFileOpener
    {
        public const int DEFAULT_BUFFER_SIZE = 1 * 1024 * 1024; // 1 МБ

        public const FileShare DEFAULT_SOURCE_FILE_SHARE = FileShare.Read;
        public const FileShare DEFAULT_TARGET_FILE_SHARE = FileShare.None;

        public const FileOptions DEFAULT_FILE_OPTIONS = default(FileOptions);

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

        public virtual int BufferSize { get; set; }

        public virtual FileShare SourceFileShare { get; set; }
        public virtual FileShare TargetFileShare { get; set; }

        public virtual FileOptions SourceFileOptions { get; set; }
        public virtual FileOptions TargetFileOptions { get; set; }

        public abstract string CompressionExtension { get; }

        public Stream OpenSource(string sourcePath, CompressionMode mode)
        {
            Stream source = OpenSource(sourcePath);
            return mode == CompressionMode.Compress ? source
                : CreateCompressionStream(source, mode);
        }

        public Stream OpenTarget(string targetPath, CompressionMode mode)
        {
            CorrectCompressionPath(ref targetPath, mode);
            Stream target = OpenTarget(targetPath);
            return mode == CompressionMode.Compress
                ? CreateCompressionStream(target, mode) : target;
        }

        private Stream OpenSource(string sourcePath)
        {
            return new FileStream(sourcePath, FileMode.Open, FileAccess.Read,
                SourceFileShare, BufferSize, SourceFileOptions);
        }

        private Stream OpenTarget(string targetPath)
        {
            return new FileStream(targetPath, FileMode.Create, FileAccess.Write,
                TargetFileShare, BufferSize, TargetFileOptions);
        }

        protected virtual void CorrectCompressionPath(ref string initialPath, CompressionMode mode)
        {
            if (mode == CompressionMode.Compress &&
                !String.Equals(Path.GetExtension(initialPath), CompressionExtension,
                StringComparison.InvariantCultureIgnoreCase))
            {
                initialPath = initialPath + CompressionExtension;
            }
        }

        protected abstract Stream CreateCompressionStream(Stream stream, CompressionMode mode);
    }
}