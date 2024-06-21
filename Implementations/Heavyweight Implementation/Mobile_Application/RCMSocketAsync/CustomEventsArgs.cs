using System;


namespace MobileClient {
    public class ReceivedTextEventArgs : EventArgs {
        public string textReceived { get; set; }

        public ReceivedTextEventArgs(string textReceivedParam) {
            textReceived = textReceivedParam;
        }
    }

    public class WriteToFileEventArgs : EventArgs {
        public string filepath { get; set; }

        public WriteToFileEventArgs(string filepathParam) {
            filepath = filepathParam;
            //textReceived = textReceivedParam;
        }
    }
}