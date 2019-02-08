using System;
using System.IO;

namespace Sbb.Compression.Stream4ers.Pumps
{
    /// <summary>
    /// Интерфейс насоса, перекачивающего байты из одного потока в другой.
    /// </summary>
    public interface IStream2StreamPump : IDisposable
    {
        /// <summary>
        /// Перекачивает байты из одного заданного потока в другой.
        /// </summary>
        void Pump(Stream source, Stream target);
    }
}