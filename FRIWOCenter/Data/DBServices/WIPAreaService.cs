using FRIWOCenter.Data.TRACE;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FRIWOCenter.DBServices
{
    public class WIPAreaService : ControllerBase
    {

        private readonly IDbContextFactory<TraceDbContext> _contextFactory;
        public WIPAreaService(IDbContextFactory<TraceDbContext> context)
        {
            _contextFactory = context;
        }
        public async Task<List<Area>>
            GetAllAsync()
        {
            using (var _context = await _contextFactory.CreateDbContextAsync())
            {
                var result = await _context.Areas
                //.Where(c => c.Area == departmentNo)
                .OrderBy(c => c.areaId)
                .AsNoTracking()
                .ToListAsync();


                // Collection to return
                List<Area> WipAreaList =
                    new List<Area>();
                // Loop through the results
                foreach (var item in result)
                {
                    // Create a new WeatherForecast instance
                    Area ObjArea =
                        new Area();
                    // Set the values for the WeatherForecast instance
                    ObjArea.areaId =
                        item.areaId;
                    ObjArea.areaDescritpion =
                        item.areaDescritpion;
                    ObjArea.sit =
                        item.sit;
                    // Add the WeatherForecast instance to the collection
                    WipAreaList.Add(ObjArea);
                }
                // Return the final collection
                return WipAreaList;
            }
        }
    }
}
