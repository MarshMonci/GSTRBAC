using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using SerializablePackages;
using System.Windows.Forms;


namespace MobileClient
{
    public class ReceivedTextEventArgs : EventArgs
    {
        public string textReceived { get; set; }

        public ReceivedTextEventArgs (string textReceivedParam)
        {
            textReceived = textReceivedParam;
        }
    }

    public class WriteToFileEventArgs : EventArgs
    {
        public string filepath { get; set; }

        public WriteToFileEventArgs (string filepathParam)
        {
            filepath = filepathParam;
            //textReceived = textReceivedParam;
        }
    }
}