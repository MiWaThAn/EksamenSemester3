using Application.Interfaces.Services;
using System;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Service.Security
{
    public class EncryptionService : IEncryptionService
    {
        private readonly byte[] _key;
        public EncryptionService(byte[] key)
        {
            if (key == null || key.Length != 32)
                throw new ArgumentException("Key must be exactly 256 bits (32 bytes).", nameof(key));
            _key = key;
        }
        public string Encrypt(string input)
        {
            if(string.IsNullOrEmpty(input))
                throw new ArgumentException("Input cannot be null or empty.", nameof(input));
            byte[] plainBytes = Encoding.UTF8.GetBytes(input);
            
            byte[] nonce = new byte[AesGcm.NonceByteSizes.MaxSize];
            byte[] tag = new byte[AesGcm.TagByteSizes.MaxSize];
            byte[] cipherBytes = new byte[plainBytes.Length];

            RandomNumberGenerator.Fill(nonce);

            using(var aesGcm = new AesGcm(_key,tag.Length))
            {
                aesGcm.Encrypt(nonce, plainBytes, cipherBytes, tag);
            }
            using(var ms = new MemoryStream())
            {
                ms.Write(nonce, 0, nonce.Length);
                ms.Write(tag, 0, tag.Length);
                ms.Write(cipherBytes, 0, cipherBytes.Length);
                return Convert.ToBase64String(ms.ToArray());
            }
        }
        public string Decrypt(string input)
        {
            if (string.IsNullOrEmpty(input)) return input;
            byte[] fullPayload = Convert.FromBase64String(input);

            int nonceSize = AesGcm.NonceByteSizes.MaxSize;
            int tagSize = AesGcm.TagByteSizes.MaxSize;
            int cipherSize = fullPayload.Length - nonceSize - tagSize;

            if (cipherSize < 0)
                throw new CryptographicException("Invalid ciphertext payload.");

            byte[] nonce = new byte[nonceSize];
            byte[] tag = new byte[tagSize];
            byte[] cipherBytes = new byte[cipherSize];

            Buffer.BlockCopy(fullPayload, 0, nonce, 0, nonceSize);
            Buffer.BlockCopy(fullPayload, nonceSize, tag, 0, tagSize);
            Buffer.BlockCopy(fullPayload, nonceSize + tagSize, cipherBytes, 0, cipherSize);

            byte[] plainBytes = new byte[cipherSize];

            using (var aesGcm = new AesGcm(_key, tagSize))
            {
                aesGcm.Decrypt(nonce, cipherBytes, tag, plainBytes);
            }
            return Encoding.UTF8.GetString(plainBytes);
        }
    }
}
