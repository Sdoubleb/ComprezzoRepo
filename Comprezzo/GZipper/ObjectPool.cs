using System.Collections.Generic;
using System.Threading;

namespace GZipper
{
    class ObjectPool<T>
        where T : class
    {
        private readonly ICreator<T> _creator;
        private readonly ICleaner<T> _cleaner;
        private readonly int _maxCount;

        private readonly Queue<T> _pool = new Queue<T>();

        private int _currentCount;

        private object _locker = new object();
        private Semaphore _semaphore;

        public ObjectPool(ICreator<T> creator, ICleaner<T> cleaner, int maxCount)
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
                    T newObj = _creator.Create();
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
            _cleaner?.Clean(obj);
            lock (_locker)
            {
                _pool.Enqueue(obj);
                _semaphore.Release();
            }
        }
    }
}