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
        private static readonly int _defaultBaudRate = 9600;
        private List<string> portsList = new List<string>();
        #endregion

        #region Public Methods
        /// <summary>
        /// Default Constructor, Instantiates a new instance of COM Port I/O.
        /// </summary>
        /// <param name="port"> COM Port to use for I/O</param>
        public void Init(string port)
        {
            _serialPort = new SerialPort(port, _defaultBaudRate, Parity.None, 8, StopBits.One);

            ComInit(_serialPort);
        }

        /// <summary>
        /// Default Constructor, Instantiates a new instance of COM Port I/O.
        /// </summary>
        /// <param name="port"> COM Port to use for I/O</param>
        /// <param name="baudRate"> Baud Rate (Connection Speed) to use for I/O</param>
        public void Init(string port, int baudRate)
        {
            _serialPort = new SerialPort(port, baudRate, Parity.None, 8, StopBits.One);

            ComInit(_serialPort);
        }

        public void UpdatePortList()
        {
            portsList.Clear();
            foreach (string s in SerialPort.GetPortNames())
            {
                portsList.Add(s);
            }
        }

        public List<string> GetPortList()
        {
            return portsList;
        }
        #endregion

        #region Private Methods
        private void ComInit(SerialPort serialPort)
        {
            serialPort.ReadTimeout = 1000;
            serialPort.WriteTimeout = 1000;
        }
        #endregion
    }
}
