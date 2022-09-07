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
        public static bool IsEnabled { get; set; }
        public static SerialPort Object { get; set; }
        #endregion

        #region Public Methods
        /// <summary>
        /// Default Constructor, Instantiates a new instance of COM Port I/O.
        /// </summary>
        /// <param name="port"> COM Port to use for I/O</param>
        public static void Init(string port)
        {
            W65C51.Object = new SerialPort(port, W65C51.defaultBaudRate, Parity.None, 8, StopBits.One);

            ComInit(W65C51.Object);
        }

        /// <summary>
        /// Default Constructor, Instantiates a new instance of COM Port I/O.
        /// </summary>
        /// <param name="port">COM Port to use for I/O</param>
        /// <param name="baudRate">Baud Rate to use for I/O</param>
        public static void Init(string port, int baudRate)
        {
            W65C51.Object = new SerialPort(port, baudRate, Parity.None, 8, StopBits.One);

            ComInit(W65C51.Object);
        }

        /// <summary>
        /// Called when the window is closed.
        /// </summary>
        public static void Fini()
        {
            ComFini(W65C51.Object);
        }

        /// <summary>
        /// Called in order to write to the serial port.
        /// </summary>
        /// 
        /// <param name="data">Byte of data to send</param>
        public static void WriteCOM(byte data)
        {
            byte[] writeByte = new byte[] { data };
            W65C51.Object.Write(writeByte, 0, 1);
        }
        #endregion

        #region Private Methods
        private static void ComInit(SerialPort serialPort)
        {
            try
            {
                serialPort.Open();
            }
            catch (Win32Exception w)
            {
                FileStream file = new FileStream("./COMIO.log", FileMode.OpenOrCreate, FileAccess.ReadWrite);
                StreamWriter stream = new StreamWriter(file);
                stream.WriteLine(w.Message);
                stream.WriteLine(w.ErrorCode.ToString());
                stream.WriteLine(w.Source);
                stream.Flush();
                stream.Close();
                file.Flush();
                file.Close();
            }
            serialPort.ReadTimeout = 1000;
            serialPort.WriteTimeout = 1000;
            serialPort.DataReceived += new SerialDataReceivedEventHandler(SerialDataReceived);
            serialPort.Write("---------------------------\r\n");
            serialPort.Write(" WolfNet 6502 WBC Emulator\r\n");
            serialPort.Write("---------------------------\r\n");
            serialPort.Write("\r\n");
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
                W65C51.byteIn = Convert.ToByte(W65C51.Object.ReadByte());
                W65C02.WriteMemoryValueWithoutCycle(0xD011, W65C51.data);
                W65C02.InterruptRequest();
            }
            catch (Win32Exception w)
            {
                FileStream file = new FileStream("./COMIO.log", FileMode.OpenOrCreate, FileAccess.ReadWrite);
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