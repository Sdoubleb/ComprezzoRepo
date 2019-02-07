namespace Sbb.Compression.Stream4ers
{
    public struct BlockyStream4erPair
    {
        public IBlockyStreamReader StreamReader { get; set; }
        public IBlockyStreamWriter StreamWriter { get; set; }
    }
}