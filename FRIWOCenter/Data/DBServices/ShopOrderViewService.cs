using FRIWOCenter.Data.TRACE;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FRIWOCenter.DBServices
{

    public class ShopOrderViewService : ControllerBase
    {

        private readonly IDbContextFactory<TraceDbContext> _contextFactory;
        public ShopOrderViewService(IDbContextFactory<TraceDbContext> context)
        {
            _contextFactory = context;
        }



        public async Task<List<Shop_Order_Overview>> GetShopOrderData(string department)
        {

            using (var _context = await _contextFactory.CreateDbContextAsync())
            {
                List<Shop_Order_Overview> ShoporderArr = new List<Shop_Order_Overview>();
                string cond1 = "";
                string cond2 = "";
                switch (department)
                {
                    case "BB":
                        cond1 = "V3500";
                        cond2 = "E3500";
                        break;
                    case "MI":
                        cond1 = "V3150";
                        cond2 = "";
                        break;
                    case "SMD":
                        cond1 = "V3100";
                        cond2 = "V3110";
                        break;
                    default:
                        cond1 = "";
                        cond2 = "";
                        break;
                }

                var result = await _context.Shoporder_status
                //.Where(c => c.DEPARTMENT_NO == cond1 || c.DEPARTMENT_NO == cond2)
                .AsNoTracking()
                .ToListAsync();

                foreach (var item in result)
                {
                    Shop_Order_Overview data = new Shop_Order_Overview();
                    data.CONTRACT =
                        item.CONTRACT;
                    data.DEPARTMENT_NO =
                        item.DEPARTMENT_NO;
                    data.ORDER_NO =
                        item.ORDER_NO;
                    data.PART_NO =
                        item.PART_NO;
                    data.REVISED_QTY_DUE =
                        item.REVISED_QTY_DUE;
                    data.OBJSTATE =
                        item.OBJSTATE;
                    data.ROUTING =
                        item.ROUTING;
                    ShoporderArr.Add(data);
                }
                return ShoporderArr;
            }
        }

        public async Task<List<Shop_Order_Overview>> GetShopOrderDataSearch(string department, string departmentNo, string orderNo, string partNo)
        {
            using (var _context = await _contextFactory.CreateDbContextAsync())
            {
                List<Shop_Order_Overview> ShoporderArr = new List<Shop_Order_Overview>();
                string cond1 = "";
                string cond2 = "";
                switch (department)
                {
                    case "BB":
                        cond1 = "V3500";
                        cond2 = "E3500";
                        break;
                    case "MI":
                        cond1 = "V3150";
                        cond2 = "";
                        break;
                    case "SMD":
                        cond1 = "V3100";
                        cond2 = "V3110";
                        break;
                    default:
                        cond1 = "";
                        cond2 = "";
                        break;
                }

                string sqlQueryCondition = "1 = 1";

                if (orderNo != null && !orderNo.Equals(""))
                {
                    sqlQueryCondition = sqlQueryCondition + " AND order_no = '" + orderNo + "'";
                }
                if (partNo != null && !partNo.Equals(""))
                {
                    sqlQueryCondition = sqlQueryCondition + " AND part_no = '" + partNo + "'";
                }
                if (departmentNo != null && !departmentNo.Equals(""))
                {
                    sqlQueryCondition = sqlQueryCondition + " AND department_no = '" + departmentNo + "'";
                }
                else
                {
                    sqlQueryCondition = sqlQueryCondition + " AND (department_no = '" + cond1 + "' OR department_no = '" + cond2 + "')";
                }

                string sqlQuery = "SELECT * FROM V_IFS_SHOP_ORDER_STATUS WHERE " + sqlQueryCondition;
                ShoporderArr = await _context.Shoporder_status.FromSqlRaw(sqlQuery).AsNoTracking().ToListAsync();
                return ShoporderArr;
            }
        }

        public async Task<List<Shop_Order_Without_Routing>> GetShopOrderDataSearchNoRouting(string orderNo, string partNo)
        {

            using (var _context = await _contextFactory.CreateDbContextAsync())
            {
                List<Shop_Order_Without_Routing> ShoporderArr = new List<Shop_Order_Without_Routing>();

                string sqlQueryCondition = "1 = 1";

                if (orderNo != null && !orderNo.Equals(""))
                {
                    sqlQueryCondition = sqlQueryCondition + " AND order_no = '" + orderNo + "'";
                }
                if (partNo != null && !partNo.Equals(""))
                {
                    sqlQueryCondition = sqlQueryCondition + " AND part_no = '" + partNo + "'";
                }

                string sqlQuery = "SELECT * FROM V_IFS_SO_WITHOUT_ROUTING WHERE " + sqlQueryCondition;
                ShoporderArr = await _context.Shoporder_no_routing.FromSqlRaw(sqlQuery).AsNoTracking().ToListAsync();
                return ShoporderArr;
            }
        }


        public async Task<List<Shop_Order_Without_Routing>> GetShopOrderWithoutRouting()
        {
            using (var _context = await _contextFactory.CreateDbContextAsync())
            {
                List<Shop_Order_Without_Routing> ShoporderArr = new List<Shop_Order_Without_Routing>();
                var result = await _context.Shoporder_no_routing
                .AsNoTracking()
                .ToListAsync();

                foreach (var item in result)
                {
                    Shop_Order_Without_Routing data = new Shop_Order_Without_Routing();

                    data.ORDER_CODE =
                        item.ORDER_CODE;
                    data.ORDER_NO =
                        item.ORDER_NO;
                    data.PART_NO =
                        item.PART_NO;

                    data.OBJSTATE =
                        item.OBJSTATE;

                    ShoporderArr.Add(data);
                }
                return ShoporderArr;
            }
        }
    }
}
