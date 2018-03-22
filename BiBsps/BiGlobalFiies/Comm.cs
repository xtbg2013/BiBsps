using System;
using System.IO.Ports;
using System.Windows.Forms;

namespace BiBsps.BiGlobalFiies
{
    public class Comm: BaseCom
    {
        private readonly SerialPort _com;
        private bool _isRecv; 
        public Comm(string portName, int baudrate = 19200)
        {
            _com = new SerialPort
            {
                BaudRate = baudrate,
                StopBits = StopBits.One,
                DataBits = 8,
                Parity = Parity.None,
                PortName = portName,
                ReadTimeout = 5000
            };
            _com.DataReceived += ReceivceData;
           
        }
        private void ReceivceData(object sender, SerialDataReceivedEventArgs e)
        {
            _isRecv = true;
        }

        private static void Delay(int milliseconds)
        {
            var start = Environment.TickCount;
            var ticks = 0;
            while (ticks < milliseconds)
            {
                ticks = Math.Abs(Environment.TickCount - start);
            } 
        }
        

        public override byte[] Query(byte[] cmd, int waitTranferStart = 5000, int waitTranferDone = 500)
        {
            if (_com.IsOpen == false)
            {
                _com.Open();
            }
            _com.Write(cmd, 0, cmd.Length);
          
            var cnt = 0;
            _isRecv = false;
            while (!_isRecv)
            {
                System.Threading.Thread.Sleep(20);
                Application.DoEvents();
                cnt += 20;
                if (cnt <= waitTranferStart) continue;

                _com.Close();
                throw new Exception("Response Timeout 5s.");
            }

            Delay(waitTranferDone);
            byte[] recvBuffer = new byte[0x200];
            var recvCount = _com.Read(recvBuffer, 0, recvBuffer.Length);
            var ret = new byte[recvCount];
            Array.Copy(recvBuffer, ret, ret.Length);

            _com.Close();
            return ret;
        }


    }
}
