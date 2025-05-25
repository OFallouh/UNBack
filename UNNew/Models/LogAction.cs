using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace UNNew.Models;

public partial class LogAction
{
    public int Id { get; set; }

    public string? ActionName { get; set; }

    public string? ControllerName { get; set; }

    public string? UserName { get; set; }

    public string? ObjData { get; set; }

    public DateTime? ActDate { get; set; }

    [Column(TypeName = "CLOB")]
    public string BodyParameters { get; set; }

    [Column(TypeName = "CLOB")]
    public string? HeaderParameters { get; set; }

    public string? ResponseStatus { get; set; }

    [Column(TypeName = "CLOB")]
    public string? Result { get; set; }
}
