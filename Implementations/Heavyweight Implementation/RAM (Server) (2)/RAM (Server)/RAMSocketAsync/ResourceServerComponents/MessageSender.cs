using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace RAMSocketAsync.ResourceServerComponents {
    public class MessageSender {
        public async Task SendMessage(TcpClient client, string message) {
            if (String.IsNullOrEmpty(message)) {
                return;
            }
            try {
                //changes message into byte format
                byte[] buffMessage = Encoding.ASCII.GetBytes(message);
                client.GetStream().WriteAsync(buffMessage, 0, buffMessage.Length);
            }
            catch (Exception excp) {
                System.Diagnostics.Debug.WriteLine(excp.ToString());
            }
        }
    }
}
