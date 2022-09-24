using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Ports;

namespace Hardware
{
    /// <summary>
    /// An implementation of a W65C51 ACIA.
    /// </summary>
    [Serializable]
    public class W65C51
    {
        #region Fields
        public readonly int defaultBaudRate = 115200;
        public byte byteIn;
        #endregion

        #region Properties
        public byte[] Memory { get; set; }
        public bool IsEnabled { get; set; }
        public SerialPort Object { get; set; }
        public string ObjectName { get; set; }
        private W65C02 Processor { get; set; }
        private BackgroundWorker _backgroundWorker { get; set; }
        public int Offset { get; set; }
        public int Length { get; set; }

        private bool DataRead { get; set; }
        private bool EchoMode { get; set; }
        private bool InterruptDisabled { get; set; }
        private bool Interrupted { get; set; }
        private bool Overrun { get; set; }
        private bool ParityEnabled { get; set; }
        private bool ReceiverFull { get; set; }
        private byte RtsControl { get; set; }
        #endregion

        #region Public Methods
        public W65C51(W65C02 processor, byte offset)
        {
            if (offset > MemoryMap.DeviceArea.Length)
                throw new ArgumentException(String.Format("The offset: {0} is greater than the device area: {1}", offset, MemoryMap.DeviceArea.Length));
            
            Processor = processor;

            Offset = MemoryMap.DeviceArea.Offset | offset;
            Length = 0x04;
            Memory = new byte[Length + 1];

            _backgroundWorker = new BackgroundWorker
            {
                WorkerSupportsCancellation = true
            };
            _backgroundWorker.DoWork += BackgroundWorkerDoWork;
            _backgroundWorker.RunWorkerAsync();
        }

        public void Reset()
        {
            IsEnabled = false;
        }

        /// <summary>
        /// Default Constructor, Instantiates a new instance of COM Port I/O.
        /// </summary>
        /// 
        /// <param name="port"> COM Port to use for I/O</param>
        public void Init(string port)
        {
            Object = new SerialPort(port, defaultBaudRate, Parity.None, 8, StopBits.One);
            ObjectName = port;

            ComInit(Object);
        }

        /// <summary>
        /// Default Constructor, Instantiates a new instance of COM Port I/O.
        /// </summary>
        /// 
        /// <param name="port">COM Port to use for I/O</param>
        /// <param name="baudRate">Baud Rate to use for I/O</param>
        public void Init(string port, int baudRate)
        {
            Object = new SerialPort(port, baudRate, Parity.None, 8, StopBits.One);
            ObjectName = port;

            ComInit(Object);
        }

        /// <summary>
        /// Called when the window is closed.
        /// </summary>
        public void Fini()
        {
            ComFini(Object);
        }

        /// <summary>
        /// Returns the byte at a given address.
        /// </summary>
        /// 
        /// <param name="bank">The bank to read data from.</param>
        /// <param name="address"></param>
        /// 
        /// <returns>the byte being returned</returns>
        public byte Read(int address)
        {
            HardwarePreRead(address);
            return Memory[address - Offset];
        }

        /// <summary>
        /// Writes data to the given address.
        /// </summary>
        /// 
        /// <param name="bank">The bank to load data to.</param>
        /// <param name="address">The address to write data to</param>
        /// <param name="data">The data to write</param>
        public void Write(int address, byte data)
        {
            HardwarePreWrite(address, data);
            if (!((address == Offset) || (address == Offset + 1)))
            {
                Memory[address - Offset] = data;
            }
        }

        /// <summary>
        /// Called in order to write to the serial port.
        /// </summary>
        /// 
        /// <param name="data">Byte of data to send</param>
        public void WriteCOM(byte data)
        {
            byte[] writeByte = new byte[] { data };
            Object.Write(writeByte, 0, 1);
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Called whenever the ACIA is initialized.
        /// </summary>
        /// 
        /// <param name="serialPort">SerialPort object to initialize.</param>
        private void ComInit(SerialPort serialPort)
        {
            try
            {
                serialPort.Open();
            }
            catch (UnauthorizedAccessException w)
            {
                FileStream file = new FileStream(FileLocations.ErrorFile, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                StreamWriter stream = new StreamWriter(file);
                stream.WriteLine(w.Message);
                stream.WriteLine(w.Source);
                stream.Flush();
                file.Flush();
                stream.Close();
                file.Close();
                return;
            }
            serialPort.ReadTimeout = 50;
            serialPort.WriteTimeout = 50;
            serialPort.DataReceived += new SerialDataReceivedEventHandler(SerialDataReceived);
            try
            {
                serialPort.Write("---------------------------\r\n");
                serialPort.Write(" WolfNet 6502 WBC Emulator\r\n");
                serialPort.Write("---------------------------\r\n");
                serialPort.Write("\r\n");
            }
            catch (TimeoutException t)
            {
                _ = t;
                FileStream file = new FileStream(FileLocations.ErrorFile, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                StreamWriter stream = new StreamWriter(file);
                stream.WriteLine("Read/Write error: Port timed out!");
                stream.WriteLine("Please ensure all cables are connected properly!");
                stream.Flush();
                file.Flush();
                stream.Close();
                file.Close();
                return;
            }
        }

        /// <summary>
        /// Called when the window is closed.
        /// </summary>
        /// 
        /// <param name="serialPort">SerialPort Object to close</param>
        private void ComFini(SerialPort serialPort)
        {
            if (serialPort != null)
            {
                serialPort.Close();
            }

            _backgroundWorker.CancelAsync();
            _backgroundWorker.DoWork -= BackgroundWorkerDoWork;
        }

        /// <summary>
        /// Called whenever SerialDataReceivedEventHandler event occurs.
        /// </summary>
        /// 
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SerialDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                if (EchoMode)
                {
                    WriteCOM(Convert.ToByte(Object.ReadByte()));
                }
                else
                {
                    if (!ReceiverFull)
                    {
                        ReceiverFull = true;
                    }
                    else
                    {
                        Overrun = true;
                    }
                    Memory[0] = Convert.ToByte(Object.ReadByte());
                }

                if (!InterruptDisabled)
                {
                    Interrupted = true;
                    Processor.InterruptRequest();
                }
            }
            catch (Win32Exception w)
            {
                FileStream file = new FileStream(FileLocations.ErrorFile, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                StreamWriter stream = new StreamWriter(file);
                stream.WriteLine(w.Message);
                stream.WriteLine(w.ErrorCode.ToString());
                stream.WriteLine(w.Source);
                stream.Flush();
                stream.Close();
                file.Flush();
                file.Close();
            }
        }

        private void HardwarePreWrite(int address, byte data)
        {
            if (address == Offset)
            {
                WriteCOM(data);
            }
            else if (address == Offset + 1)
            {
                Reset();
            }
            else if (address == Offset + 2)
            {
                CommandRegister(data);
            }
            else if (address == Offset + 3)
            {
                ControlRegister(data);
            }
        }

        private void HardwarePreRead(int address)
        {
            if (address == Offset)
            {
                Interrupted = false;
                Overrun = false;
                ReceiverFull = false;
                DataRead = true;

            }
            else if (address == Offset + 1)
            {
                StatusRegisterUpdate();
            }
            else if (address == Offset + 2)
            {
                CommandRegisterUpdate();
            }
            else if (address == Offset + 3)
            {
                ControlRegisterUpdate();
            }
        }

        private void CommandRegister(byte data)
        {
            byte test = (byte)(data & 0x20);
            if (test == 0x20)
            {
                throw new ArgumentException("Parity must NEVER be enabled!");
            }

            test = (byte)(data & 0x10);
            if (test == 0x10)
            {
                EchoMode = true;
            }
            else
            {
                EchoMode = false;
            }

            test = (byte)(data & 0x0C);
            if (test == 0x00)
            {
                Object.Handshake = Handshake.None;
                Object.RtsEnable = true;
                Object.Handshake = Handshake.RequestToSend;
            }
            else if (test == 0x04)
            {
                Object.Handshake = Handshake.None;
                Object.RtsEnable = false;
            }
            else if ((test == 0x08) || (test == 0x0C))
            {
                throw new NotImplementedException("This cannot be emulated on windows!");
            }
            else
            {
                throw new ArgumentOutOfRangeException("RtsControl is invalid!");
            }

            test = (byte)(data & 0x02);
            if (test == 0x02)
            {
                InterruptDisabled = true;
            }
            else
            {
                InterruptDisabled = false;
            }

            test = (byte)(data & 0x01);
            if (test == 0x01)
            {
                Object.DtrEnable = true;
            }
            else
            {
                Object.DtrEnable= false;
            }
        }

        private void CommandRegisterUpdate()
        {
            byte data = Memory[Offset + 2];

            if (ParityEnabled)
            {
                data |= 0x20;
            }
            else
            {
                data &= 0xD0;
            }

            if (EchoMode)
            {
                data |= 0x10;
            }
            else
            {
                data &= 0xE0;
            }

            data &= RtsControl;

            if (InterruptDisabled)
            {
                data |= 0x02;
            }
            else
            {
                data &= 0x0D;
            }
            if (Object.DtrEnable)
            {
                data |= 0x01;
            }
            else
            {
                data &= 0x0E;
            }

            Memory[Offset + 2] = data;
        }

        private void ControlRegister(byte data)
        {
            byte test = (byte)(data & 0x80);
            if (test == 0x80)
            {
                test = (byte)(data & 0x60);
                if (test == 0x60)
                {
                    Object.StopBits = StopBits.OnePointFive;
                }
                else
                {
                    Object.StopBits = StopBits.Two;
                }
            }
            else
            {
                Object.StopBits = StopBits.One;
            }

            test = (byte)(data & 0x60);
            if (test == 0x20)
            {
                Object.DataBits = 7;
            }
            else if (test == 0x40)
            {
                Object.DataBits = 6;
            }
            else if (test == 0x60)
            {
                Object.DataBits = 5;
            }
            else
            {
                Object.DataBits = 8;
            }

            test = (byte)(data & 0x10);
            if (!(test == 0x10))
            {
                throw new ArgumentException("External clock rate not available on the WolfNet 65C02 WBC!");
            }

            test = (byte)(data & 0x0F);
            if (test == 0x00)
            {
                Object.BaudRate = 115200;
            }
            else if (test == 0x01)
            {
                Object.BaudRate = 50;
            }
            else if (test == 0x02)
            {
                Object.BaudRate = 75;
            }
            else if (test == 0x03)
            {
                Object.BaudRate = 110;
            }
            else if (test == 0x04)
            {
                Object.BaudRate = 135;
            }
            else if (test == 0x05)
            {
                Object.BaudRate = 150;
            }
            else if (test == 0x06)
            {
                Object.BaudRate = 300;
            }
            else if (test == 0x07)
            {
                Object.BaudRate = 600;
            }
            else if (test == 0x08)
            {
                Object.BaudRate = 1200;
            }
            else if (test == 0x09)
            {
                Object.BaudRate = 1800;
            }
            else if (test == 0x0A)
            {
                Object.BaudRate = 2400;
            }
            else if (test == 0x0B)
            {
                Object.BaudRate = 3600;
            }
            else if (test == 0x0C)
            {
                Object.BaudRate = 4800;
            }
            else if (test == 0x0D)
            {
                Object.BaudRate = 7200;
            }
            else if (test == 0x0E)
            {
                Object.BaudRate = 9600;
            }
            else
            {
                Object.BaudRate = 19200;
            }
        }

        private void ControlRegisterUpdate()
        {
            byte controlRegister = Memory[Offset + 3];

            if (Object.StopBits == StopBits.Two)
            {
                controlRegister |= 0x80;
            }
            else if ((Object.StopBits == StopBits.OnePointFive) && (Object.DataBits == 5) || (Object.StopBits == StopBits.One))
            {
                controlRegister &= 0x7F;
            }
            else
            {
                throw new ArgumentOutOfRangeException("StopBits or combination of StopBits and DataBits is invalid!");
            }

            if (Object.DataBits == 8)
            {
                controlRegister &= 0x9F;
            }
            else if (Object.DataBits == 7)
            {
                controlRegister |= 0x20;
            }
            else if (Object.DataBits == 6)
            {
                controlRegister |= 0x40;
            }
            else if (Object.DataBits == 5)
            {
                controlRegister |= 0x60;
            }
            else
            {
                throw new ArgumentOutOfRangeException("DataBits is out of range!");
            }

            if (Object.BaudRate == 115200)
            {
                controlRegister &= 0xF0;
            }
            else if (Object.BaudRate == 50)
            {
                controlRegister |= 0x01;
            }
            else if (Object.BaudRate == 75)
            {
                controlRegister |= 0x02;
            }
            else if (Object.BaudRate == 110)
            {
                controlRegister |= 0x03;
            }
            else if (Object.BaudRate == 135)
            {
                controlRegister |= 0x04;
            }
            else if (Object.BaudRate == 150)
            {
                controlRegister |= 0x05;
            }
            else if (Object.BaudRate == 300)
            {
                controlRegister |= 0x06;
            }
            else if (Object.BaudRate == 600)
            {
                controlRegister |= 0x07;
            }
            else if (Object.BaudRate == 1200)
            {
                controlRegister |= 0x08;
            }
            else if (Object.BaudRate == 1800)
            {
                controlRegister |= 0x09;
            }
            else if (Object.BaudRate == 2400)
            {
                controlRegister |= 0x0A;
            }
            else if (Object.BaudRate == 3600)
            {
                controlRegister |= 0x0B;
            }
            else if (Object.BaudRate == 4800)
            {
                controlRegister |= 0x0C;
            }
            else if (Object.BaudRate == 7200)
            {
                controlRegister |= 0x0D;
            }
            else if (Object.BaudRate == 9600)
            {
                controlRegister |= 0x0E;
            }
            else if (Object.BaudRate == 19200)
            {
                controlRegister |= 0x0F;
            }
            else
            {
                throw new ArgumentOutOfRangeException("BaudRate is outside the range of Baud Rates supported by the W65C51!");
            }

            Memory[Offset + 3] = controlRegister;
        }

        private void StatusRegisterUpdate()
        {
            byte statusRegister = Memory[Offset + 1];

            if (Interrupted)
            {
                statusRegister |= 0x80;
            }
            else
            {
                statusRegister &= 0x7F;
            }

            if (Object.DsrHolding == false)
            {
                statusRegister |= 0x40;
            }
            else
            {
                statusRegister &= 0xBF;
            }

            if (Object.CDHolding)
            {
                statusRegister |= 0x20;
            }
            else
            {
                statusRegister &= 0xDF;
            }

            statusRegister |= 0x10;

            if (ReceiverFull)
            {
                statusRegister |= 0x08;
            }
            else
            {
                statusRegister &= 0xF7;
            }

            if (Overrun)
            {
                statusRegister |= 0x04;
            }
            else
            {
                statusRegister &= 0xFB;
            }

            statusRegister &= 0xFC;

            Memory[Offset + 1] = statusRegister;
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

                if (ReceiverFull || Overrun)
                {
                    Memory[Offset + 1] = (byte)(Memory[Offset + 1] | 0x80);
                    Interrupted = true;
                    Processor.InterruptRequest();
                }

                if (DataRead)
                {
                    System.Threading.Thread.Sleep(5);
                    ReceiverFull = false;
                    Interrupted = false;
                    Overrun = false;
                }
            }
        }
        #endregion
    }
}