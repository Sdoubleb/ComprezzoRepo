namespace Sbb.Compression.Stream4ers
{
    /// <summary>
    /// Интерфейс фабрики по созданию пары из поблочных чтеца и писца байтовых потоков.
    /// </summary>
    public interface IBlockyStream4erFactory
    {
        BlockyStream4erPair CreateStream4erPair();
    }
}