using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RijndaelDemo
{
    public class Program
    {
        public static async Task<string> DecryptStringAsync(string encryptedData, byte[] key, byte[] iv, CancellationToken cancellationToken = default(CancellationToken))
        {
            var cipherTextBytes = Convert.FromBase64String(encryptedData);
            var plainTextBytes = new byte[cipherTextBytes.Length];
            using (var rijndael = Rijndael.Create())
            using (var memoryStream = new MemoryStream(cipherTextBytes))
            using (var cryptoStream = new CryptoStream(memoryStream, rijndael.CreateDecryptor(key, iv), CryptoStreamMode.Read))
            {
                var decryptedByteCount = await cryptoStream.ReadAsync(plainTextBytes, 0, plainTextBytes.Length, cancellationToken).ConfigureAwait(false);
                memoryStream.Close();
                cryptoStream.Close();
                rijndael.Clear();
                return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
            }
        }

        public static async Task<string> DecryptStringAsync(string encryptedData, string key, string iv, CancellationToken cancellationToken = default(CancellationToken))
        {
            var cryptBytes = GetCryptBytes(key, iv);
            return await DecryptStringAsync(encryptedData, cryptBytes.Item1, cryptBytes.Item2, cancellationToken).ConfigureAwait(false);
        }

        public static async Task<string> EncryptStringAsync(string plainData, byte[] key, byte[] iv, CancellationToken cancellationToken = default(CancellationToken))
        {
            byte[] cipherMessageBytes;
            using (var rijndael = Rijndael.Create())
            using (var memoryStream = new MemoryStream())
            using (var cryptoStream = new CryptoStream(memoryStream, rijndael.CreateEncryptor(key, iv), CryptoStreamMode.Write))
            {
                var plainDataInBytes = Encoding.UTF8.GetBytes(plainData);
                await cryptoStream.WriteAsync(plainDataInBytes, 0, plainDataInBytes.Length, cancellationToken).ConfigureAwait(false);
                cryptoStream.FlushFinalBlock();
                cipherMessageBytes = memoryStream.ToArray();
                memoryStream.Close();
                cryptoStream.Close();
                rijndael.Clear();
            }
            return Convert.ToBase64String(cipherMessageBytes);
        }

        public static async Task<string> EncryptStringAsync(string plainData, string key, string iv, CancellationToken cancellationToken = default(CancellationToken))
        {
            var cryptBytes = GetCryptBytes(key, iv);
            return await EncryptStringAsync(plainData, cryptBytes.Item1, cryptBytes.Item2, cancellationToken).ConfigureAwait(false);
        }

        public static Tuple<byte[], byte[]> GetCryptBytes(string key, string iv)
        {
            var keyBytes = Encoding.UTF8.GetBytes(key);
            var ivBytes = Encoding.UTF8.GetBytes(iv);
            const int keySize = 32;
            const int ivSize = 16;
            Array.Resize(ref keyBytes, keySize);
            Array.Resize(ref ivBytes, ivSize);
            return Tuple.Create(keyBytes, ivBytes);
        }

        public static Tuple<byte[], byte[]> GetRijndaelCryptoKeys()
        {
            using (var rijndael = Rijndael.Create())
            {
                var keys = Tuple.Create(rijndael.Key, rijndael.IV);
                rijndael.Clear();
                return keys;
            }
        }

        public static void Main()
        {
            var keys = GetRijndaelCryptoKeys();
            var key = keys.Item1;
            var vi = keys.Item2;
            var strKey = "This is the key for encrypt"; //Encoding.UTF8.GetString(key);
            var strVi = "This is the initialization vector"; //Encoding.UTF8.GetString(vi);
            Console.Write("Write phrase to encrypt:");
            var phrase = Console.ReadLine();
            Console.WriteLine($"Encrypt with keys: key--> {strKey}  vi--> {strVi}");
            var encrypted = EncryptStringAsync(phrase, strKey, strVi).Result;
            Console.WriteLine($"Encrypted text: {encrypted}");
            var decrypted = DecryptStringAsync(encrypted, strKey, strVi).Result;
            Console.WriteLine($"Decrypted text: {decrypted}");
            Console.ReadKey();
        }
    }
}