using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using System.Device.Gpio;
using System.Text;

public class CheckBoxService : BackgroundService
{
    HttpClient _httpClient;
    GpioController? controller;
    int pinReady = 23;
    int pinSwitch = 17;
    int pinCheckFail = 22;
    int pinCheckPass = 27;
    int pinPower = 24;
    string barcodeWaiting = "";
    string barcode = "";
    string boxNumber = "";
    public CheckBoxService()
    {


    }
    public override Task StartAsync(CancellationToken cancellationToken)
    {
        _httpClient = new HttpClient();
        controller = new GpioController();
        return base.StartAsync(cancellationToken);
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {

        Console.WriteLine("Service is starting.");
        int count = 0;
        int scan = 0;
        bool waiting = true;
        string line = "0";
        if (controller != null)
        {
            controller.OpenPin(pinCheckFail, PinMode.Output);
            controller.OpenPin(pinSwitch, PinMode.Output);
            controller.OpenPin(pinCheckPass, PinMode.Output);
            controller.OpenPin(pinReady, PinMode.Output);
            controller.OpenPin(pinPower, PinMode.Output);
            controller.Write(pinCheckPass, PinValue.Low);
            controller.Write(pinReady, PinValue.Low);
            controller.Write(pinCheckFail, PinValue.Low);
            controller.Write(pinPower, PinValue.Low);
        }
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {

                int? stationCheck = 0;
                if (boxNumber.Length > 2 && boxNumber != "null")
                {
                    var checkSwitch = controller?.Read(pinSwitch);
                    if (checkSwitch == PinValue.High)
                    {
                        if (scan == 0)
                        {
                            controller.Write(pinCheckPass, PinValue.Low);
                            controller.Write(pinCheckFail, PinValue.Low);
                            scan = 0;
                            waiting = false;
                        }
                        if (barcode.Length == 0)
                        {
                            Console.WriteLine("Enter input within 1 seconds or press Enter to cancel:");

                            var inputTask = Task.Run(() =>
                            {
                                while (!Console.KeyAvailable)
                                {
                                    if (stoppingToken.IsCancellationRequested)
                                        return;
                                }
                            });

                            var delayTask = Task.Delay(1500, stoppingToken); // Timeout after 1 seconds

                            var completedTask = await Task.WhenAny(inputTask, delayTask);

                            if (completedTask == inputTask)
                            {
                                stoppingToken.ThrowIfCancellationRequested(); // Throw if cancellation requested
                                string userInput = Console.ReadLine(); // Read the input after cancellation
                                barcode = userInput;
                            }
                            else
                            {
                                Console.WriteLine("Input timed out.");
                                barcode = "time out";
                            }
                        }
                        Console.WriteLine($"You entered: {barcode}");
                        if (barcode.Length > 2)
                        {
                            if (barcode != boxNumber)
                            {
                                if (scan == 0)
                                {
                                    switch (barcode)
                                    {
                                        case "time out":
                                            var rq = new HttpRequestMessage();
                                            rq.Method = HttpMethod.Get;
                                            var requestStr = $"http://fvn-s-web01:5000/api/Email/Alarmwrongbox?line={line}&type=Time%20Out";
                                            rq.RequestUri = new Uri(requestStr);
                                            var rs = await _httpClient.SendAsync(rq);
                                            var responseBody = await rs.Content.ReadAsStringAsync();
                                            break;
                                        default:
                                            var rq = new HttpRequestMessage();
                                            rq.Method = HttpMethod.Get;
                                            var requestStr = $"http://fvn-s-web01:5000/api/Email/Alarmwrongbox?line={line}&type=Wrong%20barcode%20content%3A%20{barcode}";
                                            rq.RequestUri = new Uri(requestStr);
                                            var rs = await _httpClient.SendAsync(rq);
                                            var responseBody = await rs.Content.ReadAsStringAsync();
                                            break;
                                    }
                                    // if(barcode == "time out"){
                                    //     var rq = new HttpRequestMessage();
                                    //     rq.Method = HttpMethod.Get;
                                    //     var requestStr = $"http://fvn-s-web01:5000/api/Email/Alarmwrongbox?line={line}&type=Time%20Out";
                                    //     rq.RequestUri = new Uri(requestStr);
                                    //     var rs = await _httpClient.SendAsync(rq);
                                    //     var responseBody = await rs.Content.ReadAsStringAsync();
                                    // }
                                }
                                await Task.Delay(500);
                                controller.Write(pinCheckFail, PinValue.High);
                                Console.Write("Wrong Box Number! ");
                                scan = 3;

                            }
                            else
                            {
                                await Task.Delay(500);
                                controller.Write(pinCheckPass, PinValue.High);
                                Console.Write("correct Box Number! ");
                                scan = 1;
                            }
                        }
                    }
                    else
                    {
                        controller.Write(pinCheckPass, PinValue.Low);
                        if (waiting == false)
                        {
                            count = 0;

                            if (scan == 0)
                            {
                                await Task.Delay(500);
                                controller.Write(pinCheckFail, PinValue.High);
                            }
                        }
                        scan = 0;
                        barcode = "";
                        waiting = true;
                    }
                }
                else
                {
                    boxNumber = "";
                    string val = "";
                    controller.Write(pinReady, PinValue.Low);
                    controller.Write(pinCheckPass, PinValue.Low);
                    controller.Write(pinCheckFail, PinValue.Low);
                    controller.Write(pinPower, PinValue.High);
                    Console.Write("Enter Line: ");
                    val = Console.ReadLine();
                    if (val.Length > 0 && val != "")
                    {
                        barcode = val;
                        line = barcode;
                        var rq = new HttpRequestMessage();
                        rq.Method = HttpMethod.Post;
                        rq.Content = new StringContent($"\"{barcode}\"", Encoding.UTF8, "application/json");
                        // var requestStr = $"http://fvn-s-ws01.friwo.local:5000/api/ProcessLock/FA/GetAdditionalData";
                        var requestStr = $"http://fvn-s-web01:5000/api/ProcessLock/FA/GetAdditionalData";
                        rq.RequestUri = new Uri(requestStr);
                        var rs = await _httpClient.SendAsync(rq);
                        var responseBody = await rs.Content.ReadAsStringAsync();
                        boxNumber = responseBody;
                        Console.WriteLine("Barcode: " + boxNumber);
                        if (boxNumber.Length > 2 && boxNumber != "null")
                        {
                            controller.Write(pinReady, PinValue.High);
                            controller.Write(pinPower, PinValue.Low);
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
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("Service is stopping.");
        await Task.CompletedTask;
    }

}
