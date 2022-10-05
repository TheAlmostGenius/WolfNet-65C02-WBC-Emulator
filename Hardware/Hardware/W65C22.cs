using System;
using System.Numerics;
using System.ComponentModel;
using GalaSoft.MvvmLight.Messaging;
using System.Net;

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
        public int T1LL = 0x06;
        public int T1LH = 0x07;
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
        /// Enable or check whether timer 2 is enabled or not.
        /// </summary>
        /// <todo>
        /// Add in get and set for Memory[]
        /// </todo>
        public bool T2IsEnabled { get; set; }

        /// <summary>
        /// Local referemce to the processor object.
        /// </summary>
        private W65C02 Processor { get; set; }

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

            Messenger.Default.Register<NotificationMessage>(this, NotificationMessageReceived);
        }

        /// <summary>
        /// Reset routine called whenever the emulated computer is reset.
        /// </summary>
        public void Reset()
        {
            T1TimerControl = false;
            T2TimerControl = false;
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
                if ((Memory[PCR] != 0x01) && (Memory[PCR] != 0x03))
                {
                    Memory[IFR] &= 0xF7;
                }

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
                if ((Memory[PCR] != 0x01) && (Memory[PCR] != 0x03))
                {
                    Memory[IFR] &= 0xFE;
                }

                if (LatchingEnabled)
                {
                    return LastCA1;
                }
                else
                {
                    return Memory[address - Offset];
                }
            }
            else if (address - Offset == T1CL)
            {
                if (!LatchingEnabled)
                {
                    Memory[IFR] &= 0xC7;
                }
                return (byte)(Timer1 & 0xFF);
            }
            else if (address - Offset == T1CH)
            {
                return (byte)((Timer1 >> 8) & 0xFF);
            }
            else if (address - Offset == T1LL)
            {
                return (byte)(Timer1 & 0xFF);
            }
            else if (address - Offset == T1LH)
            {
                return (byte)((Timer1 >> 8) & 0xFF);
            }
            else if (address - Offset == T2CL)
            {
                Memory[IFR] &= 0xD7;
                return (byte)(Timer2 & 0xFF);
            }
            else if (address - Offset == T2CH)
            {
                return (byte)((Timer2 >> 8) & 0xFF);
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
            if ((address == Offset + ACR) && ((Memory[address - Offset] & ACR_T1TC) == ACR_T1TC))
            {
                T1TimerControl = true;
            }
            else if ((address == Offset + ACR) && ((Memory[address - Offset] & ACR_T2TC) == ACR_T2TC))
            {
                T2TimerControl = true;
            }

            Memory[address - Offset] = data;
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// T1 counter initialization routine.
        /// </summary>
        private void T1Init()
        {
            T1TimerControl = true;
        }

        /// <summary>
        /// T2 counter initialization routine.
        /// </summary>
        private void T2Init()
        {
            T2TimerControl = true;
        }

        private void OnT1Timeout()
        {
            var doFirePB7 = false;
            if (Processor.isRunning)
            {
                if (((Memory[IER] & IER_T1) == IER_T1) && ((Memory[IER] & IER_EN) == IER_EN))
                {
                    if ((Memory[ACR] & ACR_PB7) == ACR_PB7)
                    {
                        if ((Memory[IORB] & 0x80) == 0x80)
                        {
                            Memory[IORB] &= 0x7F;
                        }
                        else
                        {
                            Memory[IORB] |= 0x80;
                        }
                    }
                    else if ((Memory[ACR] & 0xC0) == 0xC0)
                    {
                        Memory[IORB] &= 0x7F;
                        doFirePB7 = true;
                    }

                    if ((Read(IER) & IER_T1) == IER_T1)
                    {
                        Write(IFR, (byte)(IFR_T1 & IFR_INT));

                        if (T1IsIRQ)
                        {
                            Processor.InterruptRequest();
                        }
                        else
                        {
                            Processor.TriggerNmi = true;
                        }

                        if (doFirePB7)
                        {
                            Memory[IORB] |= 0x80;
                        }
                    }
                }
            }
        }

        private void OnT2Timeout()
        {
            if (Processor.isRunning)
            {
                if (((Memory[IER] & IER_T2) == IER_T2) && ((Memory[IER] & IER_EN) == IER_EN))
                {
                    if ((Read(IER) & IER_T1) == IER_T1)
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
        }

        private void NotificationMessageReceived(NotificationMessage notificationMessage)
        {
            if (notificationMessage.Notification == "PHI2")
            {
                if ((Memory[ACR] & 0x1C) == 0x04)
                {
                    Memory[SR] = (byte)(Memory[SR] << 1);
                }
                else if ((Memory[ACR] & 0x1C) == 0x14)
                {
                    Memory[SR] = (byte)((Memory[SR] >> 1) | (Memory[SR] << (8 - 1)));
                }

                if (((Memory[IER] & IER_T1) == IER_T1) && ((Memory[IER] & IER_EN) == IER_EN))
                {
                    --Timer1;
                    if (Timer1 == 0)
                    {
                        OnT1Timeout();

                        if (T1TimerControl)
                        {
                            Timer1 = (short)(Memory[T1CL] & (Memory[T1CH] << 8));
                        }
                        else
                        {
                            Memory[IER] &= 0xCF;
                        }
                    }
                }

                if (((Memory[IER] & IER_T2) == IER_T2) && ((Memory[IER] & IER_EN) == IER_EN))
                {
                    if ((((Memory[ACR] & 0x20) == 0x20) && !((Memory[IORB] & 0x40) == 0x40)) || ((Memory[ACR] & 0x20) != 0x20))
                    {
                        --Timer2;
                        if (Timer2 == 0)
                        {
                            if ((Memory[ACR] & 0x1C) == 0x08)
                            {
                                Memory[SR] = (byte)(Memory[SR] << 1);
                            }
                            else if ((Memory[ACR] & 0x1C) == 0x18)
                            {
                                Memory[SR] = (byte)((Memory[SR] >> 1) | (Memory[SR] << (8 - 1)));
                            }

                            OnT2Timeout();

                            if (T2TimerControl)
                            {
                                Timer2 = (short)(Memory[T2CL] & (Memory[T2CH] << 8));
                            }
                            else
                            {
                                Memory[IER] &= 0xDF;
                            }
                        }
                    }
                }
            }
        }
        #endregion
    }
}