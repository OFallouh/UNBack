using System;
using System.Collections.Generic;

namespace UNNew.Models;

public partial class User
{
    public int Id { get; set; }

    public string? PasswordHash { get; set; }

    public string UserName { get; set; } = null!;

    public string? Email { get; set; }

    public string? Permissions { get; set; }
    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}
