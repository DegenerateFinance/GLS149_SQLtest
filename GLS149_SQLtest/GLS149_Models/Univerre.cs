using System;
using System.Collections.Generic;

namespace GLS149_SQLtest.Models;

public partial class Univerre
{
    public string UVRTabla { get; set; } = null!;

    public int UVRKeyN01 { get; set; }

    public int UvrkeyN02 { get; set; }

    public string UvrkeyC01 { get; set; } = null!;

    public string UvrkeyC02 { get; set; } = null!;

    public string? UVRTxt01 { get; set; }

    public string? Uvrtxt02 { get; set; }

    public int? UVRNum01 { get; set; }

    public int? UVRNum02 { get; set; }

    public decimal? UVRBar01 { get; set; }

    public decimal? UVRBar02 { get; set; }

    public DateTime? Uvrfec01 { get; set; }

    public DateTime? Uvrfec02 { get; set; }

    public DateTime? Uvrdt01 { get; set; }

    public DateTime? Uvrdt02 { get; set; }
}
