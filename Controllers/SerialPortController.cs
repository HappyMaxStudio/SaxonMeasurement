using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SaxonSO2.Controllers
{
    internal class SerialPortController
    {
        private string PortName {  get; set; }
        public StringBuilder ReceivedData { get; private set; }
        private SerialPort SerialPort { get; set; }
        public SerialPortController(string portName)
        {
            PortName = portName;
            SerialPort = new SerialPort(portName);
            SerialPort.BaudRate = 9600;
            SerialPort.Parity = Parity.None;
            SerialPort.StopBits = StopBits.One;
            SerialPort.DataBits = 8;
            SerialPort.Handshake = Handshake.None;
            ReceivedData = new StringBuilder();
            SerialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
        }
        public void OpenPort()
        {
            try
            {
                SerialPort.Open();
            }
            catch (System.IO.IOException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public void ClosePort()
        {
            if (SerialPort.IsOpen)
            {
                SerialPort.Close();
            }
        }
        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            ReceivedData.Append(SerialPort.ReadExisting());
        }
    }
}
