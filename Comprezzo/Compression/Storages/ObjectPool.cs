using System;
using System.Collections.Generic;
using System.Threading;
using Sbb.Compression.Common;

namespace Sbb.Compression.Storages
{
    /// <summary>
    /// Объектный пул, поддерживающий операцию ожидания объекта.
    /// </summary>
    public class ObjectPool<T> : IWaitableObjectPool<T> where T : class
    {
        private readonly Func<T> _creator;
        private readonly Action<T> _cleaner;
        
        // внутренняя очередь (массив, список - нужное подчеркнуть)
        private readonly Queue<T> _pool = new Queue<T>();

        // максимально допустимое число объектов, которые может создать пул
        private readonly int _maxCount;

        // текущее число объектов, которые создал пул
        private int _currentCount;

        private readonly object _locker = new object();
        private readonly Semaphore _semaphore; // позволяет ожидать объект

        private bool _disposed = false;

        /// <param name="maxCount">Максимальне число объектов, которые может создать пул.</param>
        public ObjectPool(ICreator<T> creator, ICleaner<T> cleaner = null, int maxCount = Int32.MaxValue)
            : this(() => creator.Create(), obj => cleaner.Clean(obj), maxCount) { }

        /// <param name="maxCount">Максимальне число объектов, которые может создать пул.</param>
        public ObjectPool(Func<T> creator, Action<T> cleaner = null, int maxCount = Int32.MaxValue)
        {
            _creator = creator ?? throw new ArgumentNullException(paramName: nameof(creator));
            _cleaner = cleaner;
            _maxCount = maxCount;
            _semaphore = new Semaphore(0, maxCount);
        }

        /// <summary>
        /// Возвращает объект пула или значение <c>null</c>,
        /// если объект на данный момент отсутствует.
        /// </summary>
        public T Get()
        {
            lock (_locker)
            {
                ThrowIfDisposed();

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

        /// <summary>
        /// Ожидает освобождения объекта пула и возвращает его.
        /// </summary>
        /// <remarks>
        /// При текущей реализации имеет смысл вызывать,
        /// только будучи безоговорочно уверенным в том, что дождёшься.
        /// </remarks>
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
                ThrowIfDisposed();

                _pool.Enqueue(obj);
                _semaphore.Release();
            }
        }

        private void ThrowIfDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(objectName: GetType().FullName);
        }

        public void Dispose()
        {
            lock (_locker)
            {
                _pool.Clear();
                _semaphore.Close();
            }
            _disposed = true;
        }
    }

    /// <summary>
    /// Обязан по запросу предоставить объектный пул в трезвом уме и добром здравии.
    /// Обязан знать объём занимаемой одним объект пула памяти,
    /// дабы рассчитать максимальное число элементов, которые может создать пул,
    /// исходя из объёма наличествующей оперативной памяти.
    /// </summary>
    public class SizeDefiningObjectPoolProvider<T> : IWaitableObjectPoolProvider<T> where T : class
    {
        private readonly Func<T> _creator;
        private readonly Action<T> _cleaner;
        
        private int _sizeOfElement;

        public SizeDefiningObjectPoolProvider(int sizeOfElement, ICreator<T> creator, ICleaner<T> cleaner = null)
            : this(sizeOfElement, () => creator.Create(), obj => cleaner.Clean(obj)) { }

        public SizeDefiningObjectPoolProvider(int sizeOfElement, Func<T> creator, Action<T> cleaner = null)
        {
            if (sizeOfElement < 1)
                throw new ArgumentOutOfRangeException(paramName: nameof(sizeOfElement));

            _creator = creator ?? throw new ArgumentNullException(paramName: nameof(creator));
            _cleaner = cleaner;
            _sizeOfElement = sizeOfElement;
        }

        /// <summary>
        /// Определитель размера объектного пула.
        /// </summary>
        public IPoolSizeDefiner PoolSizeDefiner { get; set; } = new ModestPoolSizeDefiner();

        /// <exception cref="MemoryLacksException">
        /// Размер элемента превышает объём доступной памяти.
        /// </exception>
        public IWaitableObjectPool<T> ProvideNew()
            => new ObjectPool<T>(_creator, _cleaner, PoolSizeDefiner.Define(_sizeOfElement));
    }
}