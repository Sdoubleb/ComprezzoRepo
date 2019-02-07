using System.Collections.Generic;

namespace Sbb.Compression.Storages
{
    public interface INumericStorageEnumerableProvider<TValue>
    {
        IEnumerable<TValue> ProvideNew(ISizeableStorage<long, TValue> storage);
    }
}