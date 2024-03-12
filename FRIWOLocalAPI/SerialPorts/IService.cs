using FRIWOLocalAPI.Models;
using Microsoft.Extensions.Logging;

namespace FRIWOLocalAPI.SerialPorts
{
    public interface IService
    {
        int Counter { get; set; }
        ILogger Logger { get; }

        public Task<List<Unit>> SendData(List<Unit> units);
    }
}
