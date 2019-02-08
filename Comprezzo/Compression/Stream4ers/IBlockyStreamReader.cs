using System;
using System.IO;
using Sbb.Compression.Storages;

namespace Sbb.Compression.Stream4ers
{
    /// <summary>
    /// Интерфейс высокоуровневого поблочного читателя байтовых потоков.
    /// </summary>
    public interface IBlockyStreamReader : IDisposable
    {
        /// <param name="blockLength">Длина единоразово считываемого байтового массива.</param>
        ISizeableStorage<long, NumberedByteBlock> Read(Stream stream, int blockLength);
    }
}