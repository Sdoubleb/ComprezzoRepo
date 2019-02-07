namespace Sbb.Compression.Common
{
    public interface ICleaner<T> where T : class
    {
        void Clean(T obj);
    }
}