using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entity.Person.Auth
{
    public class DeviceToken : Base
    {
        public Guid AccountId { get; internal set; }
        public string Value { get; internal set; }
        internal DeviceToken(Account account, string value)
        {
            AccountId = account.Id;
            Value = value;
        }
        public DeviceToken() { }
    }
}
