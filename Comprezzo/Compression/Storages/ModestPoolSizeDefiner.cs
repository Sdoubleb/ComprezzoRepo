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
            float availableMemory = GetAvailableMemory();
            float memoryToUse = MemoryShareToUse * availableMemory;
            float size = memoryToUse / Utils.GetSizeOfElementInMBytes(sizeOfElement);
            return (int)size;
        }

        protected virtual float GetAvailableMemory()
        {
            try
            {
                return Utils.GetAvailableMemory();
            }
            catch
            {
                Debugger.Break();
                return MemoryToUseIfSmthWhenWrong;
            }
        }
    }
}