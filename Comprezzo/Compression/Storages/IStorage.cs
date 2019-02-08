namespace Sbb.Compression.Storages
{
    /// <summary>
    /// Интерфейс временного хранилища пар ключ-значение.
    /// </summary>
    public interface IStorage<TKey, TValue>  // TODO: унаследовать от IDictionary<TKey, TValue>
    {
        void Add(TKey key, TValue value);

        bool TryGetAndRemove(TKey key, out TValue value);
    }

    /// <summary>
    /// Интерфейс временного хранилища пар ключ-значение,
    /// которое знает общее количество всех элементов, когда-либо бывших или будущих в нём.
    /// </summary>
    public interface ISizeableStorage<TKey, TValue> : IStorage<TKey, TValue>
    {
        /// <summary>
        /// Общее количество всех элементов, когда-либо бывших или будущих в хранилище.
        /// </summary>
        long TotalSize { get; }
    }

    public interface IStorageProvider<TKey, TValue>
    {
        IStorage<TKey, TValue> ProvideNew();
    }

    public interface ISizeableStorageProvider<TKey, TValue>
    {
        ISizeableStorage<TKey, TValue> ProvideNew(long totalSize);
    }
}