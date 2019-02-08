using System;
using System.IO;
using Sbb.Compression.Storages;

namespace Sbb.Compression.Stream4ers
{
    /// <summary>
    /// Интерфейс высокоуровневого поблочного писателя байтовых потоков.
    /// </summary>
    public interface IBlockyStreamWriter : IDisposable
    {
        /// <param name="blockLength">Длина единоразово записываемого байтового массива.</param>
        void Write(Stream stream, int blockLength, ISizeableStorage<long, NumberedByteBlock> storage);
    }
}