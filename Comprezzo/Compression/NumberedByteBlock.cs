namespace Sbb.Compression
{
    /// <summary>
    /// Нумерованный массив байтов.
    /// </summary>
    public class NumberedByteBlock
    {
        public NumberedByteBlock(long number, byte[] bytes)
        {
            Number = number;
            Bytes = bytes;
        }

        public long Number { get; }

        public byte[] Bytes { get; }

        /// <summary>
        /// Количество отсчитываемых от нуля байтов в массиве <see cref="Bytes"/>,
        /// которые имеют значение для программной логики.
        /// </summary>
        /// <remarks>
        /// Фактическая длина массива <see cref="Bytes"/> может быть больше этого значения.
        /// </remarks>
        public int Length { get; set; }
    }
}