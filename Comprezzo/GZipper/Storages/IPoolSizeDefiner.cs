namespace Sbb.Compression.Storages
{
    public interface IPoolSizeDefiner
    {
        int Define(int sizeOfElement);
    }
}