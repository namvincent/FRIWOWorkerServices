using System.Device.Gpio;
using System.Text;
using System.IO.Ports;
using System.Linq;
using Microsoft.AspNetCore.Components;
using System.Text.Json;
using MarkingComponent;

namespace FRIWO.WorkerServices
{
    public class Worker : BackgroundService
    {
        [Inject]
        private MultimeterServices? multimeterServices
        {
            get; set;
        }
        int pinWorking = 17;
        int pinFail = 2;
        int pinPass = 5;
        int pinTestingIndicator = 22;
        int pinPassIndicator = 6;
        int pinFailIndicator = 3;
        int pinCheckLink = 24;
        int pinCheckStation = 27;
        int startTest = 25;
        string barcodeWaiting = "";
        string barcode = "";

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
            MultimeterResult test = new MultimeterResult();
            Console.WriteLine("Start blinking LED");

            if (controller != null)
            {
                controller.OpenPin(pinWorking, PinMode.Output);
                controller.OpenPin(pinFail, PinMode.Output);
                controller.OpenPin(pinPass, PinMode.Output);
                controller.OpenPin(pinTestingIndicator, PinMode.Output);
                controller.OpenPin(pinPassIndicator, PinMode.Output);
                controller.OpenPin(pinFailIndicator, PinMode.Output);
                controller.OpenPin(pinCheckLink, PinMode.Output);
                controller.OpenPin(pinCheckStation, PinMode.Output);
                controller.OpenPin(startTest, PinMode.Output);
                controller.Write(pinPassIndicator, PinValue.Low);
                controller.Write(pinFailIndicator, PinValue.Low);
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
                    Console.Write("Enter barcode: ");
                    val = Console.ReadLine();
                    test = new MultimeterResult();
                    controller.Write(pinPassIndicator, PinValue.Low);
                    controller.Write(pinFailIndicator, PinValue.Low);

                    if (val.Length > 2 && val != "")
                    {
                        barcode = val;
                        var rq = new HttpRequestMessage();
                        rq.Method = HttpMethod.Post;
                        rq.Content = new StringContent($"\"{barcode}\"", Encoding.UTF8, "application/json");
                        // var requestStr = $"http://fvn-nb-132.friwo.local:5000/api/ProcessLock/FA/GetLinkData";
                        var requestStr = $"http://fvn-s-web01.friwo.local:5000/api/ProcessLock/FA/GetLinkData";
                        rq.RequestUri = new Uri(requestStr);
                        var rs = await _httpClient.SendAsync(rq);
                        var responseBody = await rs.Content.ReadAsStringAsync();
                        barcode = responseBody;
                        ///////////////////////////////////////////////////////////////
                        if (barcode.Length > 2 && barcode != "null")
                        {
                            var httpRQ = new HttpRequestMessage();
                            httpRQ.Method = HttpMethod.Post;
                            var previousCheck = $"http://fvn-s-web01.friwo.local:5000/api/ProcessLock/FA/CheckPreviousStation/{barcode}/CHECK LED";
                            Console.WriteLine(previousCheck);
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
                        controller.Write(pinPassIndicator, PinValue.Low);
                        controller.Write(pinFailIndicator, PinValue.Low);
                        controller.Write(startTest, PinValue.Low);
                        if (barcode.Length > 2 && barcode != "null")
                        {
                            if (stationCheck > 0)
                            {
                                p1 = true;
                                controller.Write(startTest, PinValue.High);
                            }
                            else
                            {
                                p1 = false;
                                controller.Write(pinCheckStation, PinValue.High);
                                "Fail Station!".WriteLineColor(ConsoleColor.Red);
                            }
                        }
                        else
                        {
                            p1 = false;
                            controller.Write(pinCheckLink, PinValue.High);
                            "Fail Link!".WriteLineColor(ConsoleColor.Red);
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
                if (p1)
                {
                    test = await GetResultData();
                    testing = true;
                    if (controller != null)
                    {
                        controller?.Write(pinTestingIndicator, PinValue.High);
                    }
                }

                while (testing)
                {
                    try
                    {
                        // read_analog_outputs(ref p1, ref p2);

                        if (test.Status.Equals("PASS"))
                        {
                            p2 = true;

                        }
                        else
                        {
                            p3 = true;
                        }
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
                            BodyInsertData body = new BodyInsertData()
                            {
                                Barcode = barcode.ToString(),
                                Status = 1,
                                MachineID = "PI",
                                // Result = test.Measure.Trim()+"@@"+test.Measure.Trim()
                                Result = test.Measure.Trim()
                            };
                            var bodyJson = JsonSerializer.Serialize(body);
                            Console.WriteLine(bodyJson);
                            var rq = new HttpRequestMessage();
                            rq.Method = HttpMethod.Post;
                            rq.Content = new StringContent(bodyJson, Encoding.UTF8, "application/json");
                            // var requestStr = $"http://fvn-nb-132.friwo.local:5000/api/ProcessLock/FA/InsertCheckLEDDataAsync/";
                            var requestStr = $"http://fvn-s-web01.friwo.local:5000/api/ProcessLock/FA/InsertCheckLEDDataAsync/";
                            // Console.WriteLine(requestStr);
                            rq.RequestUri = new Uri(requestStr);
                            var rs = await _httpClient.SendAsync(rq);
                            //write_analog_outputs("00_00");
                            //var rs = await _httpClient.CreateClient().SendAsync(new HttpRequestMessage(HttpMethod.Post, "http://fvn-nb-077.friwo.local:5000/api/ProcessLock/LaserTrimming/InsertFAILAsync/"));
                            controller.Write(pinPassIndicator, PinValue.High);
                            controller.Write(startTest, PinValue.Low);
                            test.Status.WriteLineColor(ConsoleColor.Green);
                            test.Measure.WriteLineColor(ConsoleColor.Green);
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
                            BodyInsertData body = new BodyInsertData()
                            {
                                Barcode = barcode.ToString(),
                                Status = 1,
                                MachineID = "PI",
                                // Result = test.Measure.Trim()+"@@"+test.Measure.Trim()
                                Result = test.Measure.Trim()
                            };
                            var bodyJson = JsonSerializer.Serialize(body);
                            var rq = new HttpRequestMessage();
                            rq.Method = HttpMethod.Post;
                            rq.Content = new StringContent(bodyJson, Encoding.UTF8, "application/json");
                            // var requestStr = $"http://fvn-nb-132.friwo.local:5000/api/ProcessLock/FA/InsertCheckLEDDataAsync/";
                            var requestStr = $"http://fvn-s-web01.friwo.local:5000/api/ProcessLock/FA/InsertCheckLEDDataAsync/";
                            // Console.WriteLine(requestStr);
                            rq.RequestUri = new Uri(requestStr);
                            var rs = await _httpClient.SendAsync(rq);
                            controller.Write(pinFailIndicator, PinValue.High);
                            controller.Write(startTest, PinValue.Low);
                            test.Status.WriteLineColor(ConsoleColor.Red);
                            test.Measure.WriteLineColor(ConsoleColor.Red);
                            if (rs.StatusCode == System.Net.HttpStatusCode.OK)
                            {
                                showResult(false, stoppingToken);

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

                    // if (counter >= 28)
                    // {
                    //     try
                    //     {

                    //         var rq = new HttpRequestMessage();
                    //         rq.Method = HttpMethod.Post;
                    //         // var requestStr = $"http://fvn-nb-132.friwo.local:5000/api/ProcessLock/FA/InsertCheckLEDDataAsync/" + barcode.ToString() + "/" + 0;
                    //         var requestStr = $"http://fvn-s-web01.friwo.local:5000/api/ProcessLock/FA/InsertCheckLEDDataAsync/" + barcode.ToString() + "/" + 0;
                    //         // var requestStr = $"http://fvn-s-web01.friwo.local:5000/api/ProcessLock/AOI/InsertFAILAOIAsync/" + barcode.ToString();
                    //         Console.WriteLine(requestStr);
                    //         rq.RequestUri = new Uri(requestStr);
                    //         controller.Write(pinFailIndicator, PinValue.High);
                    //         controller.Write(startTest, PinValue.Low);
                    //         var rs = await _httpClient.SendAsync(rq);
                    //         Console.WriteLine(rs.StatusCode);
                    //         if (rs.StatusCode == System.Net.HttpStatusCode.OK)
                    //         {
                    //             showResult(false, stoppingToken);
                    //             Console.WriteLine("Data is sent. >>>>>>>>>>GOOD");
                    //             _logger.LogInformation("Data is sent. >>>>>>>>>>GOOD");
                    //         }
                    //         else
                    //         {
                    //             Console.WriteLine("Cannot update data");
                    //             _logger.LogInformation("Cannot update data");
                    //             showResult(false, stoppingToken);
                    //         }

                    //         counter = 0;
                    //         testing = false;
                    //         p1 = false;
                    //         p2 = false;
                    //         p3 = false;


                    //     }
                    //     catch (Exception ex)
                    //     {
                    //         counter = 0;
                    //         testing = false;
                    //         p1 = false;
                    //         p2 = false;
                    //         p3 = false;
                    //         Console.WriteLine(ex);
                    //     }

                    // }
                    await Task.Delay(1000, stoppingToken);
                }
                await Task.Delay(1000, stoppingToken);
            }
        }
        async Task<MultimeterResult> GetResultData()
        {

            MultimeterResult testResult = new MultimeterResult();
            if (string.IsNullOrEmpty(portName))
            {
                foreach (var item in serialList)
                {

                    SerialPort test = new SerialPort(item);
                    try
                    {
                        test.Open();
                        Console.WriteLine(item);
                        if (test.IsOpen == true)
                        {
                            portName = item;
                            test.Close();
                            break;
                        }
                    }
                    catch (Exception) { }
                }
            }
            SerialPort myport = new SerialPort(portName);
            List<MultimeterResult> resultData = new();
            myport.ReadTimeout = 50000;
            myport.WriteTimeout = 50000;
            myport.Open();
            string measure = "";
            for (int i = 0; i < 150; i++)
            {
                myport.WriteLine("COMP?");
                myport.WriteLine("VAL1?");

                string serialRead = myport.ReadLine();
                if (!serialRead.StartsWith(".") && !string.IsNullOrEmpty(serialRead) && !serialRead.Contains(">") && !serialRead.Contains("!>") && !serialRead.Contains("=>") && !serialRead.Contains("?>") && !serialRead.Contains("PASS") && !serialRead.Contains("P") && !serialRead.Contains("PAPASS") && !serialRead.Contains("LO") && !serialRead.Contains("HI"))
                {
                    measure = serialRead;
                }
                if (serialRead.Contains("PASS"))
                {
                    testResult.Status = "PASS";
                    myport.ReadLine();
                    testResult.Measure = myport.ReadLine();
                    // resultData.Add(testResult);
                    break;
                }
                else
                {
                    testResult.Status = "FAIL";
                    testResult.Measure = measure;
                }
                Console.WriteLine(testResult.Measure);
                Console.WriteLine(testResult.Status);
                await Task.Delay(100);


            }
            myport.Close();
            return testResult;
        }
    }
}