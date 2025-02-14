using Dobrasync.Common.Util;

namespace Dobrasync.Api.Tests.Util;

public class TestFile
{
    public string ContentSourcePath { get; set; }
    public byte[] Bytes { get; set; }

    public List<byte[]> Blocks { get; set; }
    public string Checksum => ChecksumUtil.CalculateFileChecksum(Bytes);

    public FileInfo FileInfo { get; set; }
    public string Path { get; set; }
    
    public TestFile(string contentSource, string libraryPath)
    {
        ContentSourcePath = contentSource;
        Path = libraryPath;
        
        FileInfo = new FileInfo(contentSource);
        Bytes = File.ReadAllBytes(contentSource);
        Blocks = Chunker.ContentToBlocks(Bytes);
    }
}