using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Internal.Scripts.Core.Utils.Arrays;

namespace Internal.Scripts.Core.Utils.Cryptography {
    public static class Cryptography {
        [Serializable]
        public struct AesEncryptedText {
            public string initVector;
            public string data;
        }
        
        public static AesEncryptedText Encrypt(string data, string key) {
            if (data == null || data.Length <= 0)
                throw new ArgumentNullException(nameof(data));
            if (key == null || key.Length <= 0)
                throw new ArgumentNullException(nameof(key));

            string encrypted;
            string iv;
            var byteArrayKey = ArraysUtils.ClampArray(Encoding.ASCII.GetBytes(key), 32);

            using (var aesAlg = new AesCryptoServiceProvider() {KeySize = 256}) {
                aesAlg.Key = byteArrayKey;
                iv = Convert.ToBase64String(aesAlg.IV);

                var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using var msEncrypt = new MemoryStream();
                using var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);
                using (var swEncrypt = new StreamWriter(csEncrypt)) {
                    swEncrypt.Write(data);
                }

                encrypted = Convert.ToBase64String(msEncrypt.ToArray());
            }

            return new AesEncryptedText {data = encrypted, initVector = iv};
        }

        public static string Decrypt(AesEncryptedText encryptedData, string key) {
            if (encryptedData.data == null || encryptedData.data.Length <= 0
                                           || encryptedData.initVector == null || encryptedData.initVector.Length <= 0)
                throw new ArgumentNullException(nameof(encryptedData));
            if (key == null || key.Length <= 0)
                throw new ArgumentNullException(nameof(key));

            var byteArrayKey = ArraysUtils.ClampArray(Encoding.ASCII.GetBytes(key), 32);

            using var aesAlg = new AesCryptoServiceProvider() {KeySize = 256};
            aesAlg.Key = byteArrayKey;
            aesAlg.IV = Convert.FromBase64String(encryptedData.initVector);
            var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
            using var msDecrypt = new MemoryStream(Convert.FromBase64String(encryptedData.data));
            using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
            using var srDecrypt = new StreamReader(csDecrypt, Encoding.ASCII);
            var decrypted = srDecrypt.ReadToEnd();

            return decrypted;
        }

        public static string GetMd5Hash(string data) {
            using var md5 = new MD5CryptoServiceProvider();
            var hash = Convert.ToBase64String(md5.ComputeHash(Encoding.ASCII.GetBytes(data)));

            return hash;
        }
    }
}