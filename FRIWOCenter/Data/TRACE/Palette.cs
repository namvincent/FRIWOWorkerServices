using Microsoft.AspNetCore.Components;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace FRIWOCenter.Data.TRACE
{
    [Table("WIP_CONTROL")]
    public class Palette
    {
        [Key]
        [Column("PALETTE_ID")]
        public string paletteId { get; set; }

        [Column("AREA_ID")]
        public string areaId { get; set; }

        [Column("ORDER_NO")]
        public string orderNo { get; set; }

        [Column("PART_NO")]
        public string partNo { get; set; }

        [Column("QUANTITY")]
        public int quantity { get; set; }

        [Column("STATUS")]
        public int status { get; set; }
    }
}