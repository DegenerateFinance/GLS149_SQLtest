using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLS149_SQLtest
{
    [Table("gls149_test.Univerre")]
    public class TableUniverre
    {
        [Column("UVRTabla")]
        public string UVRTabla { get; set; }
        [Column("UVRKeyN01")]
        public int UVRKeyN01 { get; set; }
        [Column("UVRKeyN02")]
        public int UVRKeyN02 { get; set; }
        public string UVRKeyC01 { get; set; }
        public string UVRKeyC02 { get; set; }
        public string UVRTxt01 { get; set; }
        public string UVRTxt02 { get; set; }
        public int? UVRNum01 { get; set; }
        public int? UVRNum02 { get; set; }
        public decimal? UVRBar01 { get; set; }
        public decimal? UVRBar02 { get; set; }
        public DateTime? UVRFec01 { get; set; }
        public DateTime? UVRFec02 { get; set; }
        public DateTime? UVRDT01 { get; set; }
        public DateTime? UVRDT02 { get; set; }
    }
}
