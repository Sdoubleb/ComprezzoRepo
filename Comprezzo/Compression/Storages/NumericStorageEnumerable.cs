using System;
using System.Collections;
using System.Collections.Generic;

namespace Sbb.Compression.Storages
{
    /// <summary>
    /// Перечислитель значений временного хранилища с числовым ключом.
    /// </summary>
    public class NumericStorageEnumerable<TValue> : IEnumerable<TValue>
    {
        private readonly ISizeableStorage<long, TValue> _storage;

        public NumericStorageEnumerable(ISizeableStorage<long, TValue> storage)
        {
            _storage = storage;
        }

        public IEnumerator<TValue> GetEnumerator() => new NumericStorageEnumerator(_storage);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public struct NumericStorageEnumerator : IEnumerator<TValue>
        {
            private readonly ISizeableStorage<long, TValue> _storage;
            
            private long _counter;

            public NumericStorageEnumerator(ISizeableStorage<long, TValue> storage)
            {
                _storage = storage;
                _counter = 0;
                Current = default(TValue);
            }

            public TValue Current { get; private set; }

            object IEnumerator.Current => Current;

            public bool MoveNext()
            {
                if (_counter < _storage.TotalSize)
                {
                    TValue value;
                    while (!_storage.TryGetAndRemove(_counter, out value))
                        continue;
                    Current = value;
                    _counter++;
                    return true;
                }
                return false;
            }

            public void Reset()
            {
                if (_counter > 0)
                {
                    throw new ObjectDisposedException(objectName: GetType().FullName,
                        message: $"Перечисление значений временного хранилища {_storage.GetType()}"
                            + " можно выполнить только единожды.");
                }
            }

            void IDisposable.Dispose() { }
        }
    }

    public class NumericStorageEnumerableProvider<TValue> : INumericStorageEnumerableProvider<TValue>
    {
        public IEnumerable<TValue> ProvideNew(ISizeableStorage<long, TValue> storage)
            => new NumericStorageEnumerable<TValue>(storage);
    }
}