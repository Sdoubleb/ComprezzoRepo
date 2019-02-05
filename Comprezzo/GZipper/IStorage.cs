namespace GZipper
{
    public interface IStorage<TKey, TValue>  // TODO: унаследовать от IDictionary<TKey, TValue>
    {
        void Add(TKey key, TValue value);

        bool TryGetAndRemove(TKey key, out TValue value);
    }

    public interface IStorageProvider<TKey, TValue>
    {
        IStorage<TKey, TValue> ProvideNew();
    }
}