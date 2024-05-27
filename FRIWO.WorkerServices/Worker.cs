using System.Device.Gpio;
using System.Text;
using System.IO.Ports;
using System.Linq;
using Microsoft.AspNetCore.Components;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace FRIWO.WorkerServices
{
    public class Worker : BackgroundService
    {
        private static Regex smdReg = new Regex("^^\\d{7}([-])\\d{5}([-])\\S{1}$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static Regex uidReg = new Regex("^^\\d{6}", RegexOptions.Compiled | RegexOptions.IgnoreCase);
       
        int pinWorking = 17;
        int pinFailStation = 2;
        int pinPass = 5;
        int pinTestingIndicator = 22;
        int pinPassIndicator = 6;
        int pinPassStation = 3;
        int pinCheckLink = 24;
        int pinCheckStation = 27;
        int startTest = 25;
        string barcodeWaiting = "";
        string barcode = "";

        string uid="0";
        string tempUid = "0";
        //Keypress listener
        DateTime? _lastKeystroke = new DateTime(0);
        List<char>? _barcode = new List<char>(50);


        private readonly ILogger<Worker> _logger;

        private bool testing = false;
        HttpClient _httpClient;
        GpioController? controller;
        string[] serialList = new string[4]{"/dev/ttyUSB0",
                "/dev/ttyUSB1",
                "/dev/ttyUSB2",
                "/dev/ttyUSB3"};

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _httpClient = new HttpClient();
            controller = new GpioController();
            return base.StartAsync(cancellationToken);
        }


        public override Task StopAsync(CancellationToken cancellationToken)
        {
            //_httpClient?.Dispose();
            return base.StopAsync(cancellationToken);
        }
        private async Task blindLED(int pin, CancellationToken cancellationToken)
        {

        }

        private async Task showResult(bool pass, CancellationToken cancellationToken)
        {

        }
        string portName = "";
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            bool p1 = false;
            bool p2 = false;
            bool p3 = false;
            portName = null;
            int counter = 0;
            int firstTest = 0;
            string station = "THT 1";

            bool countCheck = true;
            int? count = 0;
            string? tempOrder = "";
            Console.WriteLine("Start blinking LED");

            if (controller != null)
            {
                controller.OpenPin(pinWorking, PinMode.Output);
                controller.OpenPin(pinFailStation, PinMode.Output);
                controller.OpenPin(pinPass, PinMode.Output);
                controller.OpenPin(pinTestingIndicator, PinMode.Output);
                controller.OpenPin(pinPassIndicator, PinMode.Output);
                controller.OpenPin(pinPassStation, PinMode.Output);
                controller.OpenPin(pinCheckLink, PinMode.Output);
                controller.OpenPin(pinCheckStation, PinMode.Output);
                controller.OpenPin(startTest, PinMode.Output);
                controller.Write(pinPassIndicator, PinValue.Low);
                controller.Write(pinPassStation, PinValue.Low);
                controller.Write(pinCheckLink, PinValue.Low);
                controller.Write(pinCheckStation, PinValue.Low);
                controller.Write(pinWorking, PinValue.Low);
                controller.Write(startTest, PinValue.Low);
            }
            controller.Write(startTest, PinValue.Low);
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                try
                {
                    string? val;
                    barcode = "";
                    int? stationCheck = 0;
                    bool checkRegEx = true;
                    bool checkUidReg = true;
                    Console.Write("Enter barcode: ");
                    val = Console.ReadLine();

                    
                    if (val.Length > 2 && val != "")
                    {
                        barcode = val;
                        checkRegEx = smdReg.IsMatch(barcode);
                        if(!checkRegEx){
                            checkUidReg = uidReg.IsMatch(barcode);
                            if(checkUidReg){
                                tempUid = barcode;
                                if(!tempUid.Equals(uid)){
                                    uid = tempUid;
                                    "Update UID Success!".WriteLineColor(ConsoleColor.Green);
                                }else{
                                    uid = "0";
                                    "CheckOut UID Success!".WriteLineColor(ConsoleColor.Yellow);
                                }
                                
                            }
                            barcode = "";   

                        }
                                               
                        //////////Check Routing 
                        if (barcode.Length > 2 && barcode != "null")
                        {
                            var httpRQ = new HttpRequestMessage();
                            httpRQ.Method = HttpMethod.Post;
                            var previousCheck = $"http://10.100.10.83:5000/api/ProcessLock/MI/CheckPreviousSMDStation/{barcode}/{station}";
                            httpRQ.RequestUri = new Uri(previousCheck);
                            var rsData = await _httpClient.SendAsync(httpRQ);
                            var previousresponseBody = await rsData.Content.ReadAsStringAsync();
                            stationCheck = Int16.Parse(previousresponseBody);
                        }
                    }
                    else
                    {
                        p1 = false;
                    }
                    if (controller != null)
                    {
                        controller.Write(pinCheckLink, PinValue.Low);
                        controller.Write(pinCheckStation, PinValue.Low);
                        controller.Write(pinPassStation, PinValue.Low);
                        controller.Write(pinFailStation, PinValue.Low);
                        controller.Write(startTest, PinValue.Low);
                        if (barcode.Length > 2 && barcode != "null")
                        {
                            if (stationCheck > 0)
                            {
                                p1 = true;
                                controller.Write(pinPassStation, PinValue.High);
                            }
                            else
                            {
                                p1 = false;
                                controller.Write(pinFailStation, PinValue.High);
                                "Fail Station!".WriteLineColor(ConsoleColor.Red);
                            }
                        }
                        else if(!checkUidReg)
                        {
                            p1 = false;
                            controller.Write(pinFailStation, PinValue.High);
                            "Wrong barcode!".WriteLineColor(ConsoleColor.Red);
                        }
                    }
                    // p1 = true;
                }
                catch (Exception ex)
                {
                    counter = 0;
                    testing = false;
                    p1 = false;
                    p2 = false;
                    p3 = false;
                    Console.WriteLine(ex);
                }
                if(p1){
                    testing = true;
                }
                
                while (testing)
                {
                    try
                    {                       
                        p2 = true;
                    }
                    catch (Exception ex)
                    {
                        counter = 0;
                        testing = false;
                        p1 = false;
                        p2 = false;
                        p3 = false;
                        Console.WriteLine(ex);
                    }


                    counter++;

                    if (p2)
                    {
                        try
                        {
                            var rq = new HttpRequestMessage();
                            rq.Method = HttpMethod.Post;                            
                            var requestStr = $"http://10.100.10.83:5000/api/ProcessLock/SMT/InsertTHTDataWithOperator/" + barcode.Split("-")[0].ToString() + "/" + barcode.ToString() +"/"+ 1 +"/"+ station+"/"+ uid;                            
                            rq.RequestUri = new Uri(requestStr);
                            //Console.WriteLine(requestStr);
                            var rs = await _httpClient.SendAsync(rq);
                            var rsultData = await rs.Content.ReadAsStringAsync(); 
                            //rsultData.ToString().WriteLineColor(ConsoleColor.Yellow);
                            //checking count and insert status
                            if(tempOrder != barcode.Split("-")[0].ToString())
                            {
                                countCheck = true;
                                count = 0;  
                            }
                            if(countCheck)
                            {
                                var rq_1 = new HttpRequestMessage();
                                rq_1.Method = HttpMethod.Post;                            
                                var requestStrCnt = $"http://10.100.10.83:5000/api/GetData/SMT/CountQTYByShopOrder/" + barcode.Split("-")[0].ToString() + "/" + station;
                                rq_1.RequestUri = new Uri(requestStrCnt);
                                var rs_1 = await _httpClient.SendAsync(rq_1);
                                var countData =  await rs_1.Content.ReadAsStringAsync(); 
                                tempOrder = barcode.Split("-")[0].ToString();
                                count = Int16.Parse(countData);
                                countCheck = false;
                                //Console.WriteLine(rsultData);
                                if(rsultData.ToString() == "1")
                                {                                                                              
                                    "Successful !".WriteLineColor(ConsoleColor.Green);
                                }else{
                                    "Dupplicated !".WriteLineColor(ConsoleColor.Red);
                                }                            
                            }
                            else if(!countCheck){
                                if(rsultData.ToString() == "1")
                                {        
                                    count++;                                                                       
                                    "Successful !".WriteLineColor(ConsoleColor.Green);
                                }else{
                                    "Dupplicated !".WriteLineColor(ConsoleColor.Red);
                                }
                            }
                            string countOut = "Total Qty of SO " +  barcode.Split("-")[0].ToString() + " : " + count;
                            countOut.WriteLineColor(ConsoleColor.Green); 
                            string uidInfo = "Operator: "+uid;
                            uidInfo.WriteLineColor(ConsoleColor.Green);
                            if(uid.Equals("0")){
                                var rq1 = new HttpRequestMessage();
                                rq1.Method = HttpMethod.Get;
                                var requestStr1 = $"http://10.100.10.83:5000/api/Email/Alarmoperatorid?line={station}&type=The%20station%20still%20has%20not%20scanned%20the%20operator%20ID%20yet%20%21";
                                rq1.RequestUri = new Uri(requestStr1);
                                var rs1 = await _httpClient.SendAsync(rq1);
                                var responseBody1 = await rs1.Content.ReadAsStringAsync();
                                "No Operator ID - Send Alarm Email !".WriteLineColor(ConsoleColor.Red);
                            }
                            await Task.Delay(1000);
                            controller.Write(pinPassStation, PinValue.Low);
                            controller.Write(pinFailStation, PinValue.Low);
                            controller.Write(startTest, PinValue.Low);                            
                            if (rs.StatusCode == System.Net.HttpStatusCode.OK)
                            {
                                showResult(true, stoppingToken);
                            }
                            else
                            {
                                showResult(false, stoppingToken);
                            }
                            testing = false;
                            p1 = false;
                            p2 = false;
                            p3 = false;
                            counter = 0;

                        }
                        catch (Exception ex)
                        {
                            counter = 0;
                            testing = false;
                            p1 = false;
                            p2 = false;
                            p3 = false;
                            Console.WriteLine(ex);
                        }
                    }

                    if (p3)
                    {

                        try
                        {
                           

                            testing = false;
                            p1 = false;
                            p2 = false;
                            p3 = false;
                            counter = 0;
                        }
                        catch (Exception ex)
                        {
                            counter = 0;
                            testing = false;
                            p1 = false;
                            p2 = false;
                            p3 = false;
                            Console.WriteLine(ex);
                        }

                    }

                }
                await Task.Delay(1000, stoppingToken);
            }
        }
		
    }
}