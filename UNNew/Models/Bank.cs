using System;
using System.Collections.Generic;

namespace UNNew.Models;

public partial class Bank
{
    public int BanksId { get; set; }

    public string? BanksName { get; set; }

    public string? BankLogoUrl { get; set; }


    public virtual ICollection<AccountCompany> AccountCompanys { get; set; } = new List<AccountCompany>();

    public virtual ICollection<UnEmp> UnEmps { get; set; } = new List<UnEmp>();
}
