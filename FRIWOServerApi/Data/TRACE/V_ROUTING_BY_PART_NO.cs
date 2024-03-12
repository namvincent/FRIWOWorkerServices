using System.ComponentModel.DataAnnotations.Schema;

#nullable enable

namespace FRIWOServerApi.Data.TRACE
{
    public partial class V_ROUTING_BY_PART_NO
    {
        [Column("ID")]
        public int ID { get; set; }
        public string? PART_NO { get; set; }
        public string? STATION_NAME { get; set; }
        public string? TABLE_NAME { get; set; }
        public int DISPLAY_ORDER_STATUS { get; set; }
        public int REVISION { get; set; }
    }
}