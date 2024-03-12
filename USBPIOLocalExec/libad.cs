/* C# Libad Interface
 *
 * (c) 2015 BMC Messsysteme GmbH
 * 
 */

using System;
using System.Runtime.InteropServices;

namespace USBPIOLocalExec
{
  static class LIBAD
  {
    public const int AD_CHA_TYPE_MASK            = unchecked((int) 0xff000000);

    public const int AD_CHA_TYPE_ANALOG_IN       = 0x01000000;
    public const int AD_CHA_TYPE_ANALOG_OUT      = 0x02000000;
    public const int AD_CHA_TYPE_DIGITAL_IO      = 0x03000000;
    public const int AD_CHA_TYPE_SYNC            = 0x05000000;
    public const int AD_CHA_TYPE_ROUTE           = 0x06000000;
    public const int AD_CHA_TYPE_CAN             = 0x07000000;
    public const int AD_CHA_TYPE_COUNTER         = 0x08000000;
    public const int AD_CHA_TYPE_ANALOG_COUNTER  = 0x09000000;

    [DllImport("libad4.dll", CallingConvention=CallingConvention.Cdecl)]
    public static extern int ad_open (string name);

    [DllImport("libad4.dll", CallingConvention=CallingConvention.Cdecl)]
    public static extern int ad_close (int ad);

    [DllImport("libad4.dll", CallingConvention=CallingConvention.Cdecl)]
    public static extern int ad_discrete_in (int adh, int cha, int range, ref uint data);

    [DllImport("libad4.dll", CallingConvention=CallingConvention.Cdecl)]
    public static extern int ad_discrete_in64 (int adh, int cha, ulong range, ref ulong data);

    [DllImport("libad4.dll", CallingConvention=CallingConvention.Cdecl)]
    public static extern int ad_discrete_out (int adh, int cha, int range, uint data);

    [DllImport("libad4.dll", CallingConvention=CallingConvention.Cdecl)]
    public static extern int ad_discrete_out64 (int adh, int cha, ulong range, ulong data);

    [DllImport("libad4.dll", CallingConvention=CallingConvention.Cdecl)]
    public static extern int ad_get_line_direction (int adh, int cha, ref uint mask);

    [DllImport("libad4.dll", CallingConvention=CallingConvention.Cdecl)]
    public static extern int ad_set_line_direction (int adh, int cha, uint mask);

    //[DllImport("libad4.dll", CallingConvention=CallingConvention.Cdecl)]
    //unsafe public static extern int ad_discrete_inv (int adh, int chac, int[] chav, ulong[] rangev, ulong[] datav);

    //[DllImport("libad4.dll", CallingConvention=CallingConvention.Cdecl)]
    //unsafe public static extern int ad_discrete_outv (int adh, intt chac, int[]t chav, ulong[] rangev, ulong[] datav);

    [DllImport("libad4.dll", CallingConvention=CallingConvention.Cdecl)]
    public static extern int ad_sample_to_float (int adh, int cha, int range, uint data, ref float dbl);

    [DllImport("libad4.dll", CallingConvention=CallingConvention.Cdecl)]
    public static extern int ad_sample_to_float64 (int adh, int cha, ulong range, ulong data, ref double dbl);

    [DllImport("libad4.dll", CallingConvention=CallingConvention.Cdecl)]
    public static extern int ad_float_to_sample (int adh, int cha, int range, float dbl, ref uint data);

    [DllImport("libad4.dll", CallingConvention=CallingConvention.Cdecl)]
    public static extern int ad_float_to_sample64 (int adh, int cha, int range, double dbl, ref ulong data);

    [DllImport("libad4.dll", CallingConvention=CallingConvention.Cdecl)]
    public static extern int ad_analog_in (int adh, int cha, int range, ref float data);

    [DllImport("libad4.dll", CallingConvention=CallingConvention.Cdecl)]
    public static extern int ad_analog_out (int adh, int cha, int range, float data);

    // what to store
    public const int AD_STORE_DISCRETE       = 0x0001;
    public const int AD_STORE_AVERAGE        = 0x0002;
    public const int AD_STORE_MIN            = 0x0004;
    public const int AD_STORE_MAX            = 0x0008;
    public const int AD_STORE_RMS            = 0x0010;

    // trigger mode
    public const int AD_TRG_NONE             = 0x00;
    public const int AD_TRG_POSITIVE         = 0x01;
    public const int AD_TRG_NEGATIVE         = 0x02;
    public const int AD_TRG_INSIDE           = 0x03;
    public const int AD_TRG_OUTSIDE          = 0x04;
    public const int AD_TRG_DIGITAL          = 0x80;
    public const int AD_TRG_NEVER            = 0xff;

    // scan flags
    public const int AD_SF_SCANNING          = 0x00000001;   // Is set while device is scanning.
    public const int AD_SF_TRIGGER           = 0x00000002;   // Scan has triggered.
    public const int AD_SF_SAMPLES           = 0x00010000;   // Scan buffers handled internally.



    // Scan channel descriptor.
    // Defines scan settings of a single channel.
    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    public struct ad_scan_cha_desc
    {
      public int cha;              // channel type and id
      public int range;            // range number (driver specific)
      public int store;            // what to sample
      public int ratio;            // scan ratio
      public uint zero;            // physical 0.0
      public byte trg_mode;        // trigger mode
      public byte alarm_mode;      // alarm mode
      byte sc_res1;
      byte sc_res2;
      public uint trg_par1;        // trigger parameters */
      public uint trg_par2;
      int samples_per_run;         // number of samples per run
      uint alarm_par1;             // alarm parameters
      uint alarm_par2;
      uint sc_res3;
      uint sc_res4;
      uint sc_res5;
      uint sc_res6;
      uint sc_res7;
    }

    // Scan descriptor.
    // Defines global scan settings.
    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    public struct ad_scan_desc
    {
      public double sample_rate;   // sampling rate (sec)
      public double clock_rate;    // ref clock rate (sec)
      public ulong prehist;        // number of samples prehistory
      public ulong posthist;       // number of samples posthistory
      public uint ticks_per_run;   // number of ticks per run
      public uint bytes_per_run;   // bytes per run
      public uint samples_per_run; // samples per run
      public uint flags;           // scan flags 
      uint sd_res1;
      uint sd_res2;
      uint sd_res3;
      uint sd_res4;
      uint sd_res5;
      uint sd_res6;
      uint sd_res7;
      uint sd_res8;
      uint sd_res9;
      uint sd_res10;
      uint sd_res11;
      uint sd_res12;
      }

    // Current scan state.
    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    public struct ad_scan_state
    {
      public int flags;
      public int runs_pending;
      public long posthist;
    }

    [DllImport("libad4.dll", CallingConvention=CallingConvention.Cdecl)]
    public static extern int ad_calc_run_size (int adh, ref ad_scan_desc scan_desc, uint chac, [In,Out,MarshalAs(UnmanagedType.LPArray, SizeParamIndex=2)] ad_scan_cha_desc[] chav);

    public static int ad_calc_run_size (int adh, ref ad_scan_desc scan_desc, ref ad_scan_cha_desc[] chav)
    {
      return ad_calc_run_size (adh, ref scan_desc, (uint) chav.Length, chav);
    }

    [DllImport("libad4.dll", CallingConvention=CallingConvention.Cdecl)]
    public static extern int ad_start_scan (int adh, ref ad_scan_desc scan_desc, uint chac, [In,Out,MarshalAs(UnmanagedType.LPArray, SizeParamIndex=2)] ad_scan_cha_desc[] chav);

    public static int ad_start_scan (int adh, ref ad_scan_desc scan_desc, ref ad_scan_cha_desc[] chav)
    {
      return ad_start_scan (adh, ref scan_desc, (uint) chav.Length, chav);
    }

    [DllImport("libad4.dll", CallingConvention=CallingConvention.Cdecl)]
    public static extern int ad_stop_scan (int adh, ref int result);

    [DllImport("libad4.dll", CallingConvention=CallingConvention.Cdecl)]
    public static extern int ad_get_next_run_f (int adh, ref ad_scan_state state, ref uint run, [Out,MarshalAs(UnmanagedType.LPArray, SizeParamIndex=4)] float[] samplev, uint samplec);

    public static int ad_get_next_run_f (int adh, ref ad_scan_state state, ref uint run, ref float[] samplev)
    {
      return ad_get_next_run_f (adh, ref state, ref run, samplev, (uint) samplev.Length);
    }

    [DllImport("libad4.dll", CallingConvention=CallingConvention.Cdecl)]
    public static extern int ad_get_next_run_f64 (int adh, ref ad_scan_state state, ref uint run, [Out,MarshalAs(UnmanagedType.LPArray, SizeParamIndex=4)] double[] samplev, uint samplec);

    public static int ad_get_next_run_f64 (int adh, ref ad_scan_state state, ref uint run, ref double[] samplev)
    {
      return ad_get_next_run_f64 (adh, ref state, ref run, samplev, (uint) samplev.Length);
    }

    // Current scan position
    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    public struct ad_scan_pos
    {
      public uint run;
      public uint offset;
    }

    [DllImport("libad4.dll", CallingConvention=CallingConvention.Cdecl)]
    public static extern int ad_get_trigger_pos (int adh, ref ad_scan_pos pos);


    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    private struct ad_par
    {
    }

    [DllImport("libad4.dll", CallingConvention=CallingConvention.Cdecl)]
    private static extern int ad_ioctl (int adh, uint ioc, ref ad_par par, int size);

    [DllImport("kernel32.dll")]
    private static extern uint GetLastError();

    public static int errno
    {
      get { return (int) GetLastError (); }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    public struct ad_counter_mode
    {
      public int cha;                 // counter channel
      public byte mode;               // counter mode
      public byte mux_a;              // a input mux setting
      public byte mux_b;              // b input mux setting
      public byte mux_rst;            // reset input mux setting

      public ushort flags;            // control flags

      public uint capt_a;             // capture registers
      public uint capt_b;

      public byte eact_a;             // event action on capt_a match
      public byte eact_b;             // event action on capt_b match
    }

    private const uint AD_SET_COUNTER_MODE         = 0xb3800001;
    private const uint AD_GET_COUNTER_MODE         = 0xb3800002;

    [DllImport("libad4.dll", CallingConvention=CallingConvention.Cdecl, EntryPoint = "ad_ioctl")]
    private static extern int ad_ioctl_counter_mode (int adh, uint ioc, ref ad_counter_mode par, int size);

    public static int ad_set_counter_mode (int adh, ref ad_counter_mode mode)
    {
      return ad_ioctl_counter_mode (adh, AD_SET_COUNTER_MODE, ref mode, Marshal.SizeOf(mode));
    }

    public static int ad_get_counter_mode (int adh, ref ad_counter_mode mode)
    {
      return ad_ioctl_counter_mode (adh, AD_GET_COUNTER_MODE, ref mode, Marshal.SizeOf(mode));
    }

    public const byte AD_CNT_COUNTER      = 0;         // counter mode
    public const byte AD_CNT_UPDOWN       = 1;         // up/down counter mode
    public const byte AD_CNT_QUAD_DECODER = 4;         // quadrature decoder
    public const byte AD_CNT_PULSE_TIME   = 5;

    public const ushort AD_CNT_INV_A      = 0x0001;   // invert A input
    public const ushort AD_CNT_INV_B      = 0x0002;   // invert B input
    public const ushort AD_CNT_INV_RST    = 0x0004;   // invert reset input
    public const ushort AD_CNT_ENABLE_RST = 0x0008;   // enable reset input

    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    public struct ad_product_info
    {
      public uint serial;             // serial number
      public uint fw_version;         // firmware version

      [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 32)]
      public string model;            // model name

      [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 256)]
      public byte[] res;
    }

    [DllImport("libad4.dll", CallingConvention=CallingConvention.Cdecl, EntryPoint = "ad_get_product_info")]
    private static extern int ad_read_product_info (int adh, int id, ref ad_product_info info, int size);

    public static int ad_get_product_info (int adh, int id, ref ad_product_info info)
    {
      return ad_read_product_info (adh, id, ref info, Marshal.SizeOf(info));
    }

    public static byte AD_MAJOR_VERS (uint vers)
    {
      return (byte) (vers >> 24);
    }

    public static byte AD_MINOR_VERS (uint vers)
    {
      return (byte) (vers >> 16);
    }

    public static ushort AD_BUILD_VERS (uint vers)
    {
      return (ushort) (vers >> 0);
    }

  }
}

