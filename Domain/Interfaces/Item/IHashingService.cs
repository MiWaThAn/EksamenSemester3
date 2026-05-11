using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Interfaces.Item
{
    public interface IHashingService
    {
        string HashPassword(string plainTextPassword);
        bool VerifyPassword(string hash, string plainTextPassword);
    }
}
