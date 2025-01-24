using System.Security.Cryptography;

namespace Dobrasync.Common.Util;

public static class ChecksumUtil
{
    public static byte[] CalculateBlockChecksum(byte[] payload)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            return sha256.ComputeHash(payload);
        }
    }
    
    public static bool VerifyBlockChecksum(byte[] payload, byte[] checksum)
    {
        byte[] calculatedChecksum = CalculateBlockChecksum(payload);
        
        return calculatedChecksum.SequenceEqual(checksum);
    }

    public static string ByteArrayToHexString(byte[] byteArray)
    {
        return BitConverter.ToString(byteArray).Replace("-", "").ToLower();
    }
    
    public static byte[] HexStringToByteArray(string hexString)
    {
        int length = hexString.Length;
        byte[] byteArray = new byte[length / 2];
        for (int i = 0; i < length; i += 2)
        {
            byteArray[i / 2] = Convert.ToByte(hexString.Substring(i, 2), 16);
        }
        return byteArray;
    }
}