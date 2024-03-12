using Microsoft.AspNetCore.Components;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FRIWOCenter.Data.TRACE
{
    public class Monthly_Output
    {

        [Column("MONTHH")]
        public String MONTHH { get; set; }
        [Key]
        [Column("PART_NO")]
        public String PART_NO { get; set; }

        [Column("QTY")]
        public String QTY { get; set; }

        [Column("PROD_TIME")]
        public String PROD_TIME { get; set; }
    }
}
