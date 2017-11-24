using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GSMapp.Connectors;
using GSMapp.Hellpers;
using GSMapp.Models;

namespace GSMapp
{
    public class GeneralCommands
    {
        public string OperatorName { get; set; } = null;

        public SerialDataReceivedEventHandler Receiver;

        public PortConnect port;

        public GeneralCommands(PortConnect port)
        {
            this.port = port;
        }

        public string Operator(string sender)
        {
            string a = sender;
            int startIndex = a.IndexOf("\"", StringComparison.Ordinal)+4;
            int lastIndex = a.LastIndexOf("\"", StringComparison.Ordinal) - startIndex;
            if (sender.Contains("+COPS:"))
            {
                a = a.Substring(startIndex, lastIndex);
                OperatorCheck(a);
                return a;
            }
            return null;
        }

        private void OperatorCheck(string operatorNumber)
        {
            int number = Int32.Parse(operatorNumber);

            foreach (OperatorList o in Enum.GetValues(typeof(OperatorList)))
            {
                if (number == (int)o)
                {
                    Console.WriteLine("Operator is: "+o);
                }
            }

            
        }

        private string MessageOfNumber(string message)
        {
            int startIndex = message.IndexOf("\"", StringComparison.Ordinal)+1;
            int lastIndex = message.LastIndexOf("\"", StringComparison.Ordinal) - startIndex;
            if (message.Contains("+CUSD:"))
            {
                message = message.Substring(startIndex, lastIndex);
                string result = message.Ucs2StrToUnicodeStr();
                return result;
            }
            return null;
        }

        private string OnlyNumber(string message)
        {
            int startIndex = message.IndexOf("+", StringComparison.Ordinal);
            int lastIndex = message.Length - startIndex;
            if (message.Contains("Ваш федеральный номер"))
            {
                message = message.Substring(startIndex, lastIndex);
                return message;
            }
            return null;
        }

        private int z = 5;

        public void DeleteReceiver()
        {
            port.RemoveReceiver(Receiver);
        }

        public void ReceiverTest()
        {
            
                Receiver = (sender, args) =>
                {
                    Console.WriteLine("GeneralCommands");
                   
                    
                    
                    SerialPort sp = (SerialPort)sender;
                    string indata = sp.ReadExisting();
                    string message = MessageOfNumber(indata);
                    if (message != null)
                    {
                        Console.WriteLine("This is my message");
                        Console.WriteLine(message);
                        string number = OnlyNumber(message);
                        if (number != null)
                        {
                            Console.WriteLine("Only number: ");
                            Console.WriteLine(number);
                        }
                    }
                    

                    Console.WriteLine("Data Received->");
                    Console.Write(indata);
                    Console.WriteLine("End of data received<-");
                };
            
           
        }

        
        //Handler
        public void GeneralHandler(object sender, SerialDataReceivedEventArgs e)
        {
            Console.WriteLine("GeneralCommands");
            SerialPort sp = (SerialPort)sender;
            string indata = sp.ReadExisting();

            string op = Operator(indata);
            if (op != null)
            {
                Console.WriteLine("OPERATOR: "+op);
            }

            Console.WriteLine("Data Received->");
            Console.Write(indata);
            Console.WriteLine("End of data received<-");
            
        }
    }

   
}
