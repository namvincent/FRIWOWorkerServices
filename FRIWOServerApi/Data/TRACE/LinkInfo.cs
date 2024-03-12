using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable enable

namespace FRIWOServerApi.Data.TRACE
{
    public partial class LinkInfo
    {        
        [NotMapped]
        public int ID { get; set; }
        [Column("FRIWO_BARCODE")]
        public string? InternalCode { get; set; }
        [NotMapped]
        public string? ExternalCode { get; set; }
    }
}