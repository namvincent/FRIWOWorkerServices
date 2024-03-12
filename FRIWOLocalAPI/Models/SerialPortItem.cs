using FRIWOLocalAPI.SerialPorts;
using System.IO.Ports;
using System.Text.Json.Serialization;

namespace FRIWOLocalAPI.Models
{
    public class SerialPortItem
    {
        [JsonPropertyName(nameof(Name))]
        public string? Name
        {
            get; set;
        }
        [JsonPropertyName(nameof(IsOpened))]
        public bool IsOpened => PortIsOpen(Name);
        

        private bool PortIsOpen(string? name)
        {
            if(name != null)
            {
                var SerialPort = new SerialPort();
                SerialPort.PortName = name;
                return SerialPort.IsOpen;
                
            }
            else
            {
                return false;
            }
            
        }
    }
}
