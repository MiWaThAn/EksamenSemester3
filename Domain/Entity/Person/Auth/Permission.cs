using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entity.Person.Auth
{
    public class Permission : Base
    {
        public string Title { get; set; }
        public List<RolePermission> Permissions = new();

        public Permission()
        { }
        public Permission(string title)
        {
            Title = title;
        }
    }
}
