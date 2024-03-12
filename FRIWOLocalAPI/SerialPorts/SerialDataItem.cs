using Microsoft.AspNetCore.Components;

namespace FRIWOLocalAPI.SerialPorts
{
    public class SerialDataItem : ComponentBase
    {
        public SerialDataItem(int order, string message)
        {
            Order = order;
            Messages = message;
        }

        public int Order { get; set; }
        public string Messages { get; set; }
    }
}
