using Domain.Interfaces.Item;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Service.Security
{
    public class HashingService : IHashingService
    {
        private const int WorkFactor = 12;
        public string Hash(string plainText)
        {
           return BCrypt.Net.BCrypt.HashPassword(plainText, WorkFactor);
        }
        public bool Verify(string hash, string plainText)
        {
            return BCrypt.Net.BCrypt.Verify(plainText,hash);
        }
    }
}
