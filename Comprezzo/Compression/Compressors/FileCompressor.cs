﻿using System;
using System.IO;
using System.IO.Compression;
using Sbb.Compression.FileOpeners;
using Sbb.Compression.Stream4ers.Pumps;

namespace Sbb.Compression.Compressors
{
    /// <summary>
    /// Файловый компрессор.
    /// </summary>
    public class FileCompressor : IFileCompressor
    {
        protected readonly IStream2StreamPump _pump;

        public FileCompressor(ICompressionFileOpener fileOpener, IStream2StreamPump pump)
        {
            _pump = pump;
            FileOpener = fileOpener;
        }

        public ICompressionFileOpener FileOpener { get; set; }

        /// <summary>
        /// Сжимает файл по заданному пути в выходной файл по заданному пути.
        /// </summary>
        public void Compress(string inputFilePath, string outputFilePath)
        {
            Work(inputFilePath, outputFilePath, CompressionMode.Compress);
        }

        /// <summary>
        /// Разжимает файл по заданному пути в выходной файл по заданному пути.
        /// </summary>
        public void Decompress(string inputFilePath, string outputFilePath)
        {
            throw new NotImplementedException("Разжатие будет реализовано в следующей версии программы.");
            Work(inputFilePath, outputFilePath, CompressionMode.Decompress);
        }

        // TODO: ходят слухи, что существуют тайные магические знания
        // по обработке OutOfMemoryException - попробовать применить их здесь
        private void Work(string inputFilePath, string outputFilePath, CompressionMode mode)
        {
            using (Stream source = FileOpener.OpenSource(inputFilePath, mode))
            using (Stream target = FileOpener.CreateTarget(outputFilePath, mode))
            {
                Work(source, target);
            }
        }

        protected virtual void Work(Stream source, Stream target)
        {
            _pump.Pump(source, target);
        }

        public void Dispose() => _pump.Dispose();
    }
}