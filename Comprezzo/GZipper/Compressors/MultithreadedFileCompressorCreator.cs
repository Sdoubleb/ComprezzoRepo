using System;
using System.IO;
using Sbb.Compression.Common;
using Sbb.Compression.FileOpeners;
using Sbb.Compression.Storages;
using Sbb.Compression.Stream4ers;
using Sbb.Compression.Stream4ers.Pumps;

namespace Sbb.Compression.Compressors
{
    public class MultithreadedFileCompressorCreator : ICreator<IFileCompressor>
    {
        public virtual int BlockLength { get; set; } = 1 * 1024 * 1024; // 1 МБ

        protected virtual Func<byte[]> ByteCreator => () => new byte[BlockLength];

        public IFileCompressor Create()
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
                SourceFileOptions = CompressionFileOpenerBase.DEFAULT_FILE_OPTIONS
                    | FileOptions.Asynchronous
            };
        }

        protected virtual IStream2StreamPump CreatePump()
        {
            IOrderlyStream4erFactory stream4erFactory = CreateStream4erFactory();
            return new BlockyStream2StreamPump(stream4erFactory);
        }

        protected virtual IOrderlyStream4erFactory CreateStream4erFactory()
        {
            IBlockyStreamReaderProvider readerProvider = CreateReaderProvider();
            IBlockyStreamWriterProvider writerProvider = CreateWriterProvider();
            IWaitableObjectPoolProvider<byte[]> poolProvider = CreatePoolProvider();
            ISizeableStorageProvider<long, NumberedByteBlock> storageProvider = CreateStorageProvider();
            INumericStorageEnumerableProvider<NumberedByteBlock> storageEnumerableProvider
                = CreateStorageEnumerableProvider();
            return new ThriftyOrderlyStream4erFactory(readerProvider, writerProvider,
                poolProvider, storageProvider, storageEnumerableProvider);
        }

        protected virtual IBlockyStreamReaderProvider CreateReaderProvider()
        {
            return new AsyncBlockyStreamReaderProvider();
        }

        protected virtual IBlockyStreamWriterProvider CreateWriterProvider()
        {
            return new BlockyStreamWriterProvider();
        }

        protected virtual IWaitableObjectPoolProvider<byte[]> CreatePoolProvider()
        {
            return new SizeDefiningObjectPoolProvider<byte[]>(BlockLength, ByteCreator);
        }

        protected virtual ISizeableStorageProvider<long, NumberedByteBlock> CreateStorageProvider()
        {
            return new AvoidingLockConcurrentStorageProvider<NumberedByteBlock>();
        }

        protected virtual INumericStorageEnumerableProvider<NumberedByteBlock> CreateStorageEnumerableProvider()
        {
            return new NumericStorageEnumerableProvider<NumberedByteBlock>();
        }
    }
}