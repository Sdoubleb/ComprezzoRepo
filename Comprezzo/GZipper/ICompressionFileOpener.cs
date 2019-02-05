using System.IO;
using System.IO.Compression;

namespace GZipper
{
    public interface ICompressionFileOpener
    {
        Stream OpenSource(string sourcePath, CompressionMode mode);

        Stream OpenTarget(string targetPath, CompressionMode mode);
    }
}