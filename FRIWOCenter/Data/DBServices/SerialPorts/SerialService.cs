using FRIWOCenter.Data.SerialPorts;
using FRIWOCenter.Data.TRACE;
using FRIWOCenter.Model;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace FRIWOCenter.DBServices.SerialPorts
{
    public class SerialService : BackgroundService, IService
    {
        public delegate void ChangeEventHandler(int numberArg);
        public event ChangeEventHandler OnValueChanged;

        public int Counter { get; set; }
        SerialPort serialPort = null;
        int time = 0;
        private ObservableCollection<SerialDataItem> _asciiDataCollection;
        public object CollectionLock;

        public ObservableCollection<SerialDataItem> AsciiDataCollection
        {
            get
            {
                lock (CollectionLock)
                {
                    return _asciiDataCollection;
                }
            }
        }

        private DateTime startTime;
        private Random random = new Random();
        private bool firstCmd;

        public SerialService(ILoggerFactory loggerFactory)
        {
            Logger = loggerFactory.CreateLogger<SerialService>();
            CreateDataCollection();
            Counter = 0;

            PortName = "COM1";
            SerialPort = new SerialPort(PortName);

            SerialPort.BaudRate = 9600;
            SerialPort.Parity = System.IO.Ports.Parity.None;
            SerialPort.StopBits = System.IO.Ports.StopBits.One;
            SerialPort.DataBits = 8;
            SerialPort.Handshake = System.IO.Ports.Handshake.None;
            SerialPort.ReadTimeout = 500;
            SerialPort.WriteTimeout = 500;

            SerialPort.NewLine = Environment.NewLine;

            SerialPortError = "";

        }

        public string PortName { get; set; } = "COM9";
        public string SerialPortError { get; set; }

        private void CreateDataCollection()
        {
            CollectionLock = new object();
            _asciiDataCollection = new ObservableCollection<SerialDataItem>();

        }

        //public List<string> GetPortsList()
        //{
        //    return SerialPort.GetPortNames().ToList();
        //}

        public void OpenSerialPort()
        {
            if (!SerialPort.IsOpen)
            {
                try
                {
                    SerialPort.PortName = PortName;
                    SerialPort.Open();
                    SerialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
                    SerialPortError += $"{DateTime.Now:HH:mm:ss}: Port Open\t";
                    SerialPort.RtsEnable = true;

                }
                catch (Exception ex)
                {
                    SerialPortError += $"\n{DateTime.Now:HH:mm:ss}: ERROR-{ex.Message}\t";
                }
            }
        }

        public void CloseSerialPort()
        {
            if (SerialPortIsOpen())
            {
                try
                {
                    SerialPort.DataReceived -= DataReceivedHandler;
                    SerialPort.Close();
                    SerialPortError += $"{DateTime.Now:HH:mm:ss}: Port Closed\t";
                }
                catch (Exception ex)
                {
                    SerialPortError += $"{DateTime.Now:HH:mm:ss}: ERROR-{ex.Message}\t";
                }
            }
        }

        public bool SerialPortIsOpen()
        {
            return SerialPort.IsOpen;
        }

        public async Task<List<Unit>> SendData(List<Unit> results, System.Timers.Timer timer)
        {
            
            SerialPortError = "Start testing";
            if (!SerialPortIsOpen())
                OpenSerialPort();
            else
            {
                CloseSerialPort();
                OpenSerialPort();
            }

            timer.Start();
            SerialPort.WriteTimeout = 500;
            SerialPort.ReadTimeout = 500;
            firstCmd = true;
            SerialPort.DataReceived += DataReceivedHandler;
            SerialPort.WriteLine("SYST:TCON:SCR ON");
            SerialPort.WriteLine("SAF:RES:AREP ON");
            SerialPort.WriteLine("SAF:STOP");
            SerialPort.WriteLine("SAF:STAR");

            Debug.WriteLine($"Start_Test: {DateTime.Now.ToLongTimeString()}");
            int count = 0;
            while (firstCmd)
            {
                await Task.Delay(100);
                Debug.WriteLine(timer.ToString());
                count++;
            }
            timer.Stop();
            SerialPort.WriteTimeout = 500;
            SerialPort.WriteLine("SAF:RES:AREP OFF");
            for (int i = 0; i < results.Count; i++)
            {
                SerialPort.ReadTimeout = 500;
                SerialPort.WriteLine($"SAF:CHAN00{i + 1}:RES:ALL?");
                var str = Regex.Match(SerialPort.ReadLine(), @"\d+").Value;
                Debug.WriteLine(str);

                results[i].TestResult.Result = Int32.Parse(str) == 116 ? true : false;
                
                results[i].IsTested = true;
                results[i].TestTime = DateTime.Now;
                Debug.WriteLine($"{results[i].GetBarcode() }  { results[i].TestResult.Result}");

            }
            
            for (int i = 0; i < results.Count; i++)
            {
                SerialPort.ReadTimeout = 500;
                SerialPort.WriteLine($"SAF:CHAN00{i + 1}:RES:ALL:MMET?");
                var str = SerialPort.ReadLine();
                Debug.WriteLine(str);

                results[i].TestResult.Data = str;
                Debug.WriteLine($"{results[i].TestResult.Data }  { results[i].TestResult.Result}");
                if(i == results.Count - 1)
                {
                    //SerialPort.ReadTimeout = 500;
                    SerialPort.WriteLine("SAF:STOP");
                    //var strEnd = SerialPort.ReadLine();
                    //Debug.WriteLine(strEnd);
                    SerialPort.DataReceived -= DataReceivedHandler;
                    //SerialPort.Close();
                    CloseSerialPort();
                }
            }
            FirstCmd = false;

            //await Task.Run(() =>
            //{
            //    SerialPort.ReadTimeout = 500;
            //    SerialPort.WriteLine("SAF:STOP");
            //    SerialPort.DataReceived -= DataReceivedHandler;
            //    SerialPort.Close();
            //    CloseSerialPort();
            //});

            return results;

            //var str = serialPort.ReadLine();
            //Debug.WriteLine(str);
            //str.ToList<string>()
            //serialPort.WriteLine("SAF:CHAN001:RES:ALL:MMET?");
            ////serialPort.ReadLine();

            //serialPort.WriteLine("SAF:CHAN002:RES:ALL:MMET?");
            ////serialPort.ReadLine();

            //serialPort.WriteLine("SAF:CHAN003:RES:ALL:MMET?");
            ////serialPort.ReadLine();

            //serialPort.WriteLine("SAF:CHAN004:RES:ALL:MMET?");
            //str += serialPort.ReadLine();

            //Console.WriteLine(str);
        }



        public void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {

            try
            {
                SerialPort.DataReceived -= DataReceivedHandler;
                //serialPort.ReadTimeout = 500;
                //serialPort.ReadLine();
                firstCmd = false;
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                Logger.LogInformation("Read timeout!");
            }
        }

        public ILogger Logger { get; }
        public bool FirstCmd { get => firstCmd; set => firstCmd = value; }
        public SerialPort SerialPort { get => serialPort; set => serialPort = value; }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            int sampleTime = 100;
            Logger.LogInformation("SerialService is starting.");

            stoppingToken.Register(() => Logger.LogInformation("SerialService is stopping."));

            startTime = DateTime.Now;

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    //Logger.LogInformation("ServiceA is doing background work. Iterations:{counter}", Counter);

                    if (SerialPort.IsOpen)
                    {
                        var serialData = SerialPort.ReadLine();  //blocks until cr received, really need a time out on this
                        Debug.WriteLine($"Received: {serialData}");
                        var readData = new SerialDataItem(Counter, serialData);

                        Counter++;

                        lock (CollectionLock)
                        {
                            if (_asciiDataCollection.Count >= 15)
                            {
                                //remove first record
                                _asciiDataCollection.RemoveAt(0);
                            }

                            _asciiDataCollection.Add(readData);
                        }

                        //send ui event
                        OnValueChanged?.Invoke(Counter);
                    }
                    else
                    {
                        SerialPort.ReadTimeout = 100;
                        sampleTime = 5;
                    }
                }
                catch (System.TimeoutException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    Logger.LogInformation($"SerialService exception: {ex.Message}");
                }

                await Task.Delay(TimeSpan.FromMilliseconds(sampleTime), stoppingToken);

            }

            Logger.LogInformation("SerialService has stopped.");
        }
    }

}
