using System;
using System.Collections.Generic;

namespace GLS149_SQLtest.TestModels;

public partial class Univerre
{
    public string Uvrtabla { get; set; } = null!;

    public int UvrkeyN01 { get; set; }

    public int UvrkeyN02 { get; set; }

    public string UvrkeyC01 { get; set; } = null!;

    public string UvrkeyC02 { get; set; } = null!;

    public string? Uvrtxt01 { get; set; }

    public string? Uvrtxt02 { get; set; }

    public int? Uvrnum01 { get; set; }

    public int? Uvrnum02 { get; set; }

    public decimal? Uvrbar01 { get; set; }

    public decimal? Uvrbar02 { get; set; }

    public DateTime? Uvrfec01 { get; set; }

    public DateTime? Uvrfec02 { get; set; }

    public DateTime? Uvrdt01 { get; set; }

    public DateTime? Uvrdt02 { get; set; }
}
