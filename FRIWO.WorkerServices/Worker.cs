using System.Device.Gpio;

namespace FRIWO.WorkerServices
{
    public class Worker : BackgroundService
    {
        int pinWorking = 17;
        int pinFail = 2;
        int pinPass = 5;
        int pinTestingIndicator = 6;
        int pinPassIndicator = 18;
        int pinFailIndicator = 23;
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
                controller?.Write(pinPassIndicator, PinValue.High);
                await Task.Delay(5000);
                controller?.Write(pinPassIndicator, PinValue.Low);
            }

            else
            {
                controller?.Write(pinFailIndicator, PinValue.High);
                await Task.Delay(5000);
                controller?.Write(pinFailIndicator, PinValue.Low);
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            bool p1 = false;
            bool p2 = false;
            bool p3 = false;
            int counter = 0;
            Console.WriteLine("Start blinking LED");
            if (controller != null)
            {
                controller.OpenPin(pinWorking, PinMode.Output);
                controller.OpenPin(pinFail, PinMode.Output);
                controller.OpenPin(pinPass, PinMode.Output);
                controller.OpenPin(pinTestingIndicator, PinMode.Output);
                controller.OpenPin(pinPassIndicator, PinMode.Output);
                controller.OpenPin(pinFailIndicator, PinMode.Output);

                controller.Write(pinWorking, PinValue.High);
            }
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                try
                {
                    string? val;
                    Console.Write("Enter barcode: ");
                    val = Console.ReadLine();
                    if (val != null && val != "")
                        barcode = val;
                    else
                    {
                        p1 = false;
                    }
                    if (controller != null)
                    {
                        controller.Write(pinPassIndicator, PinValue.Low);
                        controller.Write(pinFailIndicator, PinValue.Low);
                        if (val != null)
                        {
                            p1 = true;

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
                    Console.WriteLine(ex);
                }


                Console.WriteLine((p1).ToString());

                Console.WriteLine((p2).ToString());

                if (p1)
                {
                    //write_analog_outputs("00_00");
                    testing = true;
                    if (controller != null)
                    {
                        controller?.Write(pinTestingIndicator, PinValue.High);
                    }
                }

                while (testing)
                {
                    blindLED(pinWorking, stoppingToken);
                    try
                    {
                        //read_analog_outputs(ref p1, ref p2);
                        var rs = controller?.Read(pinPass);
                        if (rs == PinValue.High)
                        {
                            p2 = true;

                        }
                        var rs1 = controller?.Read(pinFail);
                        if (rs1 == PinValue.High)
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
                            var requestStr = $"http://fvn-s-ws01.friwo.local:5000/api/ProcessLock/LaserTrimming/InsertPASSAsync/" + barcode.ToString();
                            Console.WriteLine(requestStr);
                            rq.RequestUri = new Uri(requestStr);
                            var rs = await _httpClient.SendAsync(rq);
                            //write_analog_outputs("00_00");
                            //var rs = await _httpClient.CreateClient().SendAsync(new HttpRequestMessage(HttpMethod.Post, "http://fvn-s-ws01.friwo.local:5000/api/ProcessLock/LaserTrimming/InsertFAILAsync/"));

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
                            //write_analog_outputs("00_00");



                            var rq = new HttpRequestMessage();
                            rq.Method = HttpMethod.Post;
                            var requestStr = $"http://fvn-s-ws01.friwo.local:5000/api/ProcessLock/LaserTrimming/InsertFAILAsync/" + barcode.ToString();
                            Console.WriteLine(requestStr);
                            rq.RequestUri = new Uri(requestStr);
                            var rs = await _httpClient.SendAsync(rq);
                            //controller.Write(pinPassIndicator, PinValue.Low);
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

                    if (counter >= 3)
                    {
                        try
                        {

                            var rq = new HttpRequestMessage();
                            rq.Method = HttpMethod.Post;
                            var requestStr = $"http://fvn-s-ws01.friwo.local:5000/api/ProcessLock/LaserTrimming/InsertFAILAsync/" + barcode.ToString();
                            Console.WriteLine(requestStr);
                            rq.RequestUri = new Uri(requestStr);

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

                            //controller?.Write(pinPassIndicator, PinValue.Low);
                            counter = 0;
                            testing = false;
                            p1 = false;
                            p2 = false;
                            p3 = false;

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
                    await Task.Delay(1000, stoppingToken);
                }
                controller?.Write(pinWorking, PinValue.High);
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}