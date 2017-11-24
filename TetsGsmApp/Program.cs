using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GSMapp;
using GSMapp.Connectors;
using TetsGsmApp.Database;

namespace TetsGsmApp
{
    class Program
    {
        static void Main(string[] args)
        {
            PortConnect port = new PortConnect();
            GeneralCommands gc = new GeneralCommands(port);

            MobileContext db = new MobileContext();
            Phone p = new Phone {Company = "Xiaomi", Name = "BestPhone"};

            db.Phones.Add(p);
            db.SaveChanges();

            port.AddReceiver(gc.GeneralHandler);

            port.Connect();
            Console.WriteLine(port.IsConnected);

            if (port.IsConnected)
            {
                port.Operator();
            }

            Console.ReadLine();
        }
    }
}
