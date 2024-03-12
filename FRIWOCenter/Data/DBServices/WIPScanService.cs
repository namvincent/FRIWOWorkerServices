using FRIWOCenter.Data.TRACE;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Diagnostics;

namespace FRIWOCenter.DBServices
{
    public class WIPScanService : ControllerBase
    {
        private readonly IDbContextFactory<TraceDbContext> _contextFactory;
        public WIPScanService(IDbContextFactory<TraceDbContext> context)
        {
            _contextFactory = context;
        }

        public async void SendPalette(Palette palette)
        {
            using (var _context = await _contextFactory.CreateDbContextAsync())
            {
                try
                {
                    Debug.WriteLine(palette.paletteId);
                    CallSendPalettePackage(palette);
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                }
            }
        }
        public async void CallSendPalettePackage(Palette palette)
        {
            using (var _context = await _contextFactory.CreateDbContextAsync())
            {

                var parameters = new OracleParameter[]
            {
                new OracleParameter("p_palette_id", palette.paletteId),
                new OracleParameter("p_area_id", palette.areaId),
                new OracleParameter("p_order_no", palette.orderNo),
                new OracleParameter("p_part_no", palette.partNo),
                new OracleParameter("p_quantity", palette.quantity),
                new OracleParameter("p_status", palette.status)
            };
                string query = "BEGIN WIP_PALETTE_PKG.INSERT_PALETTE_PRC(:p_palette_id, :p_area_id, :p_order_no, :p_part_no, :p_quantity, :p_status); END;";

                var result = _context.WIPScan.FromSqlRaw(query, parameters).ToListAsync();
                Debug.WriteLine(result);
            }
        }
    }
}
