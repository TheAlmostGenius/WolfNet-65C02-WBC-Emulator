﻿using System;
using System.Timers;

namespace Hardware
{
    /// <summary>
    /// An implementation of a W65C22 VIA.
    /// </summary>
    [Serializable]
    public class W65C22
    {
        #region Fields
        public readonly bool T1IsIRQ = false;
        public readonly bool T2IsIRQ = true;
        public ushort T1CL = 0xD024;
        public ushort T1CH = 0xD025;
        public ushort T2CL = 0xD028;
        public ushort T2CH = 0xD029;
        public ushort ACR = 0xD02B;
        public ushort IFR = 0xD02D;
        public ushort IER = 0xD02E;

        public byte ACR_T1TC = (byte)(1 << 7);
        public byte ACR_T2TC = (byte)(1 << 6);

        public byte IFR_T2 = (byte)(1 << 5);
        public byte IFR_T1 = (byte)(1 << 6);
        public byte IFR_INT = (byte)(1 << 7);

        public byte IER_T2 = (byte)(1 << 5);
        public byte IER_T1 = (byte)(1 << 6);
        public byte IER_EN = (byte)(1 << 7);
        #endregion

        #region Properties
        private ushort Offset { get; set; }

        /// <summary>
        /// T1 timer control
        /// </summary>
        public bool T1TimerControl
        {
            get { return T1Object.AutoReset; }
            set { T1Object.AutoReset = value; }
        }

        /// <summary>
        /// T2 timer control.
        /// </summary>
        public bool T2TimerControl
        {
            get { return T2Object.AutoReset; }
            set { T2Object.AutoReset = value; }
        }

        /// <summary>
        /// Enable or check whether timer 1 is enabled or not.
        /// </summary>
        public bool T1IsEnabled
        {
            get { return T1Object.Enabled; }
            set { T1Object.Enabled = value; }
        }

        /// <summary>
        /// Enable or check whether timer 2 is enabled or not.
        /// </summary>
        public bool T2IsEnabled
        {
            get { return T2Object.Enabled; }
            set { T2Object.Enabled = value; }
        }

        /// <summary>
        /// Set or check the timer 1 interval.
        /// </summary>
        public double T1Interval { get { return (int)(MemoryMap.ReadWithoutCycle(T1CL) | (MemoryMap.ReadWithoutCycle(T1CH) << 8)); } }

        /// <summary>
        /// Set or check the timer 2 interval.
        /// </summary>
        public double T2Interval
        {
            get { return (int)(MemoryMap.ReadWithoutCycle(T2CL) | (MemoryMap.ReadWithoutCycle(T2CH) << 8)); }
        }

        /// <summary>
        /// Set or get the timer 1 object.
        /// </summary>
        public Timer T1Object { get; set; }

        /// <summary>
        /// Set or get the timer 2 object.
        /// </summary>
        public Timer T2Object { get; set; }
        
        private W65C02 Processor { get; set; }
        #endregion

        #region Public Methods
        public W65C22(W65C02 processor)
        {
            Processor = processor;
        }

        public W65C22(W65C02 processor, byte offset)
        {
            if (offset > MemoryMap.DeviceArea.Length)
                throw new ArgumentException(String.Format("The offset: {0} is greater than the device area: {1}", offset, MemoryMap.DeviceArea.Length));
            Offset = (ushort)(MemoryMap.DeviceArea.Offset & offset);
            T1Init(1000);
            T2Init(1000);

            Processor = processor;
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
        public void T1Init(double value)
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
        public void T2Init(double value)
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
        private void OnT1Timeout(object sender, ElapsedEventArgs e)
        {
            if (Processor.isRunning)
            {
                if (T1IsEnabled)
                {
                    MemoryMap.WriteWithoutCycle(IFR, (byte)(IFR_T1 & IFR_INT));
                    if (T1IsIRQ)
                    {
                        Processor.InterruptRequest();
                    }
                    else
                    {
                        Processor.TriggerNmi = true;
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
        private void OnT2Timeout(object sender, ElapsedEventArgs e)
        {
            if (Processor.isRunning)
            {
                if (T2IsEnabled)
                {
                    MemoryMap.WriteWithoutCycle(IFR, (byte)(IFR_T2 & IFR_INT));
                    if (T2IsIRQ)
                    {
                        Processor.InterruptRequest();
                    }
                    else
                    {
                        Processor.TriggerNmi = true;
                    }
                }
            }
        }
        #endregion
    }
}