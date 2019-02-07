namespace Sbb.Compression.Storages
{
    public class ModestPoolSizeDefiner : IPoolSizeDefiner
    {
        // для начала будем использовать такую долю доступной памяти
        private const float DEFAULT_MEMORY_SHARE_TO_USE = 1 / 4F;

        protected virtual float MemoryShareToUse { get; } = DEFAULT_MEMORY_SHARE_TO_USE;

        public virtual int Define(int sizeOfElement)
        {
            float availableMemory = Utils.GetAvailableMemory();
            float memoryToUse = MemoryShareToUse * availableMemory;
            float size = memoryToUse / Utils.GetSizeOfElementInMBytes(sizeOfElement);
            return (int)size;
        }
    }
}