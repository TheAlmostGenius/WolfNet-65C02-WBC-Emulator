using System;
using System.ComponentModel;
using System.Runtime.Remoting.Messaging;
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
        public int IORB = 0x00;
        public int IORA = 0x01;
        public int DDRB = 0x02;
        public int DDRA = 0x03;
        public int T1CL = 0x04;
        public int T1CH = 0x05;
        public int T2CL = 0x08;
        public int T2CH = 0x09;
        public int SR = 0x0A;
        public int ACR = 0x0B;
        public int PCR = 0x0C;
        public int IFR = 0x0D;
        public int IER = 0x0E;

        public byte ACR_LATCH = 0x03;
        public byte ACR_T2TC = 0x20;
        public byte ACR_T1TC = 0x40;
        public byte ACR_PB7 = 0x80;

        public byte IFR_T2 = 0x20;
        public byte IFR_T1 = 0x40;
        public byte IFR_INT = 0x80;

        public byte IER_T2 = 0x20;
        public byte IER_T1 = 0x40;
        public byte IER_EN = 0x80;
        #endregion

        #region Properties
        /// <summary>
        /// The memory area.
        /// </summary>
        public byte[] Memory { get; set; }

        /// <summary>
        /// The memory offset of the device.
        /// </summary>
        public int Offset { get; set; }

        /// <summary>
        /// The length of the device memory.
        /// </summary>
        public int Length { get; set; }

        /// <summary>
        /// The end of memory
        /// </summary>
        public int End { get { return Offset + Length; } }

        public bool LatchingEnabled { get; set; }

        public byte LastCB1 { get; set; }

        public byte LastCA1 { get; set; }

        /// <summary>
        /// T1 timer control
        /// </summary>
        public bool T1TimerControl { get; set; }

        /// <summary>
        /// T2 timer control.
        /// </summary>
        public bool T2TimerControl { get; set; }

        /// <summary>
        /// Enable or check whether timer 1 is enabled or not.
        /// </summary>
        /// <todo>
        /// Add in get and set for Memory[]
        /// </todo>
        public bool T1IsEnabled { get; set; }

        /// <summary>
        /// Enable or check whether timer 2 is enabled or not.
        /// </summary>
        /// <todo>
        /// Add in get and set for Memory[]
        /// </todo>
        public bool T2IsEnabled { get; set; }

        /// <summary>
        /// Set or check the timer 1 interval.
        /// </summary>
        public double T1Interval { get { return (int)(Read(T1CL + Offset) | (Read(T1CH + Offset) << 8)); } }

        /// <summary>
        /// Set or check the timer 2 interval.
        /// </summary>
        public double T2Interval
        {
            get { return (int)(Read(T2CL + Offset) | (Read(T2CH + Offset) << 8)); }
        }

        /// <summary>
        /// Local referemce to the processor object.
        /// </summary>
        private W65C02 Processor { get; set; }

        private BackgroundWorker BackgroundWorker { get; set; }

        private bool PreviousPHI2 { get; set; }

        private short Timer1 { get; set; }

        private short Timer2 { get; set; }
        #endregion

        #region Public Methods
        public W65C22(W65C02 processor, byte offset, int length)
        {
            if (offset > MemoryMap.DeviceArea.Length)
                throw new ArgumentException(String.Format("The offset: {0} is greater than the device area: {1}", offset, MemoryMap.DeviceArea.Length));
            
            Offset = MemoryMap.DeviceArea.Offset + offset;
            Memory = new byte[length + 1];
            Length = length;
            Processor = processor;

            BackgroundWorker = new BackgroundWorker
            {
                WorkerSupportsCancellation = true
            };
            BackgroundWorker.DoWork += BackgroundWorkerDoWork;
            BackgroundWorker.RunWorkerAsync();
        }

        /// <summary>
        /// Reset routine called whenever the emulated computer is reset.
        /// </summary>
        public void Reset()
        {
            T1TimerControl = false;
            T1IsEnabled = false;
            T2TimerControl = false;
            T2IsEnabled = false;
        }

        /// <summary>
        /// Initialization routine for the VIA.
        /// </summary>
        public void Init()
        {
            T1Init();
            T2Init();
        }

        /// <summary>
        /// Routine to read from local memory.
        /// </summary>
        /// 
        /// <param name="address">Address to read from.</param>
        /// 
        /// <returns>Byte value stored in the local memory.</returns>
        public byte Read(int address)
        {
            if (address - Offset == IORB)
            {
                if (LatchingEnabled)
                {
                    return LastCB1;
                }
                else
                {
                    return Memory[address - Offset];
                }
            }
            else if (address - Offset == IORA)
            {
                if (LatchingEnabled)
                {
                    return LastCA1;
                }
                else
                {
                    return Memory[address - Offset];
                }
            }
            else if (address - Offset == ACR)
            {
                byte data = 0x00;
                if (T1TimerControl)
                {
                    data = (byte)(data | ACR_T1TC);
                }
                else if (T2TimerControl)
                {
                    data = (byte)(data | ACR_T2TC);
                }
                else if (LatchingEnabled)
                {
                    data = (byte)(data | ACR_LATCH);
                }
                return data;
            }
            else
            {
                // DDRB
                // DDRA
                return Memory[address - Offset];
            }
        }

        /// <summary>
        /// Writes data to the specified address in local memory.
        /// </summary>
        /// 
        /// <param name="address">The address to write data to.</param>
        /// <param name="data">The data to be written.</param>
        public void Write(int address, byte data)
        {
            Memory[address - Offset] = data;
            if ((address == Offset + ACR) && ((Memory[address - Offset] & ACR_T1TC) == ACR_T1TC))
            {
                T1TimerControl = true;
            }
            else if ((address == Offset + ACR) && ((Memory[address - Offset] & ACR_T2TC) == ACR_T2TC))
            {
                T2TimerControl = true;
            }
            else if ((address == Offset + IER) && ((Memory[address - Offset] & IER_T1) == IER_T1) && ((Memory[address - Offset] & IER_EN) == IER_EN))
            {
                T1IsEnabled = true;
            }
            else if ((address == Offset + IER) && ((Memory[address - Offset] & IER_T2) == IER_T2) && ((Memory[address - Offset] & IER_EN) == IER_EN))
            {
                T2IsEnabled = true;
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// T1 counter initialization routine.
        /// </summary>
        private void T1Init()
        {
            T1TimerControl = true;
            T1IsEnabled = true;
        }

        /// <summary>
        /// T2 counter initialization routine.
        /// </summary>
        private void T2Init()
        {
            T2TimerControl = true;
            T2IsEnabled = true;
        }
        
        private void OnT1Timeout()
        {
            if (Processor.isRunning)
            {
                if (T1IsEnabled)
                {
                    if ((Memory[ACR] | ACR_PB7) == ACR_PB7)
                    {
                        if ((Memory[IORB] | 0x80) == 0x80)
                        {
                            Memory[IORB] &= 0x7F;
                        }
                        else
                        {
                            Memory[IORB] |= 0x80;
                        }
                    }
                    Write(IFR, (byte)(IFR_T1 & IFR_INT));
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

        private void OnT2Timeout()
        {
            if (Processor.isRunning)
            {
                if (T2IsEnabled)
                {
                    Write(IFR, (byte)(IFR_T2 & IFR_INT));
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

        private void BackgroundWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            var worker = sender as BackgroundWorker;

            while (true)
            {
                if (worker != null && worker.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }

                if (Processor.PHI2 != PreviousPHI2)
                {
                    --Timer1;
                    --Timer2;
                    PreviousPHI2 = Processor.PHI2;
                }

                if (Timer1 == 0)
                {
                    OnT1Timeout();
                    /// <TODO>
                    /// Add in handling for reset of Timer1
                    /// </TODO>
                }

                if (Timer2 == 0)
                {
                    OnT2Timeout();
                    /// <TODO>
                    /// Add in handling for reset of Timer2
                    /// </TODO>
                }
            }
        }
        #endregion
    }
}