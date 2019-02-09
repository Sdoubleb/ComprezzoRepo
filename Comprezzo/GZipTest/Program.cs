using System;
using System.Diagnostics;
using Sbb.Compression.Compressors;

namespace GZipTest
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                CompressionParams.Instance = new CompressionParams(args);
            }
            catch (IndexOutOfRangeException)
            {
                Console.WriteLine("Неверно заданы параметры.");
                Console.ReadLine();
                return;
            }
            catch (ArgumentException ae)
            {
                Console.WriteLine(ae.Message);
                Console.ReadLine();
                return;
            }

            CompressionParams @params = CompressionParams.Instance;
            string suffix = $" {@params.InputFileName} в {@params.OutputFileName}";
            string beginMessage = (@params.Compress ? "Сжатие" : "Разжатие") + suffix + "...";
            string endMessage = (@params.Compress ? "Сжато" : "Разжато") + suffix + ".";

            Console.WriteLine(beginMessage);

            Stopwatch sw = new Stopwatch();
            sw.Start();

            try
            {
                Work();

                sw.Stop();

                Console.WriteLine(endMessage);
                Console.WriteLine($"Времени затрачено: {sw.Elapsed}.");
            }
            catch (Exception e)
            {          
#if DEBUG
                Debugger.Break();
#endif
                Console.WriteLine("Ошибка: " + e.Message);
            }

            Console.ReadLine();
        }

        private static void Work()
        {
            var creator = new FileCompressorCreator();
            using (IFileCompressor compressor = creator.Create())
            {
                CompressionParams @params = CompressionParams.Instance;
                if (@params.Compress)
                    compressor.Compress(@params.InputFileName, @params.OutputFileName);
                else
                    compressor.Decompress(@params.InputFileName, @params.OutputFileName);
            }
        }
    }
}