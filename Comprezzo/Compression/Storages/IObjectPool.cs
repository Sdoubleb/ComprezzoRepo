using System;

namespace Sbb.Compression.Storages
{
    public interface IObjectPool<T> : IDisposable where T : class
    {
        T Get();

        void Release(T obj);
    }

    public interface IWaitableObjectPool<T> : IObjectPool<T> where T : class
    {
        T Wait();
    }

    public interface IObjectPoolProvider<T> where T : class
    {
        IObjectPool<T> ProvideNew();
    }

    public interface IWaitableObjectPoolProvider<T> where T : class
    {
        IWaitableObjectPool<T> ProvideNew();
    }
}