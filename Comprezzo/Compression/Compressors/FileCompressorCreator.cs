using System;
using System.IO;
using Sbb.Compression.Common;
using Sbb.Compression.FileOpeners;
using Sbb.Compression.Storages;
using Sbb.Compression.Stream4ers;
using Sbb.Compression.Stream4ers.Pumps;

namespace Sbb.Compression.Compressors
{
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
        public virtual int BlockLength { get; set; }

        public virtual int BufferSize { get; set; }

        protected virtual Func<byte[]> ByteCreator => () => new byte[BlockLength];

        public virtual IFileCompressor Create()
        {
            ICompressionFileOpener fileOpener = CreateFileOpener();
            IStream2StreamPump pump = CreatePump();
            IFileCompressor compressor = new FileCompressor(fileOpener, pump);
            return compressor;
        }

        protected virtual ICompressionFileOpener CreateFileOpener()
        {
            return new GZipCompressionFileOpener()
            {
                BufferSize = BufferSize,
                SourceFileOptions = CompressionFileOpenerBase.DEFAULT_FILE_OPTIONS
                    | FileOptions.Asynchronous
            };
        }

        protected virtual IStream2StreamPump CreatePump()
        {
            IBlockyStream4erFactory stream4erFactory = CreateStream4erFactory();
            return new BlockyStream2StreamPump(stream4erFactory);
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
            return new AsyncBlockyStreamReaderProvider();
        }

        protected virtual IStreamWriterProvider CreateWriterProvider()
        {
            return new BlockyStreamWriterProvider();
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