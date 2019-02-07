using System;
using System.Collections.Generic;
using System.Threading;
using Sbb.Compression.Common;

namespace Sbb.Compression.Storages
{
    class ObjectPool<T> : IWaitableObjectPool<T>, IDisposable where T : class
    {
        private readonly Func<T> _creator;
        private readonly Action<T> _cleaner;
        private readonly int _maxCount;

        private readonly Queue<T> _pool = new Queue<T>();

        private int _currentCount;

        private object _locker = new object();
        private Semaphore _semaphore;

        public ObjectPool(ICreator<T> creator, ICleaner<T> cleaner = null, int maxCount = Int32.MaxValue)
            : this(() => creator.Create(), obj => cleaner.Clean(obj), maxCount) { }

        public ObjectPool(Func<T> creator, Action<T> cleaner = null, int maxCount = Int32.MaxValue)
        {
            _creator = creator;
            _cleaner = cleaner;
            _maxCount = maxCount;
            _semaphore = new Semaphore(0, maxCount);
        }

        public T Get()
        {
            lock (_locker)
            {
                if (_pool.Count > 0)
                    return _pool.Dequeue();
                if (_currentCount < _maxCount)
                {
                    T newObj = _creator();
                    _currentCount++;
                    return newObj;
                }
            }
            return null;
        }

        public T Wait()
        {
            T obj;
            while ((obj = Get()) == null)
                _semaphore.WaitOne();
            return obj;
        }

        public void Release(T obj)
        {
            _cleaner?.Invoke(obj);
            lock (_locker)
            {
                _pool.Enqueue(obj);
                _semaphore.Release();
            }
        }

        public void Dispose() => _semaphore.Close();
    }

    class SizeDefiningObjectPoolProvider<T> : IWaitableObjectPoolProvider<T> where T : class
    {
        private readonly Func<T> _creator;
        private readonly Action<T> _cleaner;

        public SizeDefiningObjectPoolProvider(int sizeOfElement, ICreator<T> creator, ICleaner<T> cleaner = null)
            : this(sizeOfElement, () => creator.Create(), obj => cleaner.Clean(obj)) { }

        public SizeDefiningObjectPoolProvider(int sizeOfElement, Func<T> creator, Action<T> cleaner = null)
        {
            _creator = creator;
            _cleaner = cleaner;
            SizeOfElement = sizeOfElement;
        }

        public int SizeOfElement { get; set; }

        public IPoolSizeDefiner PoolSizeDefiner { get; set; } = new ModestPoolSizeDefiner();

        public IWaitableObjectPool<T> ProvideNew()
            => new ObjectPool<T>(_creator, _cleaner, PoolSizeDefiner.Define(SizeOfElement));
    }
}