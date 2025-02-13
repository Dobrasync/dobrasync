using System.Security.Cryptography;
using System.Text;

namespace Dobrasync.Common.Util;

public static class ChecksumUtil
{
    public static string CalculateBlockChecksum(byte[] payload)
    {
        if (payload == null)
            throw new ArgumentNullException(nameof(payload), "Input data cannot be null.");

        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] hashBytes = sha256.ComputeHash(payload);

            // Convert the byte array to a hexadecimal string
            StringBuilder sb = new StringBuilder(hashBytes.Length * 2);
            foreach (byte b in hashBytes)
            {
                sb.AppendFormat("{0:x2}", b);
            }
            return sb.ToString();
        }
    }
    
    public static string CalculateFileChecksum(byte[] totalFileContent)
    {
        // TODO just use the same algo for now
        return CalculateBlockChecksum(totalFileContent);
    }
    
    public static bool VerifyBlockChecksum(byte[] payload, string checksum)
    {
        return CalculateBlockChecksum(payload) == checksum;
    }
}