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
        public static int address = 0xD010;
        public static byte data = 0b00000000;
        public static readonly int defaultBaudRate = 9600;
        public static byte byteIn;
        #endregion

        #region Properties
        public static ushort Offset { get; set; }
        public static bool IsEnabled { get; set; }
        public static SerialPort Object { get; set; }
        public static string ObjectName { get; set; }
        #endregion

        #region Public Methods
        public W65C51(ushort offset)
        {
            if (offset > MemoryMap.DeviceArea.Length)
                throw new ArgumentException(String.Format("The offset: {0} is greater than the device area: {1}", offset, MemoryMap.DeviceArea.Length));
            Offset = (ushort)(MemoryMap.DeviceArea.Offset & offset);
        }

        public static void Reset()
        {
            IsEnabled = false;
        }

        /// <summary>
        /// Default Constructor, Instantiates a new instance of COM Port I/O.
        /// </summary>
        /// <param name="port"> COM Port to use for I/O</param>
        public static void Init(string port)
        {
            Object = new SerialPort(port, defaultBaudRate, Parity.None, 8, StopBits.One);

            ComInit(Object);
        }

        /// <summary>
        /// Default Constructor, Instantiates a new instance of COM Port I/O.
        /// </summary>
        /// <param name="port">COM Port to use for I/O</param>
        /// <param name="baudRate">Baud Rate to use for I/O</param>
        public static void Init(string port, int baudRate)
        {
            Object = new SerialPort(port, baudRate, Parity.None, 8, StopBits.One);

            ComInit(Object);
        }

        /// <summary>
        /// Called when the window is closed.
        /// </summary>
        public static void Fini()
        {
            ComFini(Object);
        }

        /// <summary>
        /// Called in order to write to the serial port.
        /// </summary>
        /// 
        /// <param name="data">Byte of data to send</param>
        public static void WriteCOM(byte data)
        {
            byte[] writeByte = new byte[] { data };
            Object.Write(writeByte, 0, 1);
        }
        #endregion

        #region Private Methods
        private static void ComInit(SerialPort serialPort)
        {
            try
            {
                serialPort.Open();
            }
            catch (UnauthorizedAccessException w)
            {
                FileStream file = new FileStream(Hardware.ErrorFile, FileMode.OpenOrCreate, FileAccess.ReadWrite);
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
                FileStream file = new FileStream(Hardware.ErrorFile, FileMode.OpenOrCreate, FileAccess.ReadWrite);
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
        private static void ComFini(SerialPort serialPort)
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
        private static void SerialDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                byteIn = Convert.ToByte(Object.ReadByte());
                W65C02.WriteMemoryValueWithoutCycle(0xD011, data);
                W65C02.InterruptRequest();
            }
            catch (Win32Exception w)
            {
                FileStream file = new FileStream(Hardware.ErrorFile, FileMode.OpenOrCreate, FileAccess.ReadWrite);
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