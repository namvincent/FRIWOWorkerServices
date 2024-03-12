using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace FRIWOServerApi.Data.TRACE
{
    [Keyless]
    public partial class BusinessReport
    {     
        
        [Column("PART_NO",TypeName = "VARCHAR2(7)")]
        public string Part_no { get ; set; }

        [Column("OUTPUT", TypeName = "NUMBER")]
        public int Output { get; set; }

        [Column("PRICE", TypeName = "NUMBER")]
        public double Price { get; set; }

        [Column("CALCULATION_DATE", TypeName = "VARCHAR2(50)")]
        public string Date { get; set; }
    }
}
