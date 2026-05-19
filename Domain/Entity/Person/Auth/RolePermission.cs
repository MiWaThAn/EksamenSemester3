using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Domain.Entity.Person.Auth
{
    public class RolePermission : Base
    {
        public Guid PermissionId { get; private set; }
        public Guid RoleId { get; private set; }
        public string Title { get; private set; }
        internal RolePermission(Permission perm, Role role)
        {
            PermissionId = perm.Id;
            RoleId = role.Id;
            Title = perm.Title;
        }
        public RolePermission() { }
    }
}
