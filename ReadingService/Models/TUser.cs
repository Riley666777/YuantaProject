﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace ReadingService.Models;

public partial class TUser
{
    public int FUserId { get; set; }

    public string FAccount { get; set; }

    public string FPassword { get; set; }

    public string FUserName { get; set; }

    public virtual ICollection<TLog> TLogs { get; set; } = new List<TLog>();
}