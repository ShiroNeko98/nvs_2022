using System.Net;
using System.Net.Sockets;
using System.Text;
using Receiver;
using Receiver.Gui;

namespace Transmitter
{
    public  class UdpService
    {
        private const int BufferSize = 4096;

        public  void TransmitMessage(string filePath, string ip, int port)
        {
            FileInfo fi = new FileInfo(filePath);
            long fileSize = fi.Length;
            long packets =  (long)Math.Ceiling((double)(fi.Length / BufferSize));
            Console.WriteLine(packets);    
            byte[] buffer = new byte[BufferSize];
            UdpClient client = new UdpClient();
            client.Client.SendTimeout = 500;
            client.Connect(ip,port);

            IPEndPoint remoteIpEndPoint = new IPEndPoint(IPAddress.Any, 12000);
            UdpClient waitClient = new UdpClient(remoteIpEndPoint);
             
            waitClient.Client.ReceiveTimeout = 500;
                
            string initPacket = "0000\u0000"  + Path.GetFileName(filePath) + "\u0000"  + fileSize + "\u0000" + packets;
                
            using (Stream stream = File.Open(filePath,FileMode.Open))
            {
                client.Send(Encoding.ASCII.GetBytes(initPacket));
                while ((stream.Read(buffer, 0, BufferSize)) > 0)
                {
                    int retry = 0;
                    SendAndWait(client,waitClient,remoteIpEndPoint,buffer,retry);
                }
                waitClient.Close();
                client.Close();
            }
        }

        private static void SendAndWait(UdpClient sendClient, UdpClient waitClient, IPEndPoint endPoint , byte[] buffer, int retry)
        {
            try
            {
                sendClient.Send(buffer);
                waitClient.Receive(ref endPoint);
            }
            catch (Exception)
            {
                if (retry < 3)
                {
                    retry++;
                    SendAndWait(sendClient,waitClient,endPoint,buffer,retry);
                }
            }
        }
        
    }
}
