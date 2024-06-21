using System;
using System.Collections.Generic;
using System.IO;
using SerializablePackages;

namespace RAMSocketAsync {
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
                if (!File.Exists(@"C:\Programming Dump\GSTRBAC C# Projects\RAM (Server) (2)\RAM (Server)Service1.txt"))
                {
                    File.WriteAllText(@"C:\Programming Dump\GSTRBAC C# Projects\RAM (Server) (2)\RAM (Server)Service1.txt", "These are [PATIENT-001]'s medical records.\n\n" +
                        "Additional Notes:\n");
                }
            }
            catch (Exception excp)
            {
                Console.WriteLine(excp.ToString());
            }
            try
            {
                if (!File.Exists(@"C:\Programming Dump\GSTRBAC C# Projects\RAM (Server) (2)\RAM (Server)Service2.txt"))
                {
                    File.WriteAllText(@"C:\Programming Dump\GSTRBAC C# Projects\RAM (Server) (2)\RAM (Server)Service2.txt", "These are [PATIENT-002]'s medical records.\n\n" +
                        "Additional Notes:\n");
                }
            }
            catch (Exception excp)
            {
                Console.WriteLine(excp.ToString());
            }
            try
            {
                if (!File.Exists(@"C:\Programming Dump\GSTRBAC C# Projects\RAM (Server) (2)\RAM (Server)Service3.txt"))
                {
                    File.WriteAllText(@"C:\Programming Dump\GSTRBAC C# Projects\RAM (Server) (2)\RAM (Server)Service3.txt", "These are [PATIENT-003]'s medical records.\n\n" +
                        "Additional Notes:\n");
                }
            }
            catch (Exception excp)
            {
                Console.WriteLine(excp.ToString());
            }
        }

        //UPDATE
        public string getFile(int request)
        {

            //int fileID = getFileID(request);

            //UPDATE
            switch (request)
            {
                case 1:
                    return @"C:\Programming Dump\GSTRBAC C# Projects\RAM (Server) (2)\RAM (Server)Service1.txt";
                case 2:
                    return @"C:\Programming Dump\GSTRBAC C# Projects\RAM (Server) (2)\RAM (Server)Service2.txt";
                case 3:
                    return @"C:\Programming Dump\GSTRBAC C# Projects\RAM (Server) (2)\RAM (Server)Service3.txt";
                case 4:
                    return @"C:\Programming Dump\GSTRBAC C# Projects\RAM (Server) (2)\CCCSC2021.pdf";
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
