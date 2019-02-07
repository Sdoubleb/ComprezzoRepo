using System.Collections.Generic;

namespace Sbb.Compression.Storages
{
    public interface INumericStorageEnumerableProvider<TValue>
    {
        IEnumerable<TValue> ProvideNew(IStorage<long, TValue> storage, long totalCount);
    }
}