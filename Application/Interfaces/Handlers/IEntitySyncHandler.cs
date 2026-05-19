using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.Handlers
{
    public interface IEntitySyncHandler
    {

        public Task ProcessAsync();
        public Task CreateAsync();
        public Task UpdateAsync();
       

    }
}
