using Microsoft.AspNetCore.Components;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FRIWOCenter.Data.TRACE
{
    public class Husqvarna_Json
    {
      
        [Column("BODY")]
        public string BODY { get; set; }

       
        [Column("BARCODE")]
        public string BARCODE { get; set; }
    }
}