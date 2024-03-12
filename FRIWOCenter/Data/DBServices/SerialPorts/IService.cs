using Microsoft.Extensions.Logging;

namespace FRIWOCenter.DBServices.SerialPorts
{
    public interface IService
    {
        int Counter { get; set; }
        ILogger Logger { get; }
    }
}
