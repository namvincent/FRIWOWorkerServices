using FRIWOCenter.Data.SerialPorts;
using FRIWOCenter.Data.TRACE;
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
    public class PIOService : BackgroundService, IService
    {
        // Libad Digital I/O Example
        //
        // Example showing how to set and get the state of some digital inputs
        // using bmcm's LIBAD4.


        // Line Direction Masks
        const uint OUTPUT = 0x0000;
        const uint INPUT  = 0xffff;

        public int Counter { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public ILogger Logger => throw new NotImplementedException();

        // Get/Set Digital input(s)/output(s).
        static void
        //do_digital_io(string driver, uint dir, int[] chav, uint[] datav)
        //{
        //    // Open DAQ system.
        //    int adh = LIBAD.ad_open (driver);
        //    if(adh == -1)
        //    {
        //        Console.WriteLine("failed to open {0}: err = {1}", driver, LIBAD.errno);
        //        return;
        //    }
        //    // Set Set/get digital i/o.
        //    for(int i = 0; i < chav.Length; i++)
        //    {
        //        uint mask;
        //        int rc;
        //        // Setup port's direction.
        //        LIBAD.ad_set_line_direction(adh, LIBAD.AD_CHA_TYPE_DIGITAL_IO | chav[i], dir);
        //        // Either read from port or write to port.
        //        if(dir == INPUT)
        //            rc = LIBAD.ad_discrete_in(adh, LIBAD.AD_CHA_TYPE_DIGITAL_IO | chav[i], 0, ref datav[i]);
        //        else
        //            rc = LIBAD.ad_discrete_out(adh, LIBAD.AD_CHA_TYPE_DIGITAL_IO | chav[i], 0, datav[i]);
        //        if(rc == 0)
        //        {
        //            Console.Write("cha {0,2}: port = 0x{1,4X}, lines 16..1 =  ", chav[i], datav[i]);
        //            // Separate lines of that port.
        //            for(mask = 0x8000; mask != 0; mask >>= 1)
        //            {
        //                if((datav[i] & mask) != 0)
        //                    Console.Write("1");
        //                else
        //                    Console.Write("0");
        //            }
        //            Console.WriteLine();
        //        }
        //        else
        //            Console.WriteLine("error: failed to write cha {0}: err = {1}", chav[i], rc);
        //    }
        //    // Close DAQ system again.
        //    LIBAD.ad_close(adh);
        //}
        //// Show usage.
        //static void
        usage()
        {
            Console.WriteLine("usage: digital_io <driver> [-i] <cha1> .. <chan>");
            Console.WriteLine("       digital_io <driver> -o [<cha1>,]<val1> .. [<chan>,]<valn>");
            Console.WriteLine("  <driver>           string to pass to ad_open()");
            Console.WriteLine("                     - will prompt for name");
            Console.WriteLine("  <cha1> .. <chan>   number of digital port");
            Console.WriteLine("  <val1> .. <valn>   value to set digital output to");
        }

        public int Execute(string[] argv)
        {
            if(argv.Length > 0)
            {
                // First command line argument is the DAQ's name.
                // If "-" is passed, then let's read the name from
                // the console.
                string name = argv[0];
                if(argv[0] == "-")
                {
                    Console.Write("data acquisition system to open: ");
                    name = Console.ReadLine();
                }
                // Direction defaults to input, but may get overridden by -o
                // on the command line.
                int start = 1;
                uint dir = INPUT;
                if(argv.Length > 1)
                {
                    if(argv[start] == "-o")
                    {
                        dir = OUTPUT;
                        start++;
                    }
                    else if(argv[start] == "-i")
                    {
                        dir = INPUT;
                        start++;
                    }
                }
                // Convert remaining command line arguments into channel
                // numbers and values. Add those to the appropriate array.
                int[] chav = new int[argv.Length - start];
                uint[] datav = new uint[argv.Length - start];
                for(int i = start; i < argv.Length; i++)
                {
                    if(dir == INPUT)
                    {
                        // Input case, parse channel numbers only.
                        chav[i - start] = int.Parse(argv[i]);
                    }
                    else
                    {
                        // Output case, parse (optional) channel number
                        // and value to output.
                        int delim = argv[i].IndexOf (',');
                        if(delim >= 0)
                        {
                            chav[i - start] = int.Parse(argv[i].Substring(0, delim));
                            datav[i - start] = uint.Parse(argv[i].Substring(delim + 1));
                        }
                        else
                        {
                            chav[i - start] = 1;
                            datav[i - start] = uint.Parse(argv[i]);
                        }
                    }
                }
                // Set/get digital i/o and print results
                // to the console.
                //do_digital_io(name, dir, chav, datav);
                if(argv[0] == "-")
                {
                    Console.WriteLine("press return to continue...");
                    Console.ReadLine();
                }
                return 0;
            }
            else
            {
                usage();
                return 1;
            }
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            throw new NotImplementedException();
        }
    }

}
