using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace FRIWOCenter.Model
{
    [Table("UNITS")]
    public class UnitDTO
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
