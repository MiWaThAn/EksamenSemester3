using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entity.Person.Auth
{
    public class Role : Base
    {
        public string Title {  get; private set; }
        public List<Permission> Permissions { get; private set; } = new List<Permission>();
        public Role() : base()
        { 
        }
        public Role(string title) : base()
        { 
            Title = title;
        }
        public void AddPermissions(Permission permissions)
        {
            Permissions.Add(permissions);
        }
        public void RemovePermissions(Permission permissions)
        {
            Permissions.Remove(permissions);
        }
    }
}
