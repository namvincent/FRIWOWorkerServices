using System.Net.Http.Headers;

namespace FRIWO.WorkerServices
{
    public class CallApi : BackgroundService
    {
        private readonly ILogger<CallApi> _logger;

        private HttpClient httpClient;

        public CallApi(ILogger<CallApi> logger)
        {
            _logger = logger;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            httpClient = new HttpClient();
            return base.StartAsync(cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            return base.StopAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://10.100.10.81:5000");
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    var unit = "1844705-0000871-239416-001";
                    if (unit != null)
                    {
                        var response = await client.PostAsync("api/ProcessLock/LaserTrimming/InsertPASSAsync?unit=" + unit, null);
                        Console.WriteLine("Called Api " + response.Headers.ToString());

                        if (response.IsSuccessStatusCode)
                        {
                            Console.WriteLine(response.StatusCode.ToString());
                        }
                        else
                        {
                            Console.WriteLine(response.StatusCode.ToString());
                        }
                    }
                }
                await Task.Delay(10000);


            }
            await Task.CompletedTask;
        }
    }
}
