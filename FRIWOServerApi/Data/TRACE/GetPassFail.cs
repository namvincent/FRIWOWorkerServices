#nullable disable

using Microsoft.AspNetCore.Components;

namespace FRIWOServerApi.Data.TRACE
{
    public partial class GET_PASS_FAIL_BY_TIME : ComponentBase
    {
        public int PASS { get; set; }
        public int FAIL { get; set; }
        public int FIRST_PASS { get; set; }
        public int TOTAL_PASS { get; set; }
    }
}