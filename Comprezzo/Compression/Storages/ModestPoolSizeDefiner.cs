using System;
using System.Diagnostics;

namespace Sbb.Compression.Storages
{
    /// <summary>
    /// Весьма скромный и совсем не притязательный определитель размера объектного пула,
    /// которому многого не надо.
    /// </summary>
    public class ModestPoolSizeDefiner : IPoolSizeDefiner
    {
        // доля доступной памяти о умолчанию, которую можно использовать под пул
        private const float DEFAULT_MEMORY_SHARE_TO_USE = 1 / 4F;

        // объём памяти по умолчанию, которую можно использовать под пул
        private const float DEFAULT_MEMORY_SIZE = 256;

        /// <summary>
        /// Доля доступной памяти, которую можно использовать под пул.
        /// </summary>
        protected virtual float MemoryShareToUse { get; } = DEFAULT_MEMORY_SHARE_TO_USE;

        /// <summary>
        /// Объём памяти, которую можно использовать под пул,
        /// если определение доступной памяти закончилось неудачей.
        /// </summary>
        protected virtual float MemoryToUseIfSmthWhenWrong => DEFAULT_MEMORY_SIZE;

        /// <summary>
        /// Определяет размер объектного пула, исходя из объёма памяти, занимаемого одним элементом.
        /// </summary>
        /// <param name="sizeOfElement">
        /// Объём памяти в байтах, занимаемой одним элементом.
        /// Например, для массива чисел - количество элементов,
        /// помноженное на количество байтов, выделяемое под число.
        /// </param>
        public virtual int Define(int sizeOfElement)
        {
            if (sizeOfElement < 1)
                throw new ArgumentOutOfRangeException(paramName: nameof(sizeOfElement));

            float sizeOfElementInMB = Utils.GetSizeOfElementInMBytes(sizeOfElement);
            float memorysizeToUse = GetMemorySizeToUse();

            if (sizeOfElementInMB > memorysizeToUse)
                throw new MemoryLacksException("Размер элемента превышает объём доступной памяти.");

            float poolSize = memorysizeToUse / sizeOfElementInMB;
            return (int)poolSize;
        }

        protected virtual float GetMemorySizeToUse()
        {
            try
            {
                return MemoryShareToUse * Utils.GetAvailableMemorySize();
            }
            catch
            {
                Debugger.Break();
                return MemoryToUseIfSmthWhenWrong;
            }
        }
    }
}