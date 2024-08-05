using FRIWOServerApi.Data.TRACE;
using FRIWOServerApi.Model;
using MESystem.Data.TRACE;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
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
        public async Task<ActionResult> InsertWeldingAsync(string unit, int status)
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
        public async Task<ActionResult> GetLinkData([FromBody] string unit)
        {
            string resultData = "";
            using (var _context = await _contextFactory.CreateDbContextAsync())
            {


                var barcodeParam = new OracleParameter("P_BARCODE", OracleDbType.Varchar2, 500, unit, ParameterDirection.Input);

                var resultParam = new OracleParameter("P_STATUS", OracleDbType.Varchar2, 500, resultData, ParameterDirection.Output);


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
        public async Task<ActionResult> CheckPreviousStation(string unit, string station)
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

        public class DataTest
        {
            public string LowerLimit { get; set; }
            public string Weight { get; set; }
            public string UpperLimit { get; set; }
        }

        [HttpPost]
        [Route("api/ProcessLock/FA/InsertPottingAsync/{unit}/{status}/{machine}/{result}")]
        public async Task<ActionResult> InsertPottingAsync(string unit, int status, string machine, string result)
        {
            string[] resultL = result.Split("-");
            var dataR = new DataTest();
            dataR.LowerLimit = resultL[0].ToString();
            dataR.Weight = resultL[1].ToString();
            dataR.UpperLimit = resultL[2].ToString();
            string resultData = "";
            var convertjson = JsonConvert.SerializeObject(dataR);
            byte[] blobData = System.Text.Encoding.UTF8.GetBytes(convertjson);

            using (var _context = await _contextFactory.CreateDbContextAsync())
            {

                var barcodeParam = new OracleParameter("P_BARCODE", OracleDbType.Varchar2, unit, ParameterDirection.Input);
                var resultParam = new OracleParameter("P_STATUS", OracleDbType.Decimal, status, ParameterDirection.Input);
                var machineParam = new OracleParameter("P_MACHINE_ID", OracleDbType.Varchar2, machine, ParameterDirection.Input);
                var result_data = new OracleParameter("P_RESULT_DATA", OracleDbType.Blob, blobData, ParameterDirection.Input);
                var outputParam = new OracleParameter("P_OUTPUT", OracleDbType.Varchar2, 500, resultData, ParameterDirection.Output);


                var resultset = await _context.Database.ExecuteSqlInterpolatedAsync
                    ($"BEGIN INSERT_POTTING_DATA ({barcodeParam} ,{machineParam},{resultParam},{result_data},{outputParam}); END;");

                resultData = outputParam.Value.ToString();
                if (resultData.Equals("1"))
                {
                    resultData = "1";
                }
                else
                {
                    resultData = "-1";
                }

                if (resultset == 0)
                    return BadRequest(resultData);

                return Ok(resultData);
            }
        }

        [HttpPost]
        [Route("api/ProcessLock/FA/InsertPottingDataAsync/{unit}/{status}/{machineID}/{result}")]
        public async Task<ActionResult> InsertPottingDataAsync(string unit, int status, string machineID, string result)
        {
            List<PottingData> currentData = await GetPottingScaleDataAsync(unit, "");
            var tempData = currentData.Where(e => e.Station == "Potting").FirstOrDefault();
            currentData.Remove(tempData);
            string[] resultL = result.Split("-");
            PottingData dataR = new PottingData
            {
                Station = "Potting",
                LowerLimit = resultL[0].ToString(),
                Weight = resultL[1].ToString(),
                UpperLimit = resultL[2].ToString()
            };
            currentData.Add(dataR);
            string resultData = "";
            var convertjson = JsonConvert.SerializeObject(currentData);
            byte[] blobData = System.Text.Encoding.UTF8.GetBytes(convertjson);

            using (var _context = await _contextFactory.CreateDbContextAsync())
            {

                OracleParameter barcode = new("P_BARCODE", OracleDbType.Varchar2, 100, unit, ParameterDirection.Input);
                OracleParameter machine = new("P_MACHINE", OracleDbType.Varchar2, 100, machineID, ParameterDirection.Input);
                OracleParameter Status = new("P_STATUS", OracleDbType.Int32, 100, status, ParameterDirection.Input);
                OracleParameter result_scale = new("P_RESULT_DATA", OracleDbType.Blob, blobData, ParameterDirection.Input);
                OracleParameter field = new("P_PARAMETER", OracleDbType.Varchar2, 100, "", ParameterDirection.Input);
                OracleParameter values = new("P_VALUE", OracleDbType.Varchar2, 100, "", ParameterDirection.Input);
                OracleParameter outputParam = new("P_RESULT", OracleDbType.Int32, 100, ParameterDirection.Output);

                var resultset = await _context.Database.ExecuteSqlInterpolatedAsync
                    ($"BEGIN INSERT_POTTING_DATA_SEPARATE ({barcode} ,{machine},{Status},{result_scale},{field},{values},{outputParam}); END;");

                resultData = outputParam.Value.ToString();
                if (resultData.Equals("1"))
                {
                    resultData = "1";
                }
                else
                {
                    resultData = "-1";
                }

                if (resultset == 0)
                    return BadRequest(resultData);

                return Ok(resultData);
            }
        }

        public async Task<List<PottingData>> GetPottingScaleDataAsync(string barcode, string result)
        {
            var pottingDatas = new List<PottingData>();
            var data = "";
            string? resultString = "";
            OracleParameter serial = new("P_BARCODE", OracleDbType.NVarchar2, 200, barcode, ParameterDirection.Input);
            OracleParameter state = new("P_RESULT_DATA", OracleDbType.NVarchar2, 200, result, ParameterDirection.Input);
            OracleParameter output = new("P_OUT", OracleDbType.NVarchar2, 2000, resultString, ParameterDirection.Output);
            using (var context = _contextFactory.CreateDbContext())
            {

                OracleConnection? conn = new(context.Database.GetConnectionString());
                var query = "TRACE.P_POTTING_SCALE_DATA";
                conn.Open();
                if (conn.State == ConnectionState.Open)
                {
                    using var command = conn.CreateCommand();
                    command.CommandText = query;
                    command.CommandType = CommandType.StoredProcedure;
                    _ = command.Parameters.Add(serial);
                    _ = command.Parameters.Add(state);
                    _ = command.Parameters.Add(output);
                    command.Connection = conn;
                    var reader = command.ExecuteReader();
                    resultString = output.Value.ToString();

                    try
                    {
                        var rs = System.Text.Json.JsonSerializer.Deserialize<List<PottingData>>(resultString.ToString());
                        pottingDatas.AddRange(rs);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.ToString());
                    }
                    reader.Dispose();
                    command.Dispose();
                }
                conn.Dispose();
            }
            return pottingDatas;
        }

        [HttpPost]
        [Route("api/ProcessLock/FA/GetPrePottingData/{barcode}")]
        public async Task<ActionResult> CheckPrePottingData(string barcode)
        {

            string resultData = "";
            using (var _context = await _contextFactory.CreateDbContextAsync())
            {


                var barcodeParam = new OracleParameter("P_BARCODE", OracleDbType.Varchar2, 500, barcode, ParameterDirection.Input);
                var resultParam = new OracleParameter("P_OUT", OracleDbType.Varchar2, 500, resultData, ParameterDirection.Output);


                var result = await _context.Database.ExecuteSqlInterpolatedAsync
                    ($"BEGIN P_GET_PRE_POTTING_DATA ({barcodeParam} ,{resultParam}); END;");



                resultData = resultParam.Value.ToString();



                if (result == 0)
                    return BadRequest(resultData);

                return Ok(resultData);
            }
        }

        [HttpPost]
        [Route("api/ProcessLock/FA/InsertPCBHeightCheckAsync/{unit}/{status}/{machine}/{data}")]
        public async Task<ActionResult> InsertPCBAHeightCheckAsync(string unit, int status, string machine, string data)
        {
            List<PCBHeightCheck> heightCheckList = new List<PCBHeightCheck>();
            string[] dataArray = data.Split("-");
            PCBHeightCheck item = new PCBHeightCheck
            {
                Limit = dataArray[0] + " mm",
                Point1 = dataArray[1] + " mm",
                Point2 = dataArray[2] + " mm"
            };
            heightCheckList.Add(item);
            var convertjson = JsonConvert.SerializeObject(heightCheckList);
            byte[] blobData = System.Text.Encoding.UTF8.GetBytes(convertjson);
            using (var _context = await _contextFactory.CreateDbContextAsync())
            {

                var contractParam = new OracleParameter("P_CONTRACT", OracleDbType.Varchar2, "7", ParameterDirection.Input);

                var barcodeParam = new OracleParameter("P_BARCODE", OracleDbType.Varchar2, unit, ParameterDirection.Input);

                var resultParam = new OracleParameter("P_STATUS", OracleDbType.Decimal, status, ParameterDirection.Input);

                var machineParam = new OracleParameter("P_MACHINE_ID", OracleDbType.Varchar2, machine, ParameterDirection.Input);
                OracleParameter resultMeasure = new("P_RESULT_DATA", OracleDbType.Blob, blobData, ParameterDirection.Input);

                var result = await _context.Database.ExecuteSqlInterpolatedAsync
                    ($"BEGIN TRACE.TRS_DATA_PCBA_HEIGHT_CHECK_PKG.INSERT_DATA_HEIGHT_CHECK_PRC ({contractParam},{barcodeParam} ,{resultParam},{machineParam},{resultMeasure}); END;");


                var stationParam = new OracleParameter("P_STATION", OracleDbType.Varchar2, "PCBA HEIGHT CHECK", ParameterDirection.Input);


                result = await _context.Database.ExecuteSqlInterpolatedAsync
                    ($"BEGIN J_UPDATE_FINAL_RESULT({barcodeParam} ,{stationParam},{resultParam}); END;");

                if (result == 0)
                    return BadRequest();

                return Ok();
            }
        }

        [HttpPost]
        [Route("api/ProcessLock/MI/CheckDuplicateLinkPanel/{flag}/{barcode}")]
        public async Task<ActionResult> CheckDuplicateBoardBarcode(string flag, string barcode)
        {
            string resultData = "";
            using (var _context = await _contextFactory.CreateDbContextAsync())
            {

                var flagParam = new OracleParameter("P_FLAG", OracleDbType.Varchar2, 500, flag, ParameterDirection.Input);
                var barcodeParam = new OracleParameter("P_BARCODE", OracleDbType.Varchar2, 500, barcode, ParameterDirection.Input);

                var resultParam = new OracleParameter("V_RESULT", OracleDbType.Varchar2, 500, resultData, ParameterDirection.Output);


                var result = await _context.Database.ExecuteSqlInterpolatedAsync
                    ($"BEGIN TRACE.TRS_LINK_BARCODE_PANEL_PKG.CHECK_DUPLICATE_BARCODE_FCN ({flagParam},{barcodeParam} ,{resultParam}); END;");



                resultData = resultParam.Value.ToString();



                if (result == 0)
                    return BadRequest(resultData);

                return Ok(resultData);
            }
        }

        [HttpPost]
        [Route("api/ProcessLock/MI/GetNoSide/{orderNo}")]
        public async Task<ActionResult> GetNoSide(string orderNo)
        {
            string resultData = "";
            string sideQTY = "";
            string pcbQTY = "";
            using (var _context = await _contextFactory.CreateDbContextAsync())
            {

                var orderParam = new OracleParameter("P_ORDER_NO", OracleDbType.Varchar2, 500, orderNo, ParameterDirection.Input);

                var sideParam = new OracleParameter("P_SIDE", OracleDbType.Int32, 500, sideQTY, ParameterDirection.Output);
                var pcbParam = new OracleParameter("P_QTY_PCB", OracleDbType.Int32, 500, pcbQTY, ParameterDirection.Output);

                var result = await _context.Database.ExecuteSqlInterpolatedAsync
                    ($"BEGIN TRACE.TRS_LINK_BARCODE_PANEL_PKG.GET_SIDE_QUANTITY_PRC ({orderParam},{sideParam},{pcbParam}); END;");



                resultData = sideParam.Value.ToString() + ";" + pcbParam.Value.ToString();



                if (result == 0)
                    return BadRequest(resultData);

                return Ok(resultData);
            }
        }

        [HttpPost]
        [Route("api/ProcessLock/MI/GetQTYPCB/{barcode}")]
        public async Task<ActionResult> GetQTYPCB(string barcode)
        {
            string resultData = "";
            using (var _context = await _contextFactory.CreateDbContextAsync())
            {

                var barcodeParam = new OracleParameter("P_BARCODE", OracleDbType.Varchar2, 500, barcode, ParameterDirection.Input);

                var resultParam = new OracleParameter("P_OUT", OracleDbType.Int32, 500, resultData, ParameterDirection.Output);


                var result = await _context.Database.ExecuteSqlInterpolatedAsync
                    ($"BEGIN TRACE.TRS_SEMI_GOOD_SMD_PKG.GET_PANEL_QUANTITY_PRC ({barcodeParam},{resultParam}); END;");



                resultData = resultParam.Value.ToString();



                if (result == 0)
                    return BadRequest(resultData);

                return Ok(resultData);
            }
        }

        [HttpPost]
        [Route("api/ProcessLock/MI/CheckPreviousSMDStation/{unit}/{station}")]
        public async Task<ActionResult> CheckPreviousSMDStation(string unit, string station)
        {
            string[] data = unit.Split("-");
            string resultData = "";
            using (var _context = await _contextFactory.CreateDbContextAsync())
            {


                var barcodeParam = new OracleParameter("P_SERIAL", OracleDbType.Varchar2, 500, unit, ParameterDirection.Input);
                var state = new OracleParameter("P_BARCODE", OracleDbType.Varchar2, 500, station, ParameterDirection.Input);
                var resultParam = new OracleParameter("P_STATUS", OracleDbType.Varchar2, 500, resultData, ParameterDirection.Output);


                var result = await _context.Database.ExecuteSqlInterpolatedAsync
                    ($"BEGIN CHECK_PREVIOUS_STATION_SMD ({barcodeParam},{state} ,{resultParam}); END;");



                resultData = resultParam.Value.ToString();



                if (result == 0)
                    return BadRequest(resultData);

                return Ok(resultData);
            }
        }

        [HttpPost]
        [Route("api/ProcessLock/FA/InsertLinkPanel/{panelBarcode}/{pcbBarcode}")]
        public async Task<ActionResult> InsertLinkPanel(string panelBarcode, string pcbBarcode)
        {
            string[] pcbBarcodeList;
            string[] panelList = panelBarcode.Split(";");
            if (panelList.Count() == 1)
            {
                panelList[1] = "";
            }
            if (pcbBarcode.Contains(";"))
            {
                pcbBarcodeList = pcbBarcode.Split(";");
            }
            else
            {
                pcbBarcodeList = new string[1];
                pcbBarcodeList[0] = pcbBarcode;
            }
            using (var _context = await _contextFactory.CreateDbContextAsync())
            {
                foreach (var item in pcbBarcodeList)
                {
                    var panel1Param = new OracleParameter("P_PANEL_BARCODE", OracleDbType.Varchar2, panelList[0], ParameterDirection.Input);

                    var panel2Param = new OracleParameter("P_PANEL_BARCODE2", OracleDbType.Varchar2, panelList[1], ParameterDirection.Input);

                    var pcbBarcodeParam = new OracleParameter("P_PCB_BARCODE", OracleDbType.Varchar2, item, ParameterDirection.Input);


                    var result = await _context.Database.ExecuteSqlInterpolatedAsync
                        ($"BEGIN TRACE.TRS_LINK_BARCODE_PANEL_PKG.INSERT_LINK_BARCODE_PRC ({panel1Param},{panel2Param},{pcbBarcodeParam}); END;");


                    //var stationParam = new OracleParameter("P_STATION", OracleDbType.Varchar2, "PCBA HEIGHT CHECK", ParameterDirection.Input);


                    //result = await _context.Database.ExecuteSqlInterpolatedAsync
                    //    ($"BEGIN J_UPDATE_FINAL_RESULT({barcodeParam} ,{stationParam},{resultParam}); END;");
                    if (result == 0)
                        return BadRequest();
                }

                return Ok();
            }
        }

        [HttpPost]
        [Route("api/ProcessLock/Cable/InsertCableDimensionAsync/{unit}/{status}/{machine}/{data}")]
        public async Task<ActionResult> InsertCableDimensionCheckAsync(string unit, int status, string machine, string data)
        {
            //List<PCBHeightCheck> heightCheckList = new List<PCBHeightCheck>();
            //string[] dataArray = data.Split("-");
            //PCBHeightCheck item = new PCBHeightCheck
            //{
            //    Limit = dataArray[0] + " mm",
            //    Point1 = dataArray[1] + " mm",
            //    Point2 = dataArray[2] + " mm"
            //};
            //heightCheckList.Add(item);
            //var convertjson = JsonConvert.SerializeObject(heightCheckList);
            byte[] blobData = null;
            using (var _context = await _contextFactory.CreateDbContextAsync())
            {

                var contractParam = new OracleParameter("P_CONTRACT", OracleDbType.Varchar2, "7", ParameterDirection.Input);

                var barcodeParam = new OracleParameter("P_BARCODE", OracleDbType.Varchar2, unit, ParameterDirection.Input);

                var resultParam = new OracleParameter("P_STATUS", OracleDbType.Decimal, status, ParameterDirection.Input);

                var machineParam = new OracleParameter("P_MACHINE_ID", OracleDbType.Varchar2, machine, ParameterDirection.Input);
                OracleParameter resultMeasure = new("P_RESULT_DATA", OracleDbType.Blob, blobData, ParameterDirection.Input);

                var result = await _context.Database.ExecuteSqlInterpolatedAsync
                    ($"BEGIN TRACE.TRS_DATA_CABLE_DIMENSION_CHECK.INSERT_CABLE_DIMENSION_PRC ({contractParam},{barcodeParam} ,{resultParam},{machineParam},{resultMeasure}); END;");



                if (result == 0)
                    return BadRequest();

                return Ok();
            }
        }

        [HttpPost]
        [Route("api/ProcessLock/FA/InsertHVDataAsync/{barcode}/{status}/{machineID}/{result}")]
        public async Task<ActionResult> InsertHighVoltageDataAsync(string barcode, int status, string machineID, string result)
        {
            string resultData = "";
            byte[] blobData = null;
            using (var _context = await _contextFactory.CreateDbContextAsync())
            {

                OracleParameter P_BARCODE = new("P_BARCODE", OracleDbType.Varchar2, 100, barcode, ParameterDirection.Input);
                OracleParameter machine = new("P_MACHINE", OracleDbType.Varchar2, 100, machineID, ParameterDirection.Input);
                OracleParameter Status = new("P_STATUS", OracleDbType.Int32, 100, status, ParameterDirection.Input);
                OracleParameter result_scale = new("P_RESULT_DATA", OracleDbType.Blob, blobData, ParameterDirection.Input);
                OracleParameter outputParam = new("P_RESULT", OracleDbType.Int32, 100, ParameterDirection.Output);

                var resultset = await _context.Database.ExecuteSqlInterpolatedAsync
                    ($"BEGIN INSERT_HIGH_VOLTAGE ({P_BARCODE} ,{machine},{Status},{result_scale},{outputParam}); END;");

                resultData = outputParam.Value.ToString();
                if (resultData.Equals("1"))
                {
                    resultData = "1";
                }
                else
                {
                    resultData = "-1";
                }

                if (resultset == 0)
                    return BadRequest(resultData);

                return Ok(resultData);
            }
        }

        [HttpPost]
        [Route("api/ProcessLock/TR/InsertFTTransformerDataAsync/{barcode}/{order}/{status}/{machineID}/{result}")]
        public async Task<ActionResult> InsertFTTransformerDataAsync(string barcode, string order, int status, string machineID, string result)
        {
            string resultData = "";
            byte[] blobData = null;
            using (var _context = await _contextFactory.CreateDbContextAsync())
            {

                OracleParameter P_BARCODE = new("P_BARCODE", OracleDbType.Varchar2, 100, barcode, ParameterDirection.Input);
                OracleParameter P_ORDER_NO = new("P_ORDER_NO", OracleDbType.Varchar2, 100, order, ParameterDirection.Input);
                OracleParameter machine = new("P_MACHINE", OracleDbType.Varchar2, 100, machineID, ParameterDirection.Input);
                OracleParameter Status = new("P_STATUS", OracleDbType.Int32, 100, status, ParameterDirection.Input);
                OracleParameter result_scale = new("P_RESULT_DATA", OracleDbType.Blob, blobData, ParameterDirection.Input);
                OracleParameter outputParam = new("P_RESULT", OracleDbType.Int32, 100, ParameterDirection.Output);

                var resultset = await _context.Database.ExecuteSqlInterpolatedAsync
                    ($"BEGIN INSERT_FINAL_TEST_TRANSFORMER ({P_BARCODE},{P_ORDER_NO} ,{machine},{Status},{result_scale},{outputParam}); END;");

                resultData = outputParam.Value.ToString();
                if (resultData.Equals("1"))
                {
                    resultData = "1";
                }
                else
                {
                    resultData = "-1";
                }

                if (resultset == 0)
                    return BadRequest(resultData);

                return Ok(resultData);
            }
        }

        [HttpPost]
        [Route("api/ProcessLock/TR/GetPartNoTransformer")]
        public async Task<ActionResult> GetAddGetPartNoTransformeritionalData([FromBody] string order)
        {
            string resultData = "";
            using (var _context = await _contextFactory.CreateDbContextAsync())
            {


                var P_ORDER_NO = new OracleParameter("P_ORDER_NO", OracleDbType.Varchar2, 500, order, ParameterDirection.Input);

                var P_ORDER_INFO = new OracleParameter("P_ORDER_INFO", OracleDbType.Varchar2, 500, resultData, ParameterDirection.Output);


                var result = await _context.Database.ExecuteSqlInterpolatedAsync
                    ($"BEGIN GET_ORDER_TRANSFORMER_INFO ({P_ORDER_NO} ,{P_ORDER_INFO}); END;");



                resultData = P_ORDER_INFO.Value.ToString();



                if (result == 0)
                    return BadRequest(resultData);

                return Ok(resultData);
            }
        }


        [HttpPost]
        [Route("api/ProcessLock/MI/InsertComponentLink/{pcbBarcode}/{id}/{lotBatch}/{type}")]
        public async Task<ActionResult> InsertComponentLinkAsync(string pcbBarcode, string id, string lotBatch, string type)
        {
            try
            {
                List<ComponentUsed> currentData = await GetComponentLinkData(pcbBarcode);
                //var tempData = currentData.Where(e => e.Station == "Potting").FirstOrDefault();
                //currentData.Remove(tempData);
                string[] resultL = id.Split("@@");
                foreach (var item in resultL)
                {
                    ComponentUsed dataR = new ComponentUsed
                    {
                        Type = type,
                        LotBatch = lotBatch,
                        Id = item
                    };
                    currentData.Add(dataR);
                }
                string resultData = "";
                var convertjson = JsonConvert.SerializeObject(currentData);
                byte[] blobData = System.Text.Encoding.UTF8.GetBytes(convertjson);

                using (var _context = await _contextFactory.CreateDbContextAsync())
                {
                    OracleParameter P_COMPONENT = new("P_COMPONENT", OracleDbType.Varchar2, convertjson, ParameterDirection.Input);
                    OracleParameter P_PCB_BARCODE = new("P_PCB_BARCODE", OracleDbType.Varchar2, 100, pcbBarcode, ParameterDirection.Input);

                    var resultset = await _context.Database.ExecuteSqlInterpolatedAsync
                        ($"BEGIN P_INSERT_COMPONENT_USES ({P_PCB_BARCODE} ,{P_COMPONENT}); END;");


                    resultData = "1";

                    if (resultset == 0)
                        return BadRequest(resultData);

                    return Ok(resultData);
                }
            }
            catch (Exception ex) {
                return BadRequest(-1);
            }
        }

        public async Task<List<ComponentUsed>> GetComponentLinkData(string barcode)
        {
            var componentDatas = new List<ComponentUsed>();
            var data = "";
            string? resultString = "";
            OracleParameter P_PCB_BARCODE = new("P_PCB_BARCODE", OracleDbType.NVarchar2, 200, barcode, ParameterDirection.Input);
            OracleParameter P_OUT = new("P_OUT", OracleDbType.NVarchar2, 2000, resultString, ParameterDirection.Output);
            using (var context = _contextFactory.CreateDbContext())
            {

                OracleConnection? conn = new(context.Database.GetConnectionString());
                var query = "TRACE.P_GET_COMPONENT_LINK";
                conn.Open();
                if (conn.State == ConnectionState.Open)
                {
                    using var command = conn.CreateCommand();
                    command.CommandText = query;
                    command.CommandType = CommandType.StoredProcedure;
                    _ = command.Parameters.Add(P_PCB_BARCODE);
                    _ = command.Parameters.Add(P_OUT);
                    command.Connection = conn;
                    var reader = command.ExecuteReader();
                    resultString = P_OUT.Value.ToString();

                    try
                    {
                        var rs = System.Text.Json.JsonSerializer.Deserialize<List<ComponentUsed>>(resultString.ToString());
                        componentDatas.AddRange(rs);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.ToString());
                    }
                    reader.Dispose();
                    command.Dispose();
                }
                conn.Dispose();
            }
            return componentDatas;
        }

        [HttpPost]
        [Route("api/ProcessLock/MI/CheckExistComponentLinkData/{barcode}")]
        public async Task<ActionResult> CheckExistComponentLinkData(string barcode)
        {
            string resultData = "";
            using (var _context = await _contextFactory.CreateDbContextAsync())
            {

                var P_PCB_BARCODE = new OracleParameter("P_PCB_BARCODE", OracleDbType.Varchar2, 500, barcode, ParameterDirection.Input);

                var P_OUT = new OracleParameter("P_OUT", OracleDbType.Varchar2, 500, resultData, ParameterDirection.Output);


                var result = await _context.Database.ExecuteSqlInterpolatedAsync
                    ($"BEGIN TRACE.P_COMPONENT_LINK ({P_PCB_BARCODE},{P_OUT}); END;");



                resultData = P_OUT.Value.ToString();



                if (result == 0)
                    return BadRequest(resultData);

                return Ok(resultData);
            }
        }
    }
}
