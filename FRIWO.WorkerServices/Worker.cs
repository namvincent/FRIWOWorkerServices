using System.Device.Gpio;
using System.Text;
namespace FRIWO.WorkerServices
{
    public class Worker : BackgroundService
    {
        int pinReady = 23;
        int pinFail = 2;
        int pinPass = 5;
        int pinTestingIndicator = 6;
        int pinCheckFail = 22;
        int pinFailIndicator = 21;
        int pinCheckLink = 24;
        int pinCheckPass = 27;
        int startTest = 25;
        string barcodeWaiting = "";
        string barcode = "";
        string boxNumber = "";

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
                //     controller?.Write(pinPassIndicator, PinValue.High);
                //     await Task.Delay(5000);
                //     controller?.Write(pinPassIndicator, PinValue.Low);
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

            if (controller != null && counter == 0)
            {
                controller.OpenPin(pinCheckFail, PinMode.Output);
                controller.OpenPin(pinFail, PinMode.Output);
                controller.OpenPin(pinPass, PinMode.Output);
                controller.OpenPin(pinTestingIndicator, PinMode.Output);
                controller.OpenPin(pinCheckPass, PinMode.Output);
                controller.OpenPin(pinFailIndicator, PinMode.Output);
                controller.OpenPin(pinCheckLink, PinMode.Output);
                controller.OpenPin(pinReady, PinMode.Output);
                controller.OpenPin(startTest, PinMode.Output);
                controller.Write(pinCheckPass, PinValue.Low);
                controller.Write(pinFailIndicator, PinValue.Low);
                controller.Write(pinCheckLink, PinValue.Low);
                controller.Write(pinReady, PinValue.Low);
                controller.Write(pinCheckFail, PinValue.Low);
                controller.Write(startTest, PinValue.Low);
                counter++;
            }
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                try
                {
                    string? val;
                    barcode = "";
                    int? stationCheck = 0;
                    if (boxNumber.Length > 2 && boxNumber != "null")
                    {
                        Console.Write("Enter Barcode: ");
                        val = Console.ReadLine();
                        if (val != boxNumber)
                        {
                            controller.Write(pinCheckPass, PinValue.Low);
                            controller.Write(pinCheckFail, PinValue.Low);
                            await Task.Delay(500);
                            controller.Write(pinCheckFail, PinValue.High);
                            Console.Write("Wrong Box Number! ");
                            // p1 = false;
                        }
                        else
                        {
                            controller.Write(pinCheckFail, PinValue.Low);
                            controller.Write(pinCheckPass, PinValue.Low);
                            await Task.Delay(500);
                            controller.Write(pinCheckPass, PinValue.High);
                            Console.Write("correct Box Number! ");
                            // p1 = false;
                        }
                    }
                    else
                    {
                        boxNumber = "";
                        val = "";
                        controller.Write(pinReady, PinValue.Low);
                        controller.Write(pinCheckPass, PinValue.Low);
                        controller.Write(pinCheckFail, PinValue.Low);
                        Console.Write("Enter Line: ");
                        val = Console.ReadLine();
                        if (val.Length > 0 && val != "")
                        {
                            barcode = val;
                            var rq = new HttpRequestMessage();
                            rq.Method = HttpMethod.Post;
                            rq.Content = new StringContent($"\"{barcode}\"", Encoding.UTF8, "application/json");
                            var requestStr = $"http://fvn-nb-077.friwo.local:5100/api/ProcessLock/FA/GetAdditionalData";
                            // var requestStr = $"http://fvn-s-ws01.friwo.local:5000/api/ProcessLock/FA/GetAdditionalData";
                            rq.RequestUri = new Uri(requestStr);
                            var rs = await _httpClient.SendAsync(rq);
                            var responseBody = await rs.Content.ReadAsStringAsync();
                            boxNumber = responseBody;
                            Console.WriteLine("Barcode: " + boxNumber);
                            if (boxNumber.Length > 2 && boxNumber != "null")
                            {
                                controller.Write(pinReady, PinValue.High);
                                Console.Write("get box number success! ");
                            }
                            else
                            {
                                controller.Write(pinReady, PinValue.Low);
                                Console.Write("get Box Number fail! ");
                            }
                        }
                        else
                        {
                            controller.Write(pinReady, PinValue.Low);
                            p1 = false;
                        }
                    }

                }
                catch (Exception ex)
                {
                    testing = false;
                    p1 = false;
                    p2 = false;
                    p3 = false;
                    Console.WriteLine(ex);
                }



                // if (p1)
                // {
                //     //write_analog_outputs("00_00");
                //     testing = true;
                //     if (controller != null)
                //     {
                //         controller?.Write(pinTestingIndicator, PinValue.High);
                //     }
                // }

                // while (testing)
                // {
                //     // blindLED(pinWorking, stoppingToken);
                //     try
                //     {
                //         //read_analog_outputs(ref p1, ref p2);
                //         var rs = controller?.Read(pinPass);
                //         if (rs == PinValue.High)
                //         {
                //             p2 = true;

                //         }
                //         var rs1 = controller?.Read(pinFail);
                //         if (rs1 == PinValue.High)
                //         {
                //             p3 = true;
                //         }
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

                //     Console.WriteLine("Timer: " + counter + "s");

                //     Console.WriteLine("Testing(1:Testing) : " + p1.ToString());

                //     Console.WriteLine("Result(0:Testing;1:Passed;2:Fail) : " + p2.ToString());

                //     counter++;

                //     if (p2)
                //     {
                //         try
                //         {
                //             controller.Write(pinPassIndicator, PinValue.High);
                //             controller.Write(startTest, PinValue.Low);                           
                //             testing = false;
                //             p1 = false;
                //             p2 = false;
                //             p3 = false;
                //             counter = 0;

                //         }
                //         catch (Exception ex)
                //         {
                //             counter = 0;
                //             testing = false;
                //             p1 = false;
                //             p2 = false;
                //             p3 = false;
                //             Console.WriteLine(ex);
                //         }
                //     }

                //     if (p3)
                //     {

                //         try
                //         {

                //             controller.Write(pinFailIndicator, PinValue.High);
                //             controller.Write(startTest, PinValue.Low);
                //             testing = false;
                //             p1 = false;
                //             p2 = false;
                //             p3 = false;
                //             counter = 0;
                //         }
                //         catch (Exception ex)
                //         {
                //             counter = 0;
                //             testing = false;
                //             p1 = false;
                //             p2 = false;
                //             p3 = false;
                //             Console.WriteLine(ex);
                //         }

                //     }

                //     if (counter >= 3)
                //     {
                //         try
                //         {
                //             controller.Write(pinFailIndicator, PinValue.High);
                //             controller.Write(startTest, PinValue.Low);                            
                //             counter = 0;
                //             testing = false;
                //             p1 = false;
                //             p2 = false;
                //             p3 = false;


                //         }
                //         catch (Exception ex)
                //         {
                //             counter = 0;
                //             testing = false;
                //             p1 = false;
                //             p2 = false;
                //             p3 = false;
                //             Console.WriteLine(ex);
                //         }

                //     }
                //     await Task.Delay(1000, stoppingToken);
                // }
                // controller?.Write(pinWorking, PinValue.High);
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}