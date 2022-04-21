using System.Security.Cryptography;
using System.Text;
namespace Receiver
{
    public static class EncryptionHelper
    {
        public static string ComputeMD5(string s)
        {
            MD5 md5 = MD5.Create();
            byte[] inputBytes = Encoding.ASCII.GetBytes(s);
            byte[] hash = md5.ComputeHash(inputBytes);

            StringBuilder stringBuilder = new StringBuilder();
            foreach (byte item in hash)
            {
                stringBuilder.Append(item.ToString("x2"));
            }
            return stringBuilder.ToString();
        }
    }
}