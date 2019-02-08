using System;
using System.IO;
using Sbb.Compression.Storages;

namespace Sbb.Compression.Stream4ers
{
    /// <summary>
    /// ��������� ���������������� ���������� �������� �������� �������.
    /// </summary>
    public interface IBlockyStreamWriter : IDisposable
    {
        /// <param name="blockLength">����� ����������� ������������� ��������� �������.</param>
        void Write(Stream stream, int blockLength, ISizeableStorage<long, NumberedByteBlock> storage);
    }
}