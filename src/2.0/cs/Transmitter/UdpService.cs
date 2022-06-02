using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Transmitter
{
    public  class UdpService
    {
        public  void TransmitMessage(string filePath, string ip, int port, int bufferSize)
        {
            FileInfo fi = new FileInfo(filePath);
            long fileSize = fi.Length;
            long packets =  (long)Math.Ceiling((double)(fi.Length / bufferSize));
            Console.WriteLine("Packets sending: " + packets);    
            byte[] buffer = new byte[bufferSize];
            UdpClient client = new UdpClient();
            client.Client.SendTimeout = 500;
            client.Connect(ip,port);

            IPEndPoint remoteIpEndPoint = new IPEndPoint(IPAddress.Any, 12000);
            UdpClient waitClient = new UdpClient(remoteIpEndPoint);
             
            waitClient.Client.ReceiveTimeout = 500;
            int seq = 0;
            string seqi = "0";
            int again = 0;
            string initPacket = seq + "\u0000"  + Path.GetFileName(filePath) + "\u0000"  + fileSize + "\u0000" + packets;
                
            using (Stream stream = File.Open(filePath,FileMode.Open))
            {
                SendAndWait(client,waitClient,remoteIpEndPoint,Encoding.ASCII.GetBytes(initPacket), again, seqi);
                //client.Send(Encoding.ASCII.GetBytes(initPacket));
                while ((stream.Read(buffer, 0, bufferSize)) > 0)
                {
                    seq++;
                    string buffString = Convert.ToBase64String(buffer);
                    string dataPacket = seq + "\u0000" + buffString;
                    string seqs = seq.ToString();
                    int retry = 0;
                    SendAndWait(client,waitClient,remoteIpEndPoint,Encoding.ASCII.GetBytes(dataPacket),retry, seqs);
                }
                waitClient.Close();
                client.Close();
            }
        }

        private static void SendAndWait(UdpClient sendClient, UdpClient waitClient, IPEndPoint endPoint , byte[] buffer, int retry, string seqs)
        {
            try
            {
                sendClient.Send(buffer);
                byte[] a = waitClient.Receive(ref endPoint);
                string aString = Encoding.ASCII.GetString(a);
                //Console.WriteLine(aString + "=" +seqs);
                if (!aString.Equals(seqs,StringComparison.InvariantCultureIgnoreCase))
                {
                    Console.WriteLine("wrong Seqs!");
                    throw new Exception();
                }
            }
            catch (Exception)
            {
                if (retry < 3)
                {
                    retry++;
                    SendAndWait(sendClient,waitClient,endPoint,buffer,retry, seqs);
                }
            }
        }
    }
}
