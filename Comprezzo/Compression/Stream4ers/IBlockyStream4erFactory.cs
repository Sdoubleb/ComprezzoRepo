namespace Sbb.Compression.Stream4ers
{
    public interface IBlockyStream4erFactory
    {
        BlockyStream4erPair CreateStream4erPair();
    }
}