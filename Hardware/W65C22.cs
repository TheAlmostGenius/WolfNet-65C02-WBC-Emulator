using System;
using System.Timers;

namespace Hardware
{
    /// <summary>
    /// An implementation of a W65C22 VIA.
    /// </summary>
    [Serializable]
    public class W65C22
    {
        public static class ACRdata
        {
            #region Fields
            public static byte T1TC = (byte)(1 << 7);
            public static byte T2TC = (byte)(1 << 6);
            #endregion
        }

        public static class IFRdata
        {
            #region Fields
            public static byte T2 = (1 << 5);
            public static byte T1 = (1 << 6);
            public static byte INT = (1 << 7);
            #endregion
        }

        public static class IERdata
        {
            #region Fields
            public static byte T2 = (byte)(1 << 5);
            public static byte T1 = (byte)(1 << 6);
            public static byte EN = (byte)(1 << 7);
            #endregion
        }

        #region Fields
        public static readonly bool T1IsIRQ = false;
        public static readonly bool T2IsIRQ = true;
        public static int T1CL = 0xD024;
        public static int T1CH = 0xD025;
        public static int T2CL = 0xD028;
        public static int T2CH = 0xD029;
        public static int ACR = 0xD02B;
        public static int IFR = 0xD02D;
        public static int IER = 0xD02E;
        #endregion

        #region Properties
        private ushort Offset { get; set; }

        /// <summary>
        /// T1 timer control
        /// </summary>
        public static bool T1TimerControl
        {
            get { return T1Object.AutoReset; }
            set { T1Object.AutoReset = value; }
        }

        /// <summary>
        /// T2 timer control.
        /// </summary>
        public static bool T2TimerControl
        {
            get { return T2Object.AutoReset; }
            set { T2Object.AutoReset = value; }
        }

        /// <summary>
        /// Enable or check whether timer 1 is enabled or not.
        /// </summary>
        public static bool T1IsEnabled
        {
            get { return T1Object.Enabled; }
            set { T1Object.Enabled = value; }
        }

        /// <summary>
        /// Enable or check whether timer 2 is enabled or not.
        /// </summary>
        public static bool T2IsEnabled
        {
            get { return T2Object.Enabled; }
            set { T2Object.Enabled = value; }
        }

        /// <summary>
        /// Set or check the timer 1 interval.
        /// </summary>
        public static double T1Interval
        {
            get { return (int)(W65C02.ReadMemoryValueWithoutCycle(T1CL) | (W65C02.ReadMemoryValueWithoutCycle(T1CH) << 8)); }
        }

        /// <summary>
        /// Set or check the timer 2 interval.
        /// </summary>
        public static double T2Interval
        {
            get { return (int)(W65C02.ReadMemoryValueWithoutCycle(T2CL) | (W65C02.ReadMemoryValueWithoutCycle(T2CH) << 8)); }
        }

        /// <summary>
        /// Set or get the timer 1 object.
        /// </summary>
        public static Timer T1Object { get; set; }

        /// <summary>
        /// Set or get the timer 2 object.
        /// </summary>
        public static Timer T2Object { get; set; }
        #endregion

        #region Public Methods
        public W65C22()
        {
            
        }

        public W65C22(byte offset)
        {
            if (offset > MemoryMap.DeviceArea.Length)
                throw new ArgumentException(String.Format("The offset: {0} is greater than the device area: {1}", offset, MemoryMap.DeviceArea.Length));
            Offset = (ushort)(MemoryMap.DeviceArea.Offset & offset);
            T1Init(1000);
            T2Init(1000);
        }

        public void Reset()
        {
            T1TimerControl = false;
            T1IsEnabled = false;
            T2TimerControl = false;
            T2IsEnabled = false;
        }

        public void Init(double timer)
        {
            T1Init(timer);
            T2Init(timer);
        }

        /// <summary>
        /// T1 counter initialization routine.
        /// </summary>
        /// 
        /// <param name="value">Timer initialization value in milliseconds</param>
        public static void T1Init(double value)
        {
            T1Object = new System.Timers.Timer(value);
            T1Object.Start();
            T1Object.Elapsed += OnT1Timeout;
            T1TimerControl = true;
            T1IsEnabled = false;
        }

        /// <summary>
        /// T2 counter initialization routine.
        /// </summary>
        /// 
        /// <param name="value">Timer initialization value in milliseconds</param>
        public static void T2Init(double value)
        {
            T2Object = new System.Timers.Timer(value);
            T2Object.Start();
            T2Object.Elapsed += OnT2Timeout;
            T2TimerControl = true;
            T2IsEnabled = false;
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Called whenever System.Timers.Timer event elapses
        /// </summary>
        /// 
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnT1Timeout(object sender, ElapsedEventArgs e)
        {
            if (W65C02.isRunning)
            {
                if (T1IsEnabled)
                {
                    W65C02.WriteMemoryValueWithoutCycle(IFR, (byte)(IFRdata.T1 & IFRdata.INT));
                    if (T1IsIRQ)
                    {
                        W65C02.InterruptRequest();
                    }
                    else
                    {
                        W65C02.TriggerNmi = true;
                    }
                }
            }
        }

        /// <summary>
        /// Called whenever System.Timers.Timer event elapses
        /// </summary>
        /// 
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnT2Timeout(object sender, ElapsedEventArgs e)
        {
            if (W65C02.isRunning)
            {
                if (T2IsEnabled)
                {
                    W65C02.WriteMemoryValueWithoutCycle(IFR, (byte)(IFRdata.T2 & IFRdata.INT));
                    if (T2IsIRQ)
                    {
                        W65C02.InterruptRequest();
                    }
                    else
                    {
                        W65C02.TriggerNmi = true;
                    }
                }
            }
        }
        #endregion
    }
}