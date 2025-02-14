namespace Dobrasync.Common.Util;

public class Chunker
{
    private static readonly int _blockSize = 1024000;
    
    public static List<byte[]> ContentToBlocks(byte[] originalContent)
    {
        List<byte[]> blocks = new();
        
        for (int i = 0; i < originalContent.Length; i += _blockSize)
        {
            int remainingLength = originalContent.Length - i;
            int currentBlockSize = Math.Min(_blockSize, remainingLength);

            byte[] block = new byte[currentBlockSize];
            Array.Copy(originalContent, i, block, 0, currentBlockSize);
            blocks.Add(block);
        }

        return blocks;
    }
    
    public static byte[] BlocksToContent(IEnumerable<byte[]> blocks)
    {
        List<byte> content = new();

        foreach (var block in blocks)
        {
            content.AddRange(block);
        }
        
        return content.ToArray();
    }
}