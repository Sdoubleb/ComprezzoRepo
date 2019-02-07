namespace Sbb.Compression.Stream4ers
{
    public struct OrderlyStream4erPair
    {
        public IOrderlyStreamReader StreamReader { get; set; }
        public IOrderlyStreamWriter StreamWriter { get; set; }
    }
}