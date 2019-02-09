using System;
using System.IO;
using Sbb.Compression.Common;
using Sbb.Compression.FileOpeners;
using Sbb.Compression.Storages;
using Sbb.Compression.Stream4ers;
using Sbb.Compression.Stream4ers.Direct;
using Sbb.Compression.Stream4ers.Pumps;

namespace Sbb.Compression.Compressors
{
    /// <summary>
    /// Создатель конкретной реализации файлового компрессора.
    /// </summary>
    public class FileCompressorCreator : ICreator<IFileCompressor>
    {
        private const int MB = 1 * 1024 * 1024; // 1 МБ

        public FileCompressorCreator() : this(MB) { }

        public FileCompressorCreator(int blockLength = MB, int bufferSize = MB)
        {
            BlockLength = blockLength;
            BufferSize = bufferSize;
        }

        // TODO: попробовать определять длину блока в зависимости от длины файла
        /// <summary>
        /// Длина блока байтов, считывающихся из файла за одну операцию.
        /// </summary>
        public virtual int BlockLength { get; set; }

        /// <summary>
        /// Размер буфера файлового потока записи.
        /// </summary>
        public virtual int BufferSize { get; set; }

        protected virtual Func<byte[]> ByteCreator => () => new byte[BlockLength];

        /// <summary>
        /// Создаёт конкретную реализацию файлового компрессора.
        /// </summary>
        /// <exception cref="MemoryLacksException">
        /// Длина блока, выбранная для чтения файла, превышает объём доступной памяти.
        /// </exception>
        public virtual IFileCompressor Create()
        {
            try
            {
                ICompressionFileOpener fileOpener = CreateFileOpener();
                IStream2StreamPump pump = CreatePump();
                IFileCompressor compressor = new FileCompressor(fileOpener, pump);
                return compressor;
            }
            catch (MemoryLacksException exception)
            {
                // TODO: пробовать уменьшать длину блока
                throw new MemoryLacksException("Длина блока, выбранная для чтения файла,"
                    + " превышает объём доступной памяти.", exception);
            }
        }

        protected virtual ICompressionFileOpener CreateFileOpener()
        {
            return new GZipCompressionFileOpener(BufferSize)
            {
                SourceFileOptions = CompressionFileOpenerBase.DEFAULT_FILE_OPTIONS
                    | FileOptions.Asynchronous
            };
        }

        protected virtual IStream2StreamPump CreatePump()
        {
            IBlockyStream4erFactory stream4erFactory = CreateStream4erFactory();
            return new BlockyStream2StreamPump(stream4erFactory, BlockLength);
        }

        protected virtual IBlockyStream4erFactory CreateStream4erFactory()
        {
            IStreamReaderProvider readerProvider = CreateReaderProvider();
            IStreamWriterProvider writerProvider = CreateWriterProvider();
            IWaitableObjectPoolProvider<byte[]> poolProvider = CreatePoolProvider();
            ISizeableStorageProvider<long, NumberedByteBlock> storageProvider = CreateStorageProvider();
            INumericStorageEnumerableProvider<NumberedByteBlock> storageEnumerableProvider
                = CreateStorageEnumerableProvider();
            return new ThriftyBlockyStream4erFactory(readerProvider, writerProvider,
                storageProvider, storageEnumerableProvider, poolProvider);
        }

        protected virtual IStreamReaderProvider CreateReaderProvider()
        {
            return new AsyncMlthrdStreamReaderProvider();
        }

        protected virtual IStreamWriterProvider CreateWriterProvider()
        {
            return new SnglthrdStreamWriterProvider();
        }

        protected virtual ISizeableStorageProvider<long, NumberedByteBlock> CreateStorageProvider()
        {
            return new AvoidingLockConcurrentStorageProvider<NumberedByteBlock>();
        }

        protected virtual INumericStorageEnumerableProvider<NumberedByteBlock> CreateStorageEnumerableProvider()
        {
            return new NumericStorageEnumerableProvider<NumberedByteBlock>();
        }

        protected virtual IWaitableObjectPoolProvider<byte[]> CreatePoolProvider()
        {
            return new SizeDefiningObjectPoolProvider<byte[]>(BlockLength, ByteCreator);
        }
    }
}