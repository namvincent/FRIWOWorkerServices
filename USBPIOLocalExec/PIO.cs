// Libad Analog Output Example
//
// Example showing how to set the voltage at some analog outputs
// using bmcm's LIBAD4.

using System;
using System.CodeDom;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using USBPIOLocalExec;


static class PIO
{
    // Set analog output(s).

    static void
    write_analog_outputs(string driver)
    {
        uint p1 = uint.Parse($"7fffff{driver.Split('_')[0]}",System.Globalization.NumberStyles.AllowHexSpecifier);
        Console.WriteLine($"7fffff{driver.Split('_')[0]}");
        uint p2 = uint.Parse($"7fffff{driver.Split('_')[1]}",System.Globalization.NumberStyles.AllowHexSpecifier);
        Console.WriteLine($"7fffff{driver.Split('_')[1]}");
        // Open DAQ system.
        int adh = LIBAD.ad_open ("usb-pio");
        if(adh == -1)
        {
            Console.WriteLine("failed to open {0}: err = {1}", driver, LIBAD.errno);
            return;
        }

        // Set analog output to some voltage.
        //for(int i = 0; i < 1; i++)
        //{
            int rc;

            var res = LIBAD.ad_set_line_direction(adh, LIBAD.AD_CHA_TYPE_DIGITAL_IO | 0x0001 , 0x0000);
            res = LIBAD.ad_set_line_direction(adh, LIBAD.AD_CHA_TYPE_DIGITAL_IO | 0x0002 , 0x0000);

            rc = LIBAD.ad_discrete_out(adh, LIBAD.AD_CHA_TYPE_DIGITAL_IO | 0x0001, 0, p1);
            rc = LIBAD.ad_discrete_out(adh, LIBAD.AD_CHA_TYPE_DIGITAL_IO | 0x0002, 0, p2);

            //rc = LIBAD.(adh, LIBAD.AD_CHA_TYPE_DIGITAL_IO | 1,1, 5);
            //if(rc == 0)
            //    Debug.WriteLine("cha {0,2}: {1,7:##0.000} V", LIBAD.AD_CHA_TYPE_DIGITAL_IO | 3, 0xffffffff);
            //else
            //            Debug.WriteLine("error: failed to write cha {0}: err = {1}", chav[i], rc);
            //await Task.Delay(5000);

            //rc = LIBAD.ad_discrete_out(adh, LIBAD.AD_CHA_TYPE_DIGITAL_IO | 0x0001, 0, 0x7fffff00);
            //rc = LIBAD.ad_discrete_out(adh, LIBAD.AD_CHA_TYPE_DIGITAL_IO | 0x0002, 0, 0x7fffff00);
        //}

        // Close DAQ system again.
        LIBAD.ad_close(adh);
    }

    static void
    read_analog_outputs(ref uint p1,ref uint p2)
    {
        
        // Open DAQ system.
        int adh = LIBAD.ad_open ("usb-pio");
      
        // Set analog output to some voltage.
        //for(int i = 0; i < 1; i++)
        //{
        int rc;

        var res = LIBAD.ad_set_line_direction(adh, LIBAD.AD_CHA_TYPE_DIGITAL_IO | 0x0001 , 0x0000);
        res = LIBAD.ad_set_line_direction(adh, LIBAD.AD_CHA_TYPE_DIGITAL_IO | 0x0002, 0x0000);

        rc = LIBAD.ad_discrete_in(adh, LIBAD.AD_CHA_TYPE_DIGITAL_IO | 0x0001, 0,ref p1);
        rc = LIBAD.ad_discrete_in(adh, LIBAD.AD_CHA_TYPE_DIGITAL_IO | 0x0002, 0,ref p2);

        //rc = LIBAD.(adh, LIBAD.AD_CHA_TYPE_DIGITAL_IO | 1,1, 5);
        //if(rc == 0)
        //    Debug.WriteLine("cha {0,2}: {1,7:##0.000} V", LIBAD.AD_CHA_TYPE_DIGITAL_IO | 3, 0xffffffff);
        //else
        //            Debug.WriteLine("error: failed to write cha {0}: err = {1}", chav[i], rc);
        //await Task.Delay(5000);

        //rc = LIBAD.ad_discrete_out(adh, LIBAD.AD_CHA_TYPE_DIGITAL_IO | 0x0001, 0, 0x7fffff00);
        //rc = LIBAD.ad_discrete_out(adh, LIBAD.AD_CHA_TYPE_DIGITAL_IO | 0x0002, 0, 0x7fffff00);
        //}

        // Close DAQ system again.
        LIBAD.ad_close(adh);
    }


    // Show usage.

    static void
    usage()
    {
        Console.WriteLine("usage: analog_out <driver> [ -r <range> ] [<cha1>,] <voltage1> .. [<chan>,] voltagen");
        Console.WriteLine("  <driver>       string to pass to ad_open()");
        Console.WriteLine("                 - will prompt for name");
        Console.WriteLine("  <range>        range number of analog input");
        Console.WriteLine("  <cha1..n>      number of analog output to set");
        Console.WriteLine("  <voltage1..n>  output voltage");
    }


    // Main entry point.

    private static void
    Main(string[] argv)
    {
        if(argv.Length > 0)
        {
            string param = "";
            ////// First command line argument is the DAQ's name.
            ////// If "-" is passed, then let's read the name from
            ////// the console.
            try
            {
                param = argv[0].Split(':')[1];
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            //if(argv[0] == "-")
            //{
            //    Console.Write("data acquisition system to open: ");
            //    name = Console.ReadLine();
            //}

            //// Range defaults to 0 but may get overridden by
            //// -r on the command line.
            //int start = 1;
            //int range = 0;
            //if(argv.Length > 2)
            //{
            //    if(argv[start] == "-r")
            //    {
            //        range = int.Parse(argv[start + 1]);
            //        start += 2;
            //    }
            //}

            // Convert remaining command line arguments into channel
            // numbers and voltages. Add those to the appropriate array.
            //int[] chav = new int[argv.Length - start];
            //float[] voltagev = new float[argv.Length - start];
            //for(int i = start; i < argv.Length; i++)
            //{
            //    int delim = argv[i].IndexOf (',');
            //    if(delim >= 0)
            //    {
            //        chav[i - start] = int.Parse(argv[i].Substring(0, delim));
            //        voltagev[i - start] = float.Parse(argv[i].Substring(delim + 1));
            //    }
            //    else
            //    {
            //        chav[i - start] = 1;
            //        voltagev[i - start] = float.Parse(argv[i]);
            //    }
            //}
            //name = "usb-pio";

            //Set analog outputs accordingly.
            write_analog_outputs(param);

            //Console.WriteLine(param);
            //Console.WriteLine("press return to continue...");
            //Console.ReadLine();
            uint p1 = 0;
            uint p2 = 0;
            Task.Run(async () =>
            {
                while (true)
                {
                    read_analog_outputs(ref p1, ref p2);

                    Console.WriteLine(((int)p1).ToString());
                    Console.WriteLine(((int)p2).ToString());

                    await Task.Delay(1000);
                }
            });

            while (true)
            {
                Thread.Sleep(1000);
            };
            ////Console.WriteLine(param);
            //Console.WriteLine("press return to continue...");
            //Console.ReadLine();
            //return (int)p2;
        }
        else
        {
            Console.WriteLine("press return to continue...");
            Console.ReadLine();
            //return 1;
        }
    }
}
