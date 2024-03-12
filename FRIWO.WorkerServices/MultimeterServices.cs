using System.Device.Gpio;
using System.Text;
using System.IO.Ports;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Iot.Device.Mcp23xxx;
using Windows.Networking;

namespace FRIWO.WorkerServices
{
    public class MultimeterServices
    {
        SerialPort myport;

        int counter = 0;
        int firstTest = 0;
        string portName = "";
        MultimeterResult testResult = new MultimeterResult();
        string[] serialList = new string[4]{"/dev/ttyUSB0",
                "/dev/ttyUSB1",
                "/dev/ttyUSB2",
                "/dev/ttyUSB3"};

        public SerialPort SerialPort
        {
            get; set;
        }
        public void OpenSerialPort()
        {
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
            myport = new SerialPort(portName);

            myport.ReadTimeout = 50000;
            myport.WriteTimeout = 50000;
            testResult = new MultimeterResult();
            myport.Open();
            for (int i = 0; i < 350; i++)
            {
                myport.WriteLine("COMP?");
                myport.WriteLine("VAL1?");
            }
            Dispose();
        }
        public void Dispose()
        {
            myport.Close();
            myport.Dispose();
        }
        public delegate void ChangeEventHandler(MultimeterResult Item);
        public event ChangeEventHandler OnValueChanged;
        public void ExecuteAsync(object sender, SerialDataReceivedEventArgs e)
        {
           
            Console.WriteLine("Start blinking LED");
      
                try
                {
                    string? val;
                    
                    int? stationCheck = 0;
                    Console.Write("Enter barcode: ");                    
                    val = Console.ReadLine();
                    
                    if(string.IsNullOrEmpty(portName)){
                        foreach(var item in serialList){
                            SerialPort test = new  SerialPort(item);
                            try{
                            test.Open();
                            Console.WriteLine(item);
                            if(test.IsOpen == true){
                                portName = item;
                                test.Close();
                                break;
                            }
                            }catch(Exception){}
                        }
                    }
                    myport = new  SerialPort(portName);
                    List<MultimeterResult> resultData = new();
                    myport.ReadTimeout = 50000;
                    myport.WriteTimeout = 50000;
                    myport.Open ();        
                    string measure = "";            
                    for(int i = 0;i<150;i++) {
                        myport.WriteLine("COMP?");
                        myport.WriteLine("VAL1?");    
                        
                        string serialRead =   myport.ReadLine();
                        if (!serialRead.StartsWith(".")&&!string.IsNullOrEmpty(serialRead)&&!serialRead.Contains(">")&&!serialRead.Contains("!>") && !serialRead.Contains("=>")  && !serialRead.Contains("?>") && !serialRead.Contains("PASS") && !serialRead.Contains("P") && !serialRead.Contains("PAPASS") && !serialRead.Contains("LO") && !serialRead.Contains("HI") )
                        {
                            measure = serialRead;
                        }
                        Console.WriteLine(serialRead);
                        if(serialRead.Contains("PASS")){
                            testResult.Status = "PASS";
                            myport.ReadLine();
                            testResult.Measure = myport.ReadLine();                              
                            resultData.Add(testResult);  
                            Console.WriteLine(resultData[0].Measure);        
                            Console.WriteLine(resultData[0].Status); 
                            break;
                        }else{
                            testResult.Status = "FAIL";
                            testResult.Measure = measure;
                        }
                        Task.Delay(100);                                        
                                         
                                
                    }
                    myport.Close();
                    Console.WriteLine(testResult.Status);                    
                    Console.WriteLine(testResult.Measure);
                    OnValueChanged?.Invoke(testResult);
                }catch(Exception){}               
                
           
        }
    }
}