using Microsoft.AspNetCore.Components;

namespace FRIWOServerApi.Data.TRACE
{

    public class Shop_Order_Overview : ComponentBase
    {

        public string CONTRACT { get; set; }
        public string DEPARTMENT_NO { get; set; }
        public string ORDER_NO { get; set; }
        public string PART_NO { get; set; }
        public int REVISED_QTY_DUE { get; set; }
        public string OBJSTATE { get; set; }
        public string ROUTING { get; set; }
    }

    public class Shop_Order_Without_Routing : ComponentBase
    {

        public string ORDER_NO { get; set; }
        public string PART_NO { get; set; }
        public string OBJSTATE { get; set; }
        public string ORDER_CODE { get; set; }
    }
}
