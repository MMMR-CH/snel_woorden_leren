using System;
using System.Text;
using System.Security.Cryptography;

namespace UnicoStudio.UnicoLibs.Utilities
{
    public static class Crypto
    {
        //ToDo: Set random key per application
        const string KEY = "pfoAeJHldmHHvh!!dlwq";

        public static string Encrypt(string value)
        {
            using (TripleDESCryptoServiceProvider tripleDESCryptoServiceProvider = GetTripleDESCryptoServiceProvider())
            {
                using (ICryptoTransform cTransform = tripleDESCryptoServiceProvider.CreateEncryptor())
                {
                    byte[] encryptArray = Encoding.UTF8.GetBytes(value);
                    byte[] resultArray = cTransform.TransformFinalBlock(encryptArray, 0, encryptArray.Length);
                    return Convert.ToBase64String(resultArray);
                }
            }
        }

        public static string Decrypt(string value)
        {
            using (TripleDESCryptoServiceProvider tripleDESCryptoServiceProvider = GetTripleDESCryptoServiceProvider())
            {
                using (ICryptoTransform cTransform = tripleDESCryptoServiceProvider.CreateDecryptor())
                {
                    byte[] toEncryptArray = Convert.FromBase64String(value);
                    byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
                    return Encoding.UTF8.GetString(resultArray);
                }
            }
        }

        static TripleDESCryptoServiceProvider GetTripleDESCryptoServiceProvider()
        {
            byte[] keyArray = null;

            using (MD5CryptoServiceProvider hashMd5 = new MD5CryptoServiceProvider())
            {
                keyArray = hashMd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(KEY));
            }

            return new TripleDESCryptoServiceProvider
            {
                Key = keyArray,
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };
        }
    }
}