using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entity.Person.Auth
{
    public class Role : Base
    {
        public string Title {  get; private set; }
        public List<RolePermission> Permissions { get; private set; } = new List<RolePermission>();
        public List<Account> Accounts { get; private set; } = new List<Account>();
        public Role() : base()
        { 
        }
        public Role(string title) : base()
        { 
            Title = title;
        }
        public void AddPermissions(Permission permission)
        {
            Permissions.Add(new RolePermission(permission,this));
        }
        public void RemovePermissions(RolePermission permission)
        {
            Permissions.Remove(permission);
        }
    }
}
