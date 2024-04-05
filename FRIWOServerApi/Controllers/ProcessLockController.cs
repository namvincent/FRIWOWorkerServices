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
    public class ProcessLockController : ControllerBase
    {

        public readonly IDbContextFactory<TraceDbContext> _contextFactory;


        public ProcessLockController(IDbContextFactory<TraceDbContext> context)
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

        [HttpPost]
        [Route("api/ProcessLock/LaserTrimming/InsertPASSAsync/{unit}")]
        public async Task<ActionResult> InsertPASSAsync(string unit)
        {
            using (var _context = await _contextFactory.CreateDbContextAsync())
            {
                var contractParam = new OracleParameter("P_CONTRACT", OracleDbType.Varchar2, "7", ParameterDirection.Input);

                var releaseNoParam = new OracleParameter("P_RELEASE_NO", OracleDbType.Varchar2, "*", ParameterDirection.Input);

                var sequenceNoParam = new OracleParameter("P_SEQUENCE_NO", OracleDbType.Varchar2, "*", ParameterDirection.Input);

                var barcodeParam = new OracleParameter("P_BARCODE", OracleDbType.Varchar2, unit, ParameterDirection.Input);

                var resultParam = new OracleParameter("P_STATUS", OracleDbType.Decimal, 1, ParameterDirection.Input);

                var machineParam = new OracleParameter("P_MACHINE_ID", OracleDbType.Varchar2, "MINI-PC", ParameterDirection.Input);

                var result = await _context.Database.ExecuteSqlInterpolatedAsync
                    ($"BEGIN TRACE.TRS_DATA_LASER_TRIMMING_PKG.INSERT_DATA_LASER_TRIMMING_PRC ({contractParam} ,{releaseNoParam} ,{sequenceNoParam} ,{barcodeParam} ,{resultParam},{machineParam}); END;");


                var stationParam = new OracleParameter("P_STATION", OracleDbType.Varchar2, "LASER TRIMMING", ParameterDirection.Input);


                result = await _context.Database.ExecuteSqlInterpolatedAsync
                    ($"BEGIN J_UPDATE_FINAL_RESULT({barcodeParam} ,{stationParam},{resultParam}); END;");

                if (result == 0)
                    return BadRequest();

                return Ok();
            }
        }

        [HttpPost]
        [Route("api/ProcessLock/LaserTrimming/InsertFAILAsync/{unit}")]
        public async Task<ActionResult> InsertFAILAsync(string unit)
        {
            using (var _context = await _contextFactory.CreateDbContextAsync())
            {
                var contractParam = new OracleParameter("P_CONTRACT", OracleDbType.Varchar2, "7", ParameterDirection.Input);

                var releaseNoParam = new OracleParameter("P_RELEASE_NO", OracleDbType.Varchar2, "*", ParameterDirection.Input);

                var sequenceNoParam = new OracleParameter("P_SEQUENCE_NO", OracleDbType.Varchar2, "*", ParameterDirection.Input);

                var barcodeParam = new OracleParameter("P_BARCODE", OracleDbType.Varchar2, unit, ParameterDirection.Input);

                var resultParam = new OracleParameter("P_STATUS", OracleDbType.Decimal, 0, ParameterDirection.Input);

                var machineParam = new OracleParameter("P_MACHINE_ID", OracleDbType.Varchar2, "MINI-PC", ParameterDirection.Input);

                var result = await _context.Database.ExecuteSqlInterpolatedAsync
                    ($"BEGIN TRACE.TRS_DATA_LASER_TRIMMING_PKG.INSERT_DATA_LASER_TRIMMING_PRC ({contractParam} ,{releaseNoParam} ,{sequenceNoParam} ,{barcodeParam} ,{resultParam},{machineParam}); END;");


                var stationParam = new OracleParameter("P_STATION", OracleDbType.Varchar2, "LASER TRIMMING", ParameterDirection.Input);


                result = await _context.Database.ExecuteSqlInterpolatedAsync
                    ($"BEGIN J_UPDATE_FINAL_RESULT({barcodeParam} ,{stationParam},{resultParam}); END;");

                if (result == 0)
                    return BadRequest();

                return Ok();
            }
        }

        [HttpPost]
        [Route("api/ProcessLock/AOI/InsertPASSAOIAsync/{unit}")]
        public async Task<ActionResult> InsertPASSAOIAsync(string unit)
        {
            string[] data = unit.Split("-");
            using (var _context = await _contextFactory.CreateDbContextAsync())
            {             

                var order_No = new OracleParameter("P_ORDER_NO", OracleDbType.Varchar2, data[0], ParameterDirection.Input);

                var barcodeParam = new OracleParameter("P_BARCODE", OracleDbType.Varchar2, unit, ParameterDirection.Input);

                var resultParam = new OracleParameter("P_STATUS", OracleDbType.Decimal, 1, ParameterDirection.Input);


                var result = await _context.Database.ExecuteSqlInterpolatedAsync
                    ($"BEGIN TRACE.TRS_DATA_AOI_PKG.INSERT_DATA_AOI_API_PRC ({order_No} ,{barcodeParam} ,{resultParam}); END;");


                var stationParam = new OracleParameter("P_STATION", OracleDbType.Varchar2, "AOI", ParameterDirection.Input);


                result = await _context.Database.ExecuteSqlInterpolatedAsync
                    ($"BEGIN J_UPDATE_FINAL_RESULT({barcodeParam} ,{stationParam},{resultParam}); END;");

                if (result == 0)
                    return BadRequest();

                return Ok();
            }
        }

        [HttpPost]
        [Route("api/ProcessLock/AOI/InsertFAILAOIAsync/{unit}")]
        public async Task<ActionResult> InsertFAILAOIAsync(string unit)
        {
            string[] data = unit.Split("-");
            using (var _context = await _contextFactory.CreateDbContextAsync())
            {
                var order_No = new OracleParameter("P_ORDER_NO", OracleDbType.Varchar2, data[0], ParameterDirection.Input);

                var barcodeParam = new OracleParameter("P_BARCODE", OracleDbType.Varchar2, unit, ParameterDirection.Input);

                var resultParam = new OracleParameter("P_STATUS", OracleDbType.Decimal, 0, ParameterDirection.Input);

                var result = await _context.Database.ExecuteSqlInterpolatedAsync
                    ($"BEGIN TRACE.TRS_DATA_AOI_PKG.INSERT_DATA_AOI_API_PRC ({order_No}  ,{barcodeParam} ,{resultParam}); END;");


                var stationParam = new OracleParameter("P_STATION", OracleDbType.Varchar2, "AOI", ParameterDirection.Input);


                result = await _context.Database.ExecuteSqlInterpolatedAsync
                    ($"BEGIN J_UPDATE_FINAL_RESULT({barcodeParam} ,{stationParam},{resultParam}); END;");

                if (result == 0)
                    return BadRequest();

                return Ok();
            }
        }

        [HttpPost]
        [Route("api/ProcessLock/AOI/InsertWeldingAsync/{unit}/{status}")]
        public async Task<ActionResult> InsertWeldingAsync(string unit,int status)
        {
            string[] data = unit.Split("-");
            using (var _context = await _contextFactory.CreateDbContextAsync())
            {

                var contractParam = new OracleParameter("P_CONTRACT", OracleDbType.Varchar2, "7", ParameterDirection.Input);

                var releaseNoParam = new OracleParameter("P_RELEASE_NO", OracleDbType.Varchar2, "*", ParameterDirection.Input);

                var sequenceNoParam = new OracleParameter("P_SEQUENCE_NO", OracleDbType.Varchar2, "*", ParameterDirection.Input);

                var barcodeParam = new OracleParameter("P_BARCODE", OracleDbType.Varchar2, unit, ParameterDirection.Input);

                var resultParam = new OracleParameter("P_STATUS", OracleDbType.Decimal, status, ParameterDirection.Input);

                var machineParam = new OracleParameter("P_MACHINE_ID", OracleDbType.Varchar2, "MINI-PC", ParameterDirection.Input);


                var result = await _context.Database.ExecuteSqlInterpolatedAsync
                    ($"BEGIN TRACE.TRS_DATA_WELDING_PKG.INSERT_DATA_WELDING_API_PRC ({contractParam} ,{releaseNoParam} ,{sequenceNoParam} ,{barcodeParam} ,{resultParam},{machineParam}); END;");


                var stationParam = new OracleParameter("P_STATION", OracleDbType.Varchar2, "WELDING", ParameterDirection.Input);


                result = await _context.Database.ExecuteSqlInterpolatedAsync
                    ($"BEGIN J_UPDATE_FINAL_RESULT({barcodeParam} ,{stationParam},{resultParam}); END;");

                if (result == 0)
                    return BadRequest();

                return Ok();
            }
        }

        [HttpPost]
        [Route("api/ProcessLock/FA/InsertVauumAsync/{unit}/{status}")]
        public async Task<ActionResult> InsertVacuumAsync(string unit, int status)
        {
            string[] data = unit.Split("-");
            using (var _context = await _contextFactory.CreateDbContextAsync())
            {

                var contractParam = new OracleParameter("P_CONTRACT", OracleDbType.Varchar2, "7", ParameterDirection.Input);

                var releaseNoParam = new OracleParameter("P_RELEASE_NO", OracleDbType.Varchar2, "*", ParameterDirection.Input);

                var sequenceNoParam = new OracleParameter("P_SEQUENCE_NO", OracleDbType.Varchar2, "*", ParameterDirection.Input);

                var barcodeParam = new OracleParameter("P_BARCODE", OracleDbType.Varchar2, unit, ParameterDirection.Input);

                var resultParam = new OracleParameter("P_STATUS", OracleDbType.Decimal, status, ParameterDirection.Input);

                var machineParam = new OracleParameter("P_MACHINE_ID", OracleDbType.Varchar2, "MINI-PC", ParameterDirection.Input);


                var result = await _context.Database.ExecuteSqlInterpolatedAsync
                    ($"BEGIN TRACE.TRS_DATA_VACUUM_TEST_PKG.INSERT_DATA_VACUUM_API_PRC ({contractParam} ,{releaseNoParam} ,{sequenceNoParam} ,{barcodeParam} ,{resultParam},{machineParam}); END;");


                var stationParam = new OracleParameter("P_STATION", OracleDbType.Varchar2, "VACUUM STATION", ParameterDirection.Input);


                result = await _context.Database.ExecuteSqlInterpolatedAsync
                    ($"BEGIN J_UPDATE_FINAL_RESULT({barcodeParam} ,{stationParam},{resultParam}); END;");

                if (result == 0)
                    return BadRequest();

                return Ok();
            }
        }

        [HttpPost]
        [Route("api/ProcessLock/FA/GetLinkData")]
        public async Task<ActionResult> GetLinkData([FromBody]string unit)
        {
            string resultData = "";
            using (var _context = await _contextFactory.CreateDbContextAsync())
            {


                var barcodeParam = new OracleParameter("P_BARCODE", OracleDbType.Varchar2,500, unit, ParameterDirection.Input);

                var resultParam = new OracleParameter("P_STATUS", OracleDbType.Varchar2,500, resultData, ParameterDirection.Output);


                var result = await _context.Database.ExecuteSqlInterpolatedAsync
                    ($"BEGIN GET_INTERNAL_CODE_LINK ({barcodeParam} ,{resultParam}); END;");



                resultData = resultParam.Value.ToString();



                if (result == 0)
                    return BadRequest(resultData);

                return Ok(resultData);
            }
        }

        [HttpPost]
        [Route("api/ProcessLock/FA/CheckPreviousStation/{unit}/{station}")]
        public async Task<ActionResult> CheckPreviousStation(string unit,string station)
        {
            string[] data = unit.Split("-");
            string resultData = "";
            using (var _context = await _contextFactory.CreateDbContextAsync())
            {


                var barcodeParam = new OracleParameter("P_SERIAL", OracleDbType.Varchar2, 500, unit, ParameterDirection.Input);
                var state = new OracleParameter("P_BARCODE", OracleDbType.Varchar2, 500, station, ParameterDirection.Input);
                var resultParam = new OracleParameter("P_STATUS", OracleDbType.Varchar2, 500, resultData, ParameterDirection.Output);


                var result = await _context.Database.ExecuteSqlInterpolatedAsync
                    ($"BEGIN CHECK_PREVIOUS_STATION ({barcodeParam},{state} ,{resultParam}); END;");



                resultData = resultParam.Value.ToString();



                if (result == 0)
                    return BadRequest(resultData);

                return Ok(resultData);
            }
        }

        [HttpPost]
        [Route("api/ProcessLock/FA/GetAdditionalData")]
        public async Task<ActionResult> GetAdditionalData([FromBody] string unit)
        {            
            string resultData = "";
            using (var _context = await _contextFactory.CreateDbContextAsync())
            {


                var line_id = new OracleParameter("P_LINE", OracleDbType.Varchar2, 500, unit, ParameterDirection.Input);

                var resultParam = new OracleParameter("P_OUTPUT", OracleDbType.Varchar2, 500, resultData, ParameterDirection.Output);


                var result = await _context.Database.ExecuteSqlInterpolatedAsync
                    ($"BEGIN GET_ADDITIONAL_DATA ({line_id} ,{resultParam}); END;");



                resultData = resultParam.Value.ToString();



                if (result == 0)
                    return BadRequest(resultData);

                return Ok(resultData);
            }
        }

        [HttpPost]
        [Route("api/ProcessLock/AOI/InsertLeakageCurrentAsync/")]
        public async Task<ActionResult> InsertLeakageCurrentAsync([FromBody] string unit, [FromBody]int status, [FromBody]string machineID)
        {
            string[] data = unit.Split("-");
            using (var _context = await _contextFactory.CreateDbContextAsync())
            {
                var P_BARCODE = new OracleParameter("P_BARCODE", OracleDbType.Varchar2, unit, ParameterDirection.Input);

                var P_STATUS = new OracleParameter("P_STATUS", OracleDbType.Int32, status, ParameterDirection.Input);

                var P_MACHINE_ID = new OracleParameter("P_MACHINE_ID", OracleDbType.Varchar2, machineID, ParameterDirection.Input);

                var result = await _context.Database.ExecuteSqlInterpolatedAsync
                    ($"BEGIN TRACE.TRS_DATA_LEAKAGE_PRC.TRS_DATA_LEAKAGE_CURRENT_PRC ({P_BARCODE}  ,{P_STATUS} ,{P_MACHINE_ID}); END;");            



                if (result == 0)
                    return BadRequest();

                return Ok();
            }
        }
    }
}
