using System.Collections.Generic;

namespace GZipper
{
    class AvoidingLockConcurrentStorage<TValue>
    {
        private const int DEFAULT_SIZE_OF_SUBSTORAGE = 16;

        private readonly long _totalCountOfElements;
        private readonly int _sizeOfSubstorage;

        private readonly Dictionary<long, TValue>[] _substorages;

        public AvoidingLockConcurrentStorage(long totalCountOfElements)
            : this(totalCountOfElements, DEFAULT_SIZE_OF_SUBSTORAGE) { }

        public AvoidingLockConcurrentStorage(long totalCountOfElements, int sizeOfSubstorage)
        {
            _totalCountOfElements = totalCountOfElements;
            _sizeOfSubstorage = sizeOfSubstorage;

            long countOfSubstorages = totalCountOfElements / sizeOfSubstorage
                + (totalCountOfElements % sizeOfSubstorage == 0 ? 0 : 1);
            _substorages = new Dictionary<long, TValue>[countOfSubstorages];
            for (int i = 0; i < countOfSubstorages; i++)
                _substorages[i] = new Dictionary<long, TValue>();
        }

        public void Add(long key, TValue value)
        {
            Dictionary<long, TValue> substorage = GetSubstorage(key);
            lock (substorage)
            {
                substorage.Add(key, value);
            }
        }

        public bool TryGetAndRemove(long key, out TValue value)
        {
            Dictionary<long, TValue> substorage = GetSubstorage(key);
            lock (substorage)
            {
                if (substorage.TryGetValue(key, out value))
                {
                    substorage.Remove(key);
                    return true;
                }
            }
            value = default(TValue);
            return false;
        }

        private Dictionary<long, TValue> GetSubstorage(long key)
        {
            long indexOfSubstorage = key / _sizeOfSubstorage;
            return _substorages[indexOfSubstorage];
        }
    }
}
