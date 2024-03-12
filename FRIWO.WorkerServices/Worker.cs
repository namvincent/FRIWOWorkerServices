using System.Device.Gpio;
using System.Text;
namespace FRIWO.WorkerServices
{
    public class Worker : BackgroundService
    {
        public const int pinWorking = 17;
        int pinFail = 2;
        int pinFailIndicator = 3;
        int pinPass = 5;
        int pinPassIndicator = 6;
        int pinMeasuring = 26;
        int pinMeasuringIndicator = 19;
        int pinNG = 22;
        int pinNGIndicator = 23;
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
            Console.WriteLine("Blinking LED. Press Ctrl+C to end.");
            bool ledOn = true;
            while (testing)
            {
                controller?.Write(pin, ((ledOn) ? PinValue.High : PinValue.Low));
                ledOn = !ledOn;
                await Task.Delay(100);
            }
        }

        private async Task showResult(bool pass, CancellationToken cancellationToken)
        {
            Console.WriteLine("End testing. Result LED is shown");
            if (pass)
            {
                // controller?.Write(pinNG, PinValue.High);
                // await Task.Delay(5000);
                // controller?.Write(pinNG, PinValue.Low);
            }

            else
            {
                //     controller?.Write(pinNGIndicator, PinValue.High);
                //     await Task.Delay(5000);
                //     controller?.Write(pinNGIndicator, PinValue.Low);
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            bool p1 = false;
            bool p2 = false;
            bool p3 = false;
            bool p4 = false;
            int counter = 0;
            Console.WriteLine("Start blinking LED");

            if (controller != null)
            {
                controller.OpenPin(pinWorking, PinMode.Output);
                controller.OpenPin(pinFail, PinMode.Output);
                controller.OpenPin(pinPass, PinMode.Output);
                controller.OpenPin(pinNG, PinMode.Output);
                controller.OpenPin(pinNGIndicator, PinMode.Output);
                controller.OpenPin(pinPassIndicator, PinMode.Output);
                controller.OpenPin(pinFailIndicator, PinMode.Output);
                controller.OpenPin(pinMeasuringIndicator, PinMode.Output);
                controller.OpenPin(pinCheckLink, PinMode.Output);
                controller.OpenPin(pinCheckStation, PinMode.Output);
                controller.OpenPin(startTest, PinMode.Output);
                controller.OpenPin(pinMeasuring, PinMode.Output);
                controller.Write(pinNG, PinValue.Low);
                controller.Write(pinNGIndicator, PinValue.Low);
                controller.Write(pinPassIndicator, PinValue.Low);
                controller.Write(pinFailIndicator, PinValue.Low);
                controller.Write(pinMeasuringIndicator, PinValue.Low);
                controller.Write(pinCheckLink, PinValue.Low);
                controller.Write(pinCheckStation, PinValue.Low);
                controller.Write(pinWorking, PinValue.Low);
                controller.Write(startTest, PinValue.Low);
                controller.Write(pinMeasuring, PinValue.Low);
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
                    controller?.Write(pinWorking, PinValue.High);
                    Console.Write("Enter barcode: ");
                    val = Console.ReadLine();
                    if (val.Length > 2 && val != "")
                    {
                        barcode = val;
                        var rq = new HttpRequestMessage();
                        rq.Method = HttpMethod.Post;
                        rq.Content = new StringContent($"\"{barcode}\"", Encoding.UTF8, "application/json");
                        var requestStr = $"http://fvn-s-web01.friwo.local:5000/api/ProcessLock/FA/GetLinkData";
                        // var requestStr = $"http://fvn-s-web01.friwo.local:5000/api/ProcessLock/FA/GetLinkData";
                        rq.RequestUri = new Uri(requestStr);
                        var rs = await _httpClient.SendAsync(rq);
                        var responseBody = await rs.Content.ReadAsStringAsync();
                        barcode = responseBody;

                        // barcode = "1212112121212121212121";//fake internal UID
                        Console.WriteLine("Barcode: " + barcode);
                        ///////////////////////////////////////////////////////////////
                        // if (barcode.Length > 2 && barcode != "null")
                        // {
                        //     var httpRQ = new HttpRequestMessage();
                        //     httpRQ.Method = HttpMethod.Post;
                        //     // var previousCheck = $"http://fvn-nb-077.friwo.local:5000/api/ProcessLock/FA/CheckPreviousStation/{barcode}/VACUUM STATION";  
                        //     var previousCheck = $"http://fvn-s-web01.friwo.local:5000/api/ProcessLock/FA/CheckPreviousStation/{barcode}/PCBA HEIGHT CHECK";
                        //     Console.WriteLine(previousCheck);
                        //     httpRQ.RequestUri = new Uri(previousCheck);
                        //     //var rsData = await _httpClient.SendAsync(httpRQ);
                        //     //var previousresponseBody = await rsData.Content.ReadAsStringAsync();
                        //     //stationCheck = Int16.Parse(previousresponseBody);
                        stationCheck = 1;
                        //     Console.WriteLine("Previous Check: " + stationCheck.ToString());
                        // }

                    }
                    else
                    {
                        p1 = false;
                    }
                    if (controller != null)
                    {
                        controller.Write(pinCheckLink, PinValue.Low);
                        controller.Write(pinCheckStation, PinValue.Low);
                        controller.Write(pinNGIndicator, PinValue.Low);
                        controller.Write(pinPassIndicator, PinValue.Low);
                        controller.Write(pinFailIndicator, PinValue.Low);
                        controller.Write(pinMeasuringIndicator, PinValue.Low);
                        controller.Write(pinNG, PinValue.Low);
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

                            }
                        }
                        else
                        {
                            p1 = false;
                            controller.Write(pinCheckLink, PinValue.High);

                        }
                    }
                }
                catch (Exception ex)
                {
                    counter = 0;
                    testing = false;
                    p1 = false;
                    p2 = false;
                    p3 = false;
                    p4 = false;
                    Console.WriteLine(ex);
                }


                Console.WriteLine((p1).ToString());

                Console.WriteLine((p2).ToString());
                int waitMeasuring = 0;
                bool timeOut = false;
                if (p1)
                {

                    testing = true;


                    while (!controller?.Read(pinMeasuring) == PinValue.High)
                    {
                        Console.WriteLine("Waiting measure...");
                        await Task.Delay(500, stoppingToken);
                        waitMeasuring++;
                        if (waitMeasuring >= 20)
                        {
                            Console.WriteLine("Fail measure!!!");
                            controller.Write(pinNG, PinValue.High);
                            timeOut = true;
                            break;
                        }
                    }
                    if (!timeOut)
                    {
                        controller.Write(pinMeasuringIndicator, PinValue.High);
                    }
                }
                else
                {
                    controller.Write(pinNGIndicator, PinValue.High);
                }

                // testing=true;



                while (testing)
                {
                    // blindLED(pinWorking, stoppingToken);
                    try
                    {
                        //read_analog_outputs(ref p1, ref p2);

                        if (!timeOut)
                        {
                            var rs1 = controller?.Read(pinFail);
                            var rs2 = controller?.Read(pinNG);
                            if (rs1 == PinValue.High)
                            {
                                p3 = true;
                                controller.Write(pinMeasuringIndicator, PinValue.Low);
                            }
                            if (rs2 == PinValue.High)
                            {
                                p4 = true;
                                controller.Write(pinMeasuringIndicator, PinValue.Low);
                            }
                            var rs = controller?.Read(pinPass);
                            if (rs == PinValue.High)
                            {
                                p2 = true;
                                controller.Write(pinMeasuringIndicator, PinValue.Low);
                            }
                        }
                        else
                        {
                            p4 = true;
                            controller.Write(pinMeasuringIndicator, PinValue.Low);
                        }
                    }
                    catch (Exception ex)
                    {
                        counter = 0;
                        testing = false;
                        p1 = false;
                        p2 = false;
                        p3 = false;
                        p4 = false;
                        Console.WriteLine(ex);
                    }

                    Console.WriteLine("Timer: " + counter + "s");

                    Console.WriteLine("Testing(1:Testing) : " + p1.ToString());

                    Console.WriteLine("Result(0:Testing;1:Passed;2:Fail) : " + p2.ToString());

                    counter++;

                    if (p2)
                    {
                        try
                        {
                            var rq = new HttpRequestMessage();
                            rq.Method = HttpMethod.Post;
                            // var requestStr = $"http://fvn-nb-077.friwo.local:5000/api/ProcessLock/FA/InsertVauumAsync/" + barcode.ToString()+"/"+1;
                            var requestStr = $"http://fvn-s-web01.friwo.local:5000/api/ProcessLock/Cable/InsertCableDimensionAsync/" + barcode.ToString() + "/" + 1 + "/PI/AS";
                            // var requestStr = $"http://fvn-s-ws01.friwo.local:5000/api/ProcessLock/AOI/InsertPASSAOIAsync/" + barcode.ToString();
                            Console.WriteLine(requestStr);
                            rq.RequestUri = new Uri(requestStr);
                            var rs = await _httpClient.SendAsync(rq);
                            //write_analog_outputs("00_00");
                            //var rs = await _httpClient.CreateClient().SendAsync(new HttpRequestMessage(HttpMethod.Post, "http://fvn-nb-077.friwo.local:5000/api/ProcessLock/LaserTrimming/InsertFAILAsync/"));
                            controller.Write(pinPassIndicator, PinValue.High);

                            Console.WriteLine(rs.StatusCode);
                            if (rs.StatusCode == System.Net.HttpStatusCode.OK)
                            {
                                showResult(true, stoppingToken);
                                Console.WriteLine("Data is sent. >>>>>>>>>>GOOD");
                                _logger.LogInformation("Data is sent. >>>>>>>>>>GOOD");
                            }
                            else
                            {
                                Console.WriteLine("Cannot update data");
                                _logger.LogInformation("Cannot update data");
                                showResult(false, stoppingToken);
                            }
                            testing = false;
                            p1 = false;
                            p2 = false;
                            p3 = false;
                            p4 = false;
                            counter = 0;

                        }
                        catch (Exception ex)
                        {
                            counter = 0;
                            testing = false;
                            p1 = false;
                            p2 = false;
                            p3 = false;
                            p4 = false;
                            Console.WriteLine(ex);
                        }
                    }

                    if (p3)
                    {

                        try
                        {
                            //write_analog_outputs("00_00");
                            var rq = new HttpRequestMessage();
                            rq.Method = HttpMethod.Post;
                            // var requestStr = $"http://fvn-nb-077.friwo.local:5000/api/ProcessLock/FA/InsertVauumAsync/" + barcode.ToString() + "/" + 0;
                            var requestStr = $"http://fvn-s-web01.friwo.local:5000/api/ProcessLock/Cable/InsertCableDimensionAsync/" + barcode.ToString() + "/" + 0 + "/PI/FAIL";
                            Console.WriteLine(requestStr);
                            rq.RequestUri = new Uri(requestStr);
                            var rs = await _httpClient.SendAsync(rq);
                            controller.Write(pinFailIndicator, PinValue.High);
                            controller.Write(startTest, PinValue.Low);
                            Console.WriteLine(rs.StatusCode);
                            if (rs.StatusCode == System.Net.HttpStatusCode.OK)
                            {
                                showResult(false, stoppingToken);
                                Console.WriteLine("Data is sent. >>>>>>>>>>GOOD");
                                _logger.LogInformation("Data is sent. >>>>>>>>>>GOOD");
                            }
                            else
                            {
                                Console.WriteLine("Cannot update data");
                                _logger.LogInformation("Cannot update data");
                                showResult(false, stoppingToken);
                            }

                            testing = false;
                            p1 = false;
                            p2 = false;
                            p3 = false;
                            p4 = false;
                            counter = 0;
                        }
                        catch (Exception ex)
                        {
                            counter = 0;
                            testing = false;
                            p1 = false;
                            p2 = false;
                            p3 = false;
                            p4 = false;
                            Console.WriteLine(ex);
                        }


                    }
                    if (p4)
                    {
                        try
                        {
                            //write_analog_outputs("00_00");
                            var rq = new HttpRequestMessage();
                            rq.Method = HttpMethod.Post;
                            // var requestStr = $"http://fvn-nb-077.friwo.local:5000/api/ProcessLock/FA/InsertVauumAsync/" + barcode.ToString() + "/" + 0;
                            var requestStr = $"http://fvn-s-web01.friwo.local:5000/api/ProcessLock/Cable/InsertCableDimensionAsync/" + barcode.ToString() + "/" + 0 + "/PI/NG";
                            Console.WriteLine(requestStr);
                            rq.RequestUri = new Uri(requestStr);
                            var rs = await _httpClient.SendAsync(rq);
                            controller.Write(pinNGIndicator, PinValue.High);
                            controller.Write(startTest, PinValue.Low);
                            Console.WriteLine(rs.StatusCode);
                            if (rs.StatusCode == System.Net.HttpStatusCode.OK)
                            {
                                showResult(false, stoppingToken);
                                Console.WriteLine("Data is sent. >>>>>>>>>>GOOD");
                                _logger.LogInformation("Data is sent. >>>>>>>>>>GOOD");
                            }
                            else
                            {
                                Console.WriteLine("Cannot update data");
                                _logger.LogInformation("Cannot update data");
                                showResult(false, stoppingToken);
                            }

                            testing = false;
                            p1 = false;
                            p2 = false;
                            p3 = false;
                            p4 = false;
                            counter = 0;
                        }
                        catch (Exception ex)
                        {
                            counter = 0;
                            testing = false;
                            p1 = false;
                            p2 = false;
                            p3 = false;
                            p4 = false;
                            Console.WriteLine(ex);
                        }


                    }

                    if (counter >= 15)
                    {
                        try
                        {

                            var rq = new HttpRequestMessage();
                            rq.Method = HttpMethod.Post;
                            var requestStr = $"http://fvn-s-web01.friwo.local:5000/api/ProcessLock/Cable/InsertCableDimensionAsync/" + barcode.ToString() + "/" + 0 + "/PI/TIMEOUT";
                            // var requestStr = $"http://fvn-s-web01.friwo.local:5000/api/ProcessLock/FA/InsertPCBHeightCheckAsync/" + barcode.ToString() + "/" + 0;
                            // var requestStr = $"http://fvn-s-ws01.friwo.local:5000/api/ProcessLock/AOI/InsertFAILAOIAsync/" + barcode.ToString();
                            Console.WriteLine(requestStr);
                            rq.RequestUri = new Uri(requestStr);
                            controller.Write(pinNGIndicator, PinValue.High);
                            controller.Write(startTest, PinValue.Low);
                            var rs = await _httpClient.SendAsync(rq);
                            Console.WriteLine(rs.StatusCode);
                            if (rs.StatusCode == System.Net.HttpStatusCode.OK)
                            {
                                showResult(false, stoppingToken);
                                Console.WriteLine("Data is sent. >>>>>>>>>>GOOD");
                                _logger.LogInformation("Data is sent. >>>>>>>>>>GOOD");
                            }
                            else
                            {
                                Console.WriteLine("Cannot update data");
                                _logger.LogInformation("Cannot update data");
                                showResult(false, stoppingToken);
                            }

                            counter = 0;
                            testing = false;
                            p1 = false;
                            p2 = false;
                            p3 = false;
                            p4 = false;
                        }
                        catch (Exception ex)
                        {
                            counter = 0;
                            testing = false;
                            p1 = false;
                            p2 = false;
                            p3 = false;
                            p4 = false;
                            Console.WriteLine(ex);
                        }

                    }
                    await Task.Delay(1000, stoppingToken);
                }
                controller?.Write(pinWorking, PinValue.High);
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}