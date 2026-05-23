using Application.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Service.Security
{
    public class EncryptionService : IEncryptionService
    {
        Task<string> IEncryptionService.Encrypt(string input)
        {
            throw new NotImplementedException();
        }
        Task<string> IEncryptionService.UnEncrypt(string input)
        {
            throw new NotImplementedException();
        }
    }
}
