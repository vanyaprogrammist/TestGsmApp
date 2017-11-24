using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using GSMapp.Models;
using System.Security;

namespace GSMapp.Connectors
{
    public class ComConnect
    {
        private bool IsDeviceFound { get; set; } = false;

        //Return list of GSM modems (connectection)
        public Com[] List()
        {
            List<Com> gsmCom = new List<Com>();
            ConnectionOptions options = new ConnectionOptions();
            options.Impersonation = ImpersonationLevel.Impersonate;
            options.EnablePrivileges = true;
            string connectString = $@"\\{Environment.MachineName}\root\cimv2";
            ManagementScope scope = new ManagementScope(connectString, options);
            scope.Connect();

            ObjectQuery query = new ObjectQuery("SELECT * FROM Win32_POTSModem");
            ManagementObjectSearcher search = new ManagementObjectSearcher(scope, query);
            ManagementObjectCollection collection = search.Get();

            foreach (ManagementObject obj in collection)
            {
                string portName = obj["AttachedTo"].ToString();
                string portDescription = obj["Description"].ToString();

                if (portName != "")
                {
                    Com com = new Com();
                    com.Name = portName;
                    com.Description = portDescription;
                    gsmCom.Add(com);
                }
            }

            return gsmCom.ToArray();
        }

        public Com Search()
        {
            IEnumerator enumerator = List().GetEnumerator();
            Com com = enumerator.MoveNext() ? (Com)enumerator.Current : null;

            if (com == null)
            {
                IsDeviceFound = false;
                Console.WriteLine("No GSM device found!");
            }
            else
            {
                IsDeviceFound = true;
                Console.WriteLine(com.ToString());
            }

            return com;
        }
    }
}
