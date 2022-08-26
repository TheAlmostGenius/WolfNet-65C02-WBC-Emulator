using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;

namespace COMIO
{
    public class COMIO
    {
        #region Fields
        private static SerialPort _serialPort;
        private static string _defaultComPort = "COM1";
        private static int _defaultBaudRate = 9600;
        private static int _defaultDataBits = 8;
        private static Parity _defaultParity = Parity.None;
        private static StopBits _defaultStopBits = StopBits.One;
        #endregion

        #region Public Methods
        /// <summary>
        /// Default Constructor, Instantiates a new instance of COM Port I/O.
        /// </summary>
		/// <param name="port"> COM Port to use for I/O</param>
        public COMIO()
        {
            _serialPort = new SerialPort(_defaultComPort, _defaultBaudRate, _defaultParity , _defaultDataBits, _defaultStopBits);

            _ComInit(_serialPort);
        }

        /// <summary>
        /// Default Constructor, Instantiates a new instance of COM Port I/O.
        /// </summary>
        /// <param name="port"> COM Port to use for I/O</param>
        public COMIO(string port)
        {
            _serialPort = new SerialPort(port, _defaultBaudRate, Parity.None, 8, StopBits.One);

            _ComInit(_serialPort);
        }

        /// <summary>
        /// Default Constructor, Instantiates a new instance of COM Port I/O.
        /// </summary>
        /// <param name="port"> COM Port to use for I/O</param>
        /// <param name="baudRate"> Baud Rate (Connection Speed) to use for I/O</param>
        public COMIO(string port, int baudRate)
        {
            _serialPort = new SerialPort(port, baudRate, Parity.None, 8, StopBits.One);

            _ComInit(_serialPort);
        }

        /// <summary>
        /// Default Constructor, Instantiates a new instance of COM Port I/O.
        /// </summary>
        /// <param name="port"> COM Port to use for I/O</param>
        /// <param name="baudRate"> Baud Rate (Connection Speed) to use for I/O</param>
        /// <param name="dataBits"> Number of data bits to use for I/O</param>
        public COMIO(string port, int baudRate, int dataBits)
        {
            _serialPort = new SerialPort(port, baudRate, Parity.None, dataBits, StopBits.One);

            _ComInit(_serialPort);
        }

        public void _ComInit(SerialPort serialPort)
        {
            serialPort.ReadTimeout = 500;
            serialPort.WriteTimeout = 500;
        }
        #endregion
    }
}
