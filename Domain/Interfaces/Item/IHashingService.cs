using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Interfaces.Item
{
    public interface IHashingService
    {
        string Hash(string plainText);
        bool Verify(string hash, string plainText);
    }
}
