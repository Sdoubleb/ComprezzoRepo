namespace Sbb.Compression.Stream4ers
{
    /// <summary>
    /// Пара из поблочных чтеца и писца байтовых потоков.
    /// </summary>
    public struct BlockyStream4erPair
    {
        public IBlockyStreamReader StreamReader { get; set; }
        public IBlockyStreamWriter StreamWriter { get; set; }
    }
}