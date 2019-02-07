using System;
using System.Collections;
using System.Collections.Generic;

namespace Sbb.Compression.Storages
{
    class NumericStorageEnumerable<TValue> : IEnumerable<TValue>
    {
        private readonly IStorage<long, TValue> _storage;

        public NumericStorageEnumerable(IStorage<long, TValue> storage, long totalCount)
        {
            _storage = storage;
            TotalCount = totalCount;
        }

        public long TotalCount { get; }

        public IEnumerator<TValue> GetEnumerator() => new NumericStorageEnumerator(_storage, TotalCount);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public struct NumericStorageEnumerator : IEnumerator<TValue>
        {
            private readonly IStorage<long, TValue> _storage;

            private readonly long _totalCount;
            private long _counter;

            public NumericStorageEnumerator(IStorage<long, TValue> storage, long totalCount)
            {
                _storage = storage;
                _counter = 0;
                _totalCount = totalCount;
                Current = default(TValue);
            }

            public TValue Current { get; private set; }

            object IEnumerator.Current => Current;

            public bool MoveNext()
            {
                if (_counter < _totalCount)
                {
                    TValue value;
                    while (!_storage.TryGetAndRemove(_counter++, out value))
                        continue;
                    Current = value;
                    return true;
                }
                return false;
            }

            public void Reset()
            {
                if (_counter > 0)
                {
                    throw new ObjectDisposedException(objectName: GetType().FullName,
                        message: $"Перечисление значений временного хранилища {_storage.GetType()} можно выполнить только единожды.");
                }
            }

            public void Dispose() => Reset();
        }
    }

    class NumericStorageEnumerableProvider<TValue> : INumericStorageEnumerableProvider<TValue>
    {
        public IEnumerable<TValue> ProvideNew(IStorage<long, TValue> storage, long totalCount)
            => new NumericStorageEnumerable<TValue>(storage, totalCount);
    }
}