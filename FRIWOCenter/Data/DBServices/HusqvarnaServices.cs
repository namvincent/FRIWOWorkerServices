using FRIWOCenter.Data.TRACE;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace FRIWOCenter.DBServices
{

    public class HusqvarnaServices : ControllerBase
    {
        private readonly IDbContextFactory<TraceDbContext> _contextFactory;
        public HusqvarnaServices(IDbContextFactory<TraceDbContext> context)
        {
            _contextFactory = context;
        }

        public async Task<List<Husqvarna_Json>> getJSONList()
        {
            using (var _context = await _contextFactory.CreateDbContextAsync())
            {
                List<Husqvarna_Json> List_jSONs = new List<Husqvarna_Json>();
                for (int i = 0; i < 2; i++)
                {
                    var result = await _context.Husqvarna_Json
                        .AsNoTracking()
                        .ToListAsync();
                    Husqvarna_Json obj = null;
                    String datetime = "";
                    foreach (var item in result)
                    {
                        String send = string.Empty;
                        obj = new Husqvarna_Json();

                        //JavaScriptSerializer json_serializer = new JavaScriptSerializer();
                        JSONEvent JEvent = JsonConvert.DeserializeObject<JSONEvent>(item.BODY);
                        DateTime d2 = DateTime.Parse(JEvent.DateTime, null, System.Globalization.DateTimeStyles.RoundtripKind);
                        DateTime a1 = new DateTime(d2.Year, d2.Month, d2.Day, 0, 0, 0);
                        datetime = a1.ToString("o");
                        datetime = datetime.ToString() + "+07:00";
                        JSONEvent body = new JSONEvent()
                        {
                            Id = JEvent.Id,
                            Version = JEvent.Version,
                            DateTime = datetime,
                            Fingerprint = JEvent.Fingerprint,
                            State = JEvent.State,
                            Tags = JEvent.Tags
                        };

                        var data = JsonConvert.SerializeObject(body);
                        obj.BODY = data;
                        send = "OK";
                        //send = await SendEvent(obj);
                        if (send.Equals("OK"))
                        {
                            await CallPackageTest(item.BARCODE, obj.BODY);
                        }
                        obj.BARCODE = item.BARCODE;
                        List_jSONs.Add(obj);
                    }
                }
                return List_jSONs;
            }
        }


        public async Task<string> SendEvent(Husqvarna_Json obj)
        {
            using (var _context = await _contextFactory.CreateDbContextAsync())
            {
                string instance = "esb-cpms-Friwo";


                Husqvarna_Json jSONs = new Husqvarna_Json();
                string result = string.Empty;


                using (var client = new HttpClient())
                {

                    //client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "04ffc79a20ef42f4be1f66ac49f58821");
                    client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "ce59ee8e23bf4f30a316a001a094f96a");
                    client.DefaultRequestHeaders.Add("instance", instance);

                    //var response = await client.PostAsync(
                    //       "https://ecomqa.azure-api.net/esbapi/esb-cpms/store/product",
                    //      new StringContent(obj.BODY, Encoding.UTF8, "application/json"));
                    var response = await client.PostAsync(
                     "https://hqvprod.azure-api.net/esbapi/esb-cpms/store/product",
                     new StringContent(obj.BODY, Encoding.UTF8, "application/json"));

                    Husqvarna_Json objson = new Husqvarna_Json();
                    if (response.IsSuccessStatusCode)
                    {
                        result = response.StatusCode.ToString();

                    }
                    else if (response.StatusCode == HttpStatusCode.BadRequest)
                    {

                        result = response.StatusCode.ToString();
                    }
                    else
                    {
                        // dont make too many requests if backend appears to be down
                        await Task.Delay(1000);
                    }
                }
                //await Task.Delay(1000);           

            return result;

            }
        }
        public async Task CallPackageTest(string barcode, String datetime)
        {
            using (var _context = await _contextFactory.CreateDbContextAsync())
            {
                Husqvarna_Json tests = new Husqvarna_Json();

                var parameters = new OracleParameter[0];

                var bc = new OracleParameter("P_ID", OracleDbType.Varchar2, 100, barcode,ParameterDirection.Input);
                var dt = new OracleParameter("P_DATETIME", OracleDbType.Varchar2, 2000, datetime,ParameterDirection.Input);
                var result = new OracleParameter("P_OUT", OracleDbType.Int16, ParameterDirection.Output);
                               
                var res = await _context.Database.ExecuteSqlInterpolatedAsync($"BEGIN GSID_COMMON_PKG.UPDATE_SENDED_GUID_PRC({bc},{dt},{result}); END;", default);

                await _context.SaveChangesAsync();
                Debug.WriteLine(result);
            }
        }
    }
}
