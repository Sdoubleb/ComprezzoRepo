namespace Sbb.Compression.Storages
{
    public class AvoidingLockConcurrentStorage<TValue> : ISizeableStorage<long, TValue>
    {
        internal const int DEFAULT_SIZE_OF_SUBSTORAGE = 12;

        private readonly long _totalCountOfElements;
        private readonly int _sizeOfSubstorage;

        private readonly ConcurrentStorage<long, TValue>[] _substorages;

        public AvoidingLockConcurrentStorage(long totalCountOfElements)
            : this(totalCountOfElements, DEFAULT_SIZE_OF_SUBSTORAGE) { }

        public AvoidingLockConcurrentStorage(long totalCountOfElements, int sizeOfSubstorage)
        {
            _totalCountOfElements = totalCountOfElements;
            _sizeOfSubstorage = sizeOfSubstorage;

            long countOfSubstorages = Utils.CalculateCountOfBlocks(totalCountOfElements, sizeOfSubstorage);

            _substorages = new ConcurrentStorage<long, TValue>[countOfSubstorages];
            for (int i = 0; i < countOfSubstorages; i++)
                _substorages[i] = new ConcurrentStorage<long, TValue>();
        }

        public long TotalSize => _totalCountOfElements;

        public void Add(long key, TValue value)
        {
            GetSubstorage(key).Add(key, value);
        }

        public bool TryGetAndRemove(long key, out TValue value)
        {
            return GetSubstorage(key).TryGetAndRemove(key, out value);
        }

        private ConcurrentStorage<long, TValue> GetSubstorage(long key)
        {
            long indexOfSubstorage = key / _sizeOfSubstorage;
            return _substorages[indexOfSubstorage];
        }
    }

    public class AvoidingLockConcurrentStorageProvider<TValue> : ISizeableStorageProvider<long, TValue>
    {
        private readonly int _sizeOfSubstorage;

        public AvoidingLockConcurrentStorageProvider()
            : this(AvoidingLockConcurrentStorage<TValue>.DEFAULT_SIZE_OF_SUBSTORAGE) { }

        public AvoidingLockConcurrentStorageProvider(int sizeOfSubstorage)
        {
            _sizeOfSubstorage = sizeOfSubstorage;
        }

        public ISizeableStorage<long, TValue> ProvideNew(long totalCountOfElements)
        {
            return new AvoidingLockConcurrentStorage<TValue>(_sizeOfSubstorage);
        }
    }
}