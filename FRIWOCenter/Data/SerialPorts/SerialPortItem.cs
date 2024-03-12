using System.Text.Json.Serialization;

namespace FRIWOCenter.Data.SerialPorts
{
    public class SerialPortItem
    {
        [JsonPropertyName(nameof(Name))]
        public string? Name
        {
            get; set;
        }
        [JsonPropertyName(nameof(IsOpened))]
        public bool IsOpened { get; set; }


    }
}
