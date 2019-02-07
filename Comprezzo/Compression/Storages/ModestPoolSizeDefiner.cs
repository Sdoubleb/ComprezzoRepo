namespace Sbb.Compression.Storages
{
    class ModestPoolSizeDefiner : IPoolSizeDefiner
    {
        // для начала будем использовать такую долю доступной памяти
        private const float DEFAULT_MEMORY_SHARE_TO_USE = 1 / 4F;

        public virtual int Define(int sizeOfElement)
        {
            float availableMemory = Utils.GetAvailableMemory();
            float memoryToUse = MemoryShareToUse * availableMemory;
            float size = memoryToUse / GetSizeOfElementInMBytes(sizeOfElement);
            return (int)size;
        }

        protected virtual float MemoryShareToUse { get; } = DEFAULT_MEMORY_SHARE_TO_USE;

        private float GetSizeOfElementInMBytes(int sizeOfElementInBytes)
        {
            return sizeOfElementInBytes / 1024F / 1024;
        }
    }
}