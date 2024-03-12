using Microsoft.AspNetCore.Components;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace FRIWOCenter.Data.TRACE
{
    [Table("AREA",Schema ="TRACE")]
    public class Area
    {
        [Key]
        [Column("AREA_ID")]
        public int areaId { get; set; }

        [Column("AREA_DESCRIPTION")]
        public string areaDescritpion { get; set; }

        [Column("SIT")]
        public int sit { get; set; }
    }
}