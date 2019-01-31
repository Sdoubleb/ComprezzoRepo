namespace GZipper
{
    interface ICleaner<T>
        where T : class
    {
        void Clean(T obj);
    }
}