using FRIWOServerApi.Data.TRACE;
using FRIWOServerApi.Model;
using MESystem.Data.TRACE;
using System.Collections.ObjectModel;

namespace FRIWOServerApi.Data.DBServices
{
    public interface IProcessLockService
    {
        Task<string> GetLinkInfoAsync(string barcode);
        Task<ObservableCollection<ProcessLock>> GetProcessLockStatusAsync(string barcode);
        Task<ObservableCollection<V_ROUTING_BY_PART_NO>> GetRoutingAsync(string partno, int revision);
        Task<int> GetRoutingStatusAsync(string barcode, string station);
        Task<bool> InsertHVAsync(Unit unit);
    }
}