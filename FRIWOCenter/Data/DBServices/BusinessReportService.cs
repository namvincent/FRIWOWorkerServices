
using FRIWOCenter.Data.TRACE;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Oracle.ManagedDataAccess.Client;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FRIWOCenter.DBServices
{
    public class BusinessReportService : ControllerBase
    {
        private readonly IDbContextFactory<TraceDbContext> _contextFactory;
        public BusinessReportService(IDbContextFactory<TraceDbContext> context)
        {
            _contextFactory = context;
        }

        [HttpGet]
        [Route("api/BusinessReport/GetRevenue")]
        public async Task<IEnumerable<BusinessReport>> GetRevenueAsync()
        {
            using (var _context = await _contextFactory.CreateDbContextAsync())
            {
                var result = await _context.BusinessReport.ToListAsync();
               
                List<BusinessReport> BusinessReports = new();

                // Loop through the results
                foreach (var item in result)
                {
                    // Create a new WeatherForecast instance
                    BusinessReport ObjBusinessReport =
                        new BusinessReport();
                    // Set the values for the WeatherForecast instance
                    ObjBusinessReport.Part_no =
                        item.Part_no;
                    ObjBusinessReport.Output =
                        item.Output;
                    ObjBusinessReport.Price =
                        item.Price;
                    ObjBusinessReport.Date =
                        item.Date;
                    // Add the WeatherForecast instance to the collection
                    BusinessReports.Add(ObjBusinessReport);
                }
                // Return the final collection
                return BusinessReports;
            }
        }
    }
}
