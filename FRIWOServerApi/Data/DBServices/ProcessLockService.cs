using FRIWOServerApi.Data.TRACE;
using FRIWOServerApi.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace FRIWOServerApi.Data.DBServices
{
    public class ProcessLockService
    {

        public readonly IDbContextFactory<TraceDbContext> _contextFactory;


        public ProcessLockService(IDbContextFactory<TraceDbContext> context)
        {
            _contextFactory = context;
        }
        [HttpGet]
        [Route("api/ProcessLock/CheckStatus")]
        public async Task<int>
        GetRoutingStatusAsync(string barcode, string station)
        {
            using (var _context = await _contextFactory.CreateDbContextAsync())
            {
                station = station.Replace("=", " ");
                var barcodeParam = new OracleParameter("P_BARCODE", OracleDbType.Varchar2, barcode, ParameterDirection.Input);
                var stationParam = new OracleParameter("P_STATION", OracleDbType.Varchar2, station, ParameterDirection.Input);
                var resultParam = new OracleParameter("ROUTINGSTATE", OracleDbType.Int32, ParameterDirection.Output);

                await _context.Database
                      .ExecuteSqlInterpolatedAsync($"BEGIN J_CHECK_ROUTING ({barcodeParam},{stationParam},{resultParam}); END;", default);


                return int.Parse(resultParam.Value.ToString());
            }
        }

        [HttpGet]
        [Route("api/ProcessLock/GetStatus")]
        public async Task<ObservableCollection<ProcessLock>>
            GetProcessLockStatusAsync(string barcode)
        {

            using (var _context = await _contextFactory.CreateDbContextAsync())
            {
                var result = await _context.ProcessLock
                .Where(c => c.Barcode.Contains(barcode))
                //.OrderBy(c => c.Area).ThenBy(n => n.LineDescription)
                .AsNoTracking()
                .ToListAsync();

                // Collection to return
                ObservableCollection<ProcessLock> processLocks =
                new ObservableCollection<ProcessLock>();
                // Loop through the results
                foreach (var item in result)
                {
                    // Create a new WeatherForecast instance
                    ProcessLock processLock =
                        new ProcessLock();
                    // Set the values for the WeatherForecast instance
                    processLock.Barcode =
                        item.Barcode;
                    processLock.FINAL_RESULT_THROUGH_STATIONS =
                        item.FINAL_RESULT_THROUGH_STATIONS;
                    // Add the WeatherForecast instance to the collection
                    processLocks.Add(processLock);
                }
                // Return the final collection
                return processLocks;
            }
        }

        [HttpGet]
        [Route("api/ProcessLock/GetRouting")]
        public async Task<ObservableCollection<V_ROUTING_BY_PART_NO>>
           GetRoutingAsync(string partno, int revision)
        {

            using (var _context = await _contextFactory.CreateDbContextAsync())
            {
                var result = await _context.GetTableName
                .Where(c => c.PART_NO == partno && c.REVISION == revision)
                //.OrderBy(c => c.Area).ThenBy(n => n.LineDescription)
                .AsNoTracking()
                .ToListAsync();

                // Collection to return
                ObservableCollection<V_ROUTING_BY_PART_NO> V_ROUTING_BY_PART_NOs =
                new ObservableCollection<V_ROUTING_BY_PART_NO>();
                // Loop through the results
                foreach (var item in result)
                {
                    // Create a new WeatherForecast instance
                    V_ROUTING_BY_PART_NO V_ROUTING_BY_PART_NO =
                        new V_ROUTING_BY_PART_NO();
                    // Set the values for the WeatherForecast instance
                    V_ROUTING_BY_PART_NO.ID =
                        item.ID;
                    V_ROUTING_BY_PART_NO.PART_NO =
                        item.PART_NO;
                    V_ROUTING_BY_PART_NO.REVISION =
                        item.REVISION;
                    V_ROUTING_BY_PART_NO.STATION_NAME =
                       item.STATION_NAME;

                    // Add the WeatherForecast instance to the collection
                    V_ROUTING_BY_PART_NOs.Add(V_ROUTING_BY_PART_NO);
                }

                // Return the final collection
                return V_ROUTING_BY_PART_NOs;
            }
        }

        [HttpGet]
        [Route("api/ProcessLock/LinkInfo")]
        public async Task<string> GetLinkInfoAsync(string barcode)
        {
            #region Obsolate
            //using (var _context = await _contextFactory.CreateDbContextAsync())
            //{
            //    //var linkInfo = new LinkInfo();
            //    var result = await _context.GetLinkInfos
            //        .AsNoTracking()
            //        .SingleAsync(e=>e.ExternalCode.Contains(scannedCode));
            //    //.OrderBy(c => c.Area).ThenBy(n => n.LineDescription)
            //    //if (result!=null)
            //    //{
            //    //    linkInfo.InternalCode = result.InternalCode;
            //    //    linkInfo.ExternalCode = result.ExternalCode;
            //    Debug.WriteLine($"{ result.InternalCode} <=> { result.ExternalCode}");

            //    //}

            //    // Collection to return
            //    //LinkInfo LinkInfo = new ();
            //    // Loop through the results
            //    //foreach (var item in result)
            //    //{
            //    //    // Create a new WeatherForecast instance
            //    //    V_ROUTING_BY_PART_NO V_ROUTING_BY_PART_NO =
            //    //        new V_ROUTING_BY_PART_NO();
            //    //    // Set the values for the WeatherForecast instance
            //    //    LinkInfo.ID =
            //    //        item.ID;
            //    //    LinkInfo.internalCode =
            //    //        item.internalCode;
            //    //    LinkInfo.externalCode =
            //    //        item.externalCode;

            //    //    // Add the WeatherForecast instance to the collection
            //    //    //LinkInfo.Add(V_ROUTING_BY_PART_NO);
            //    //}

            //    // Return the final collection

            //    return result;
            //}
            #endregion
            using (var _context = await _contextFactory.CreateDbContextAsync())
            {
                var barcodeParam = new OracleParameter("P_CUSTOMER_BARCODE", OracleDbType.Varchar2, barcode, ParameterDirection.Input);

                var resultParam = new OracleParameter("P_REF_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output);

                var result = await _context.GetLinkInfos.FromSqlInterpolated($"BEGIN TRACE.TRS_LINK_BARCODE_HOUSING_PKG.GET_INTERNAL_BARCODE_PRC ({barcodeParam} ,{resultParam}); END;").ToListAsync();

                //if(result.Count == 0)
                //    return "";
                if (result.Count == 0)
                    return "";

                return result?.FirstOrDefault().InternalCode?.ToString();

            }
        }

        [HttpGet]
        [Route("api/ProcessLock/InsertHVAsync")]
        public async Task<bool> InsertHVAsync(Unit unit)
        {
            using (var _context = await _contextFactory.CreateDbContextAsync())
            {
                var barcodeParam = new OracleParameter("P_BARCODE", OracleDbType.Varchar2, unit.LinkInfo.InternalCode, ParameterDirection.Input);

                var resultParam = new OracleParameter("P_STATUS", OracleDbType.Decimal, (int)unit.ResultTest, ParameterDirection.Input);

                var resultDataParam = new OracleParameter("P_RESULT_DATA", OracleDbType.Blob, System.Text.Encoding.UTF8.GetBytes(unit.DataTest), ParameterDirection.Input);

                var dateParam = new OracleParameter("P_INPUT_DATE", unit.TestTime);

                var machineParam = new OracleParameter("P_MACHINE_ID", OracleDbType.Varchar2, "JASON-PC", ParameterDirection.Input);

                var partNoParam = new OracleParameter("P_PART_NO", OracleDbType.Varchar2, unit.GetPartNoBB(), ParameterDirection.Input);

                var orderNoParam = new OracleParameter("P_ORDER_NO", OracleDbType.Varchar2, unit.GetOrderNoBB(), ParameterDirection.Input);

                var stationParam = new OracleParameter("P_ORDER_NO", OracleDbType.Varchar2, "HIGH VOLTAGE", ParameterDirection.Input);

                var result = await _context.Database.ExecuteSqlInterpolatedAsync
                    ($"BEGIN TRACE.TRS_DATA_HIGH_VOLTAGE_PKG.INSERT_DATA_HV_PRC ({barcodeParam} ,{resultParam},{resultDataParam} ,{dateParam},{machineParam} ,{partNoParam},{orderNoParam}); END;");


                result = await _context.Database.ExecuteSqlInterpolatedAsync
                    ($"BEGIN J_UPDATE_FINAL_RESULT({barcodeParam} ,{stationParam},{resultParam}); END;");

                if (result == 0)
                    return false;

                return true;
            }
        }
    }
}
