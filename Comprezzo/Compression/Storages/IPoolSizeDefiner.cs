namespace Sbb.Compression.Storages
{
    /// <summary>
    /// Интерфейс определителя размера объектного пула.
    /// </summary>
    public interface IPoolSizeDefiner
    {
        /// <summary>
        /// Определяет размер объектного пула, исходя из объёма памяти, занимаемого одним элементом.
        /// </summary>
        /// <param name="sizeOfElement">Объём памяти в байтах, занимаемой одним элементом.</param>
        /// <exception cref="MemoryLacksException">
        /// Размер элемента превышает объём доступной памяти.
        /// </exception>
        int Define(int sizeOfElement);
    }
}