using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Management;
using System.Net.Configuration;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GSMapp.Models;

namespace GSMapp.Connectors
{
    public class PortConnect
    {
        private readonly SerialPort _port = null;
        private ComConnect ComConnect { get; set; }

        public bool IsConnected { get; set; } = false;
        public Com ComPort { get; private set; }

        private List<SerialDataReceivedEventHandler> Receivers { get; set; }

        public PortConnect()
        {
            _port = new SerialPort();
            Receivers = new List<SerialDataReceivedEventHandler>();
            ComConnect = new ComConnect();
            ComPort = new Com();
        }

        public bool Connect()
        {
            if (_port == null || !IsConnected || !_port.IsOpen)
            {
                IsConnected = false;

                Com com = ComConnect.Search();
                if (com != null)
                {
                    try
                    {
                        _port.PortName = com.Name;
                        _port.BaudRate = 9600; // еще варианты 4800, 9600, 28800 или 56000
                        _port.DataBits = 8; // еще варианты 8, 9
                        _port.StopBits = StopBits.One; // еще варианты StopBits.Two StopBits.None или StopBits.OnePointFive         
                        _port.Parity = Parity.Odd; // еще варианты Parity.Even Parity.Mark Parity.None или Parity.Space
                        _port.ReadTimeout = 500; // еще варианты 1000, 2500 или 5000 (больше уже не стоит)
                        _port.WriteTimeout = 500; // еще варианты 1000, 2500 или 5000 (больше уже не стоит)
                        _port.NewLine = Environment.NewLine;
                        _port.Handshake = Handshake.RequestToSend;
                        _port.DtrEnable = true;
                        _port.RtsEnable = true;
                        _port.Encoding = Encoding.GetEncoding("windows-1251");

                        _port.Open();

                        this.ComPort.Name = com.Name;
                        this.ComPort.Description = com.Description;
                        
                        IsConnected = true;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message+" tut");
                        IsConnected = false;
                    }
                }
                else
                {
                    IsConnected = false;
                }

            }
            return IsConnected;
        }

        public void Disconnect()
        {
            if (_port != null || IsConnected || _port.IsOpen)
            {
                _port.Close();
                _port.Dispose();
                IsConnected = false;
            }
        }

        public void AddReceiver(SerialDataReceivedEventHandler receiver)
        {
            if (Receivers.Any(r => r == receiver))
            {
                throw new Exception("Событие уже добавленно");
            }
            _port.DataReceived += receiver;
            Receivers.Add(receiver);
        }

        public void RemoveReceiver(SerialDataReceivedEventHandler receiver)
        {
            if (Receivers.Any(r => r == receiver))
            {
                _port.DataReceived -= receiver;
                Receivers.Remove(receiver);
            }
            else
            {
                throw new Exception("Нет подписки на событие");
            }
            
        }

        public void UpdateReceiver(SerialDataReceivedEventHandler receiver)
        {
            if (Receivers.Any(r => r == receiver))
            {
                _port.DataReceived -= receiver;
                Receivers.Remove(receiver);
            }
            else
            {
                Receivers.Add(receiver);
            }
        }

        public void DeleteAllReceiver()
        {
            
        }

        public void Write(string command)
        {
            if (IsConnected)
            {
                _port.WriteLine(command);
                Thread.Sleep(500);
            }
        }

        //В класс команд
        public void ReadFirst()
        {
            Console.WriteLine("Reading first...");

            _port.WriteLine("AT+CMGF=1"); //Set mode to Text(1) or PDU(0)
            Thread.Sleep(500); //Give a second or write
            _port.WriteLine("AT+CPMS=\"SM\""); //Set storage to SIM(SM)
            Thread.Sleep(500);
            _port.WriteLine("AT+CMGL=\"ALL\""); //What category to read ALL, REC READ, or REC UNREAD
            Thread.Sleep(500);

            string responce = _port.ReadExisting();

            if (responce.EndsWith("\r\nOK\r\n"))
            {
                Console.WriteLine(responce);
            }
            else
            { 
                Console.WriteLine("!!Error text: "+responce); 
            }
        }

        public void Read()
        {
            Console.WriteLine("Reading...");

            _port.WriteLine("AT+CMGL=\"ALL\""); //What category to read ALL, REC READ, or REC UNREAD
            Thread.Sleep(500);

            string responce = _port.ReadExisting();

            if (responce.EndsWith("\r\nOK\r\n"))
            {
                Console.WriteLine(responce);
            }
            else
            {
                Console.WriteLine("!!Error text: " + responce);
            }
        }

        public void Send(string toAdress, string message)
        {
            Console.WriteLine("Sending...");

            _port.WriteLine("AT+CMGF=1");
            Thread.Sleep(500);
            _port.WriteLine($"AT+CMGS=\"{toAdress}\"");
            Thread.Sleep(500);
            _port.WriteLine(message + char.ConvertFromUtf32(26));
            Thread.Sleep(500);

            string responce = _port.ReadExisting();

            if (responce.EndsWith("\r\nOK\r\n") && responce.Contains("+CMGS"))
            {
                Console.WriteLine(responce);
            }
            else
            {
                Console.WriteLine("!!Error text: " + responce);
            }
        }

        public void Number()
        {
            Console.WriteLine("Number->");

            _port.WriteLine("AT^USSDMODE=0");
            Thread.Sleep(500);
            _port.WriteLine("AT+CUSD=1,\"*201#\",15");
            Thread.Sleep(500);

            string responce = _port.ReadExisting();

            if (responce.EndsWith("\r\nOK\r\n") && responce.Contains("+CMGS"))
            {
                Console.WriteLine(responce);
            }
            else
            {
                Console.WriteLine("!!Error text: " + responce);
            }
        }

        public void Unlock()
        {
            Console.WriteLine("Unlock->");

            _port.WriteLine("AT^U2DIAG=0");
            Thread.Sleep(500);
            _port.WriteLine("AT^CARDLOCK?");
            Thread.Sleep(500);

            string responce = _port.ReadExisting();

            if (responce.EndsWith("\r\nOK\r\n"))
            {
                Console.WriteLine(responce);
            }
            else
            {
                Console.WriteLine("!!Error text: " + responce);
            }
        }

        public void Operator()
        {
            Console.WriteLine("Operator->");

            _port.WriteLine("AT+COPS?");
            Thread.Sleep(500);

            
        }

        public void Imsi()
        {
            Console.WriteLine("IMSI->");

            _port.WriteLine("AT+CIMI");
            Thread.Sleep(500);
        }
    }
}
