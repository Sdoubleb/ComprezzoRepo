using System;

namespace Sbb.Compression
{
    public class MemoryLacksException : CompressionException
    {
        public MemoryLacksException() { }

        public MemoryLacksException(string message) : base(message) { }

        public MemoryLacksException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}