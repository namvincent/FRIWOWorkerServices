using Microsoft.AspNetCore.Components;
using System.ComponentModel.DataAnnotations;

namespace FRIWOCenter.Data.TRACE
{
    public class GUID_HUSQVARNA 
    {
       
        public string GUID { get; set; }
        public string EXTERNAL_BARCODE { get; set; }
        public string FRIWO_BARCODE { get; set; }
        public string ORDER_NO { get; set; }
        public string PART_NO { get; set; }
        public int SENDED { get; set; }

    }
    public class UPDATE_GUID
    {
        
        public int STATUS { get; set; }
    }
}
