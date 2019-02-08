using System;

namespace Sbb.Compression.Storages
{
    /// <summary>
    /// Потокобезопасное временное хранилище с числовым ключом, которое пытается избегать блокировок потоков.
    /// </summary>
    public class AvoidingLockConcurrentStorage<TValue> : ISizeableStorage<long, TValue>
    {
        // размер по умолчанию одного подхранилища
        internal const int DEFAULT_SIZE_OF_SUBSTORAGE = 12;

        internal const int MIN_SIZE_OF_SUBSTORAGE = 2;
        
        // размер одного подхранилища
        private readonly int _sizeOfSubstorage;

        // массив потокобезопасных подхранилищ, реально содержащих пары ключ-значение
        private readonly ConcurrentStorage<long, TValue>[] _substorages;

        /// <param name="totalCountOfElements">
        /// Общее количество элементов, которые когда-либо хранились или будут храниться.
        /// </param>
        public AvoidingLockConcurrentStorage(long totalCountOfElements)
            : this(totalCountOfElements, DEFAULT_SIZE_OF_SUBSTORAGE) { }
        
        /// <param name="totalCountOfElements">
        /// Общее количество элементов, которые когда-либо хранились или будут храниться.
        /// </param>
        /// <param name="sizeOfSubstorage">Размер одного подхранилища.</param>
        public AvoidingLockConcurrentStorage(long totalCountOfElements, int sizeOfSubstorage)
        {
            if (totalCountOfElements < 1)
                throw new ArgumentOutOfRangeException(paramName: nameof(totalCountOfElements));
            if (sizeOfSubstorage < MIN_SIZE_OF_SUBSTORAGE)
                throw new ArgumentOutOfRangeException(paramName: nameof(sizeOfSubstorage));
            if (sizeOfSubstorage > totalCountOfElements)
                throw new ArgumentException();

            _sizeOfSubstorage = sizeOfSubstorage;
            TotalSize = totalCountOfElements;

            // определяем длину одного подхранилища
            long countOfSubstorages = Utils.CalculateCountOfBlocks(totalCountOfElements, sizeOfSubstorage);

            _substorages = new ConcurrentStorage<long, TValue>[countOfSubstorages];
            for (int i = 0; i < countOfSubstorages; i++)
                _substorages[i] = new ConcurrentStorage<long, TValue>();
        }

        /// <summary>
        /// Общее количество элементов, которые когда-либо хранились или будут храниться.
        /// </summary>
        public long TotalSize { get; }

        public void Add(long key, TValue value)
        {
            GetSubstorage(key).Add(key, value);
        }

        public bool TryGetAndRemove(long key, out TValue value)
        {
            return GetSubstorage(key).TryGetAndRemove(key, out value);
        }
        
        // получаем нужное подхранилище по значению числового ключа;
        // операция потокобезопасна в многопоточной среде
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
            return new AvoidingLockConcurrentStorage<TValue>(totalCountOfElements, _sizeOfSubstorage);
        }
    }
}