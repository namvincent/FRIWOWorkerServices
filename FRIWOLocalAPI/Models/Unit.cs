using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;
using FRIWOLocalAPI.StaticObjects;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace FRIWOLocalAPI.Models
{
   [Table("UNITS")]
    public class Unit
    {
        [Key]
        [JsonPropertyName(nameof(ID))]
        [Column(nameof(ID),TypeName ="varchar(50)")]
        public Guid ID { get; set; } = new Guid();
        [JsonPropertyName(nameof(IsTested))]
        [Column(nameof(IsTested), TypeName = "number(2)")]
        public bool IsTested { get; set; }
        [Column(nameof(Result), TypeName = "number(2)")]
        [JsonPropertyName(nameof(Result))]
        public bool Result { get; set; }
        [Column(nameof(Data), TypeName = "varchar(200)")]
        [JsonPropertyName(nameof(Data))]
        public string? Data { get; set; }

    }

}
