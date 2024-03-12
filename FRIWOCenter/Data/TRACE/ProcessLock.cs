using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace FRIWOCenter.Data.TRACE
{
    [Index(nameof(Barcode), IsUnique = true)]
    public partial class ProcessLock 
    {
        [Key]
        [Column("FINAL_RESULT_ID")]
        public int ID { get; set; }

        [Column("BARCODE")]
        public string Barcode { get; set; }

        [Column("FINAL_RESULT_THROUGH_STATIONS")]
        public string FINAL_RESULT_THROUGH_STATIONS { get; set; }
    }
}