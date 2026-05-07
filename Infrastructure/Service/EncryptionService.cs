using Application.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Service
{
    public class EncryptionService : IEncryptionService
    {
        Task<string> IEncryptionService.Encrypt(string input)
        {
            throw new NotImplementedException();
        }
    }
}
