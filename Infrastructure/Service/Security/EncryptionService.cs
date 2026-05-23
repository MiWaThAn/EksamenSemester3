using Application.Interfaces.Services;
using System;
using System.IO; // <-- Added this
using System.Security.Cryptography;
using System.Text;

namespace Infrastructure.Service.Security
{
    public class EncryptionService : IEncryptionService
    {
        private readonly byte[] _key;

        // AesGcm sizes are constant (Nonce = 12 bytes, Tag = 16 bytes)
        private const int NonceSize = 12;
        private const int TagSize = 16;

        public EncryptionService(byte[] key)
        {
            if (key == null || key.Length != 32)
                throw new ArgumentException("Key must be exactly 256 bits (32 bytes).", nameof(key));
            _key = key;
        }

        public string Encrypt(string input)
        {
            if (string.IsNullOrEmpty(input))
                throw new ArgumentException("Input cannot be null or empty.", nameof(input));

            byte[] plainBytes = Encoding.UTF8.GetBytes(input);

            byte[] nonce = new byte[NonceSize];
            byte[] tag = new byte[TagSize];
            byte[] cipherBytes = new byte[plainBytes.Length];

            RandomNumberGenerator.Fill(nonce);

            using (var aesGcm = new AesGcm(_key, TagSize))
            {
                aesGcm.Encrypt(nonce, plainBytes, cipherBytes, tag);
            }

            byte[] result = new byte[NonceSize + TagSize + cipherBytes.Length];
            Buffer.BlockCopy(nonce, 0, result, 0, NonceSize);
            Buffer.BlockCopy(tag, 0, result, NonceSize, TagSize);
            Buffer.BlockCopy(cipherBytes, 0, result, NonceSize + TagSize, cipherBytes.Length);

            return Convert.ToBase64String(result);
        }

        public string Decrypt(string input)
        {
            if (string.IsNullOrEmpty(input)) return input;

            byte[] fullPayload = Convert.FromBase64String(input);

            int cipherSize = fullPayload.Length - NonceSize - TagSize;

            if (cipherSize < 0)
                throw new CryptographicException("Invalid ciphertext payload.");

            byte[] nonce = new byte[NonceSize];
            byte[] tag = new byte[TagSize];
            byte[] cipherBytes = new byte[cipherSize];

            Buffer.BlockCopy(fullPayload, 0, nonce, 0, NonceSize);
            Buffer.BlockCopy(fullPayload, NonceSize, tag, 0, TagSize);
            Buffer.BlockCopy(fullPayload, NonceSize + TagSize, cipherBytes, 0, cipherSize);

            byte[] plainBytes = new byte[cipherSize];

            using (var aesGcm = new AesGcm(_key, TagSize))
            {
                aesGcm.Decrypt(nonce, cipherBytes, tag, plainBytes);
            }

            return Encoding.UTF8.GetString(plainBytes);
        }
    }
}