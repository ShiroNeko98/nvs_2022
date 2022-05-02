using System.Security.Cryptography;
using System.Text;
namespace Receiver
{
    public static class EncryptionHelper
    {
        public static string ComputeMD5(byte[] dataBytes)
        {
            byte[]? hash;
            using (MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                md5.TransformFinalBlock(dataBytes, 0, dataBytes.Length);
                hash = md5.Hash;
            }
            
            StringBuilder stringBuilder = new StringBuilder();
            if (hash == null) return stringBuilder.ToString();
            
            foreach (byte item in hash)
            {
                stringBuilder.Append(item.ToString("x2"));
            }

            return stringBuilder.ToString();
            
            /*
            MD5 md5 = MD5.Create();
            byte[] inputBytes = Encoding.ASCII.GetBytes(s);
            byte[] hash = md5.ComputeHash(inputBytes);

            StringBuilder stringBuilder = new StringBuilder();
            foreach (byte item in hash)
            {
                stringBuilder.Append(item.ToString("x2"));
            }
            
            return stringBuilder.ToString(); */
        }
    }
}