using System;
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
        public int address = 0xD010;
        public byte data = 0b00000000;
        public readonly int defaultBaudRate = 9600;
        public byte byteIn;
        #endregion

        #region Properties
        private W65C02 Processor { get; set; }
        public ushort Offset { get; set; }
        public bool IsEnabled { get; set; }
        public SerialPort Object { get; set; }
        public string ObjectName { get; set; }
        #endregion

        #region Public Methods
        public W65C51(W65C02 processor, ushort offset)
        {
            if (offset > MemoryMap.DeviceArea.Length)
                throw new ArgumentException(String.Format("The offset: {0} is greater than the device area: {1}", offset, MemoryMap.DeviceArea.Length));
            Offset = (ushort)(MemoryMap.DeviceArea.Offset & offset);
            Processor = processor;
        }

        public void Reset()
        {
            IsEnabled = false;
        }

        /// <summary>
        /// Default Constructor, Instantiates a new instance of COM Port I/O.
        /// </summary>
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
            catch (System.TimeoutException t)
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
                byteIn = Convert.ToByte(Object.ReadByte());
                MemoryMap.WriteWithoutCycle(0xD011, data);
                Processor.InterruptRequest();
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
        #endregion
    }
}