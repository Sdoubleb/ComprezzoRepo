using System;

namespace GZipTest
{
    struct CompressionParams
    {
        public static CompressionParams Instance;

        public CompressionParams(string[] args)
        {
            switch (args[0].ToLower())
            {
                case "compress":
                    Compress = true;
                    break;
                case "decompress":
                    Compress = false;
                    break;
                default:
                    throw new ArgumentException($"Неподдерживаемая операция: '{args[0]}'.");
            }
            
            InputFileName = args[1];
            OutputFileName = args[2];
        }

        public bool Compress { get; }

        public string InputFileName { get; }
        public string OutputFileName { get; }
    }
}