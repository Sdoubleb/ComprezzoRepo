namespace GZipper
{
    class AvoidingLockConcurrentStorage<TValue>
    {
        private const int DEFAULT_SIZE_OF_SUBSTORAGE = 16;

        private readonly long _totalCountOfElements;
        private readonly int _sizeOfSubstorage;

        private readonly ConcurrentDictionary<long, TValue>[] _substorages;

        public AvoidingLockConcurrentStorage(long totalCountOfElements)
            : this(totalCountOfElements, DEFAULT_SIZE_OF_SUBSTORAGE) { }

        public AvoidingLockConcurrentStorage(long totalCountOfElements, int sizeOfSubstorage)
        {
            _totalCountOfElements = totalCountOfElements;
            _sizeOfSubstorage = sizeOfSubstorage;

            long countOfSubstorages = totalCountOfElements / sizeOfSubstorage
                + (totalCountOfElements % sizeOfSubstorage == 0 ? 0 : 1);

            _substorages = new ConcurrentDictionary<long, TValue>[countOfSubstorages];
            for (int i = 0; i < countOfSubstorages; i++)
                _substorages[i] = new ConcurrentDictionary<long, TValue>();
        }

        public void Add(long key, TValue value)
        {
            GetSubstorage(key).Add(key, value);
        }

        public bool TryGetAndRemove(long key, out TValue value)
        {
            return GetSubstorage(key).TryGetAndRemove(key, out value);
        }

        private ConcurrentDictionary<long, TValue> GetSubstorage(long key)
        {
            long indexOfSubstorage = key / _sizeOfSubstorage;
            return _substorages[indexOfSubstorage];
        }
    }
}