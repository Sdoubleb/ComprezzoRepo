using System.Diagnostics;

namespace Sbb.Compression.Storages
{
    public class ModestPoolSizeDefiner : IPoolSizeDefiner
    {
        // для начала будем использовать такую долю доступной памяти
        private const float DEFAULT_MEMORY_SHARE_TO_USE = 1 / 4F;

        private const float DEFAULT_MEMORY_SIZE = 256;

        protected virtual float MemoryShareToUse { get; } = DEFAULT_MEMORY_SHARE_TO_USE;

        protected virtual float MemoryToUseIfSmthWhenWrong => DEFAULT_MEMORY_SIZE;

        public virtual int Define(int sizeOfElement)
        {
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