using System.Collections.Generic;

namespace Sbb.Compression.Storages
{
    public class ConcurrentStorage<TKey, TValue> : IStorage<TKey, TValue>
    {
        private readonly Dictionary<TKey, TValue> _dictionary = new Dictionary<TKey, TValue>();

        private readonly object _locker = new object();

        public void Add(TKey key, TValue value)
        {
            lock (_locker)
            {
                _dictionary.Add(key, value);
            }
        }

        public bool TryGetAndRemove(TKey key, out TValue value)
        {
            lock (_locker)
            {
                if (_dictionary.TryGetValue(key, out value))
                {
                    _dictionary.Remove(key);
                    return true;
                }
            }
            value = default(TValue);
            return false;
        }
    }
}