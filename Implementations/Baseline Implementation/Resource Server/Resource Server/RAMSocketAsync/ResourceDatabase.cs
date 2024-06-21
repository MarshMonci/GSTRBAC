using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using SerializablePackages;
using System.Net.Sockets;

namespace RAMSocketAsync
{
    public class ResourceDatabase
    {
        List<Packages.Resource> Resources;
        public ResourceDatabase()
        {
            Resources = new List<Packages.Resource>();

            Resources.Add(new Packages.Resource() { Filename = "service1" });
            Resources.Add(new Packages.Resource() { Filename = "service2" });
            Resources.Add(new Packages.Resource() { Filename = "service3" });

            try
            {
                if (!File.Exists(@"E:\Programming Dump\GSTRBAC C# Projects\Resource Server\Resource ServerService1.txt"))
                {
                    File.WriteAllText(@"E:\Programming Dump\GSTRBAC C# Projects\Resource Server\Resource ServerService1.txt", "These are [PATIENT-001]'s medical records.\n\n" +
                        "Additional Notes:\n");
                }
            }
            catch (Exception excp)
            {
                Console.WriteLine(excp.ToString());
            }
            try
            {
                if (!File.Exists(@"E:\Programming Dump\GSTRBAC C# Projects\Resource Server\Resource ServerService2.txt"))
                {
                    File.WriteAllText(@"E:\Programming Dump\GSTRBAC C# Projects\Resource Server\Resource ServerService2.txt", "These are [PATIENT-002]'s medical records.\n\n" +
                        "Additional Notes:\n");
                }
            }
            catch (Exception excp)
            {
                Console.WriteLine(excp.ToString());
            }
            try
            {
                if (!File.Exists(@"E:\Programming Dump\GSTRBAC C# Projects\Resource Server\Resource ServerService3.txt"))
                {
                    File.WriteAllText(@"E:\Programming Dump\GSTRBAC C# Projects\Resource Server\Resource ServerService3.txt", "These are [PATIENT-003]'s medical records.\n\n" +
                        "Additional Notes:\n");
                }
            }
            catch (Exception excp)
            {
                Console.WriteLine(excp.ToString());
            }
        }

        public string getFile(int request)
        {

            //int fileID = getFileID(request);


            switch (request)
            {
                case 1:
                    return @"E:\Programming Dump\GSTRBAC C# Projects\Resource Server\Resource ServerService1.txt";
                case 2:
                    return @"E:\Programming Dump\GSTRBAC C# Projects\Resource Server\Resource ServerService2.txt";
                case 3:
                    return @"E:\Programming Dump\GSTRBAC C# Projects\Resource Server\Resource ServerService3.txt";
                default:
                    return "Bad Request!";
            }
        }

        private int getFileID(string request)
        {
            return 1;
        }
    }
}
