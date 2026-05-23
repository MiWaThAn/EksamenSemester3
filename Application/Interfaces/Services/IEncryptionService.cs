using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.Services
{
    public interface IEncryptionService
    {
        Task<string> Encrypt(string input);
        Task<string> UnEncrypt(string input);
    }
}
