﻿namespace Sbb.Compression.Storages
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
        int Define(int sizeOfElement);
    }
}