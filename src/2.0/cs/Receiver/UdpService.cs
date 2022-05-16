using System.Net;
using System.Net.Sockets;
using System.Text;
using Receiver.Gui;

namespace Receiver
{
    public static class UdpService
    {
        public static void ReceiveTransmission(string path, int port)
        {
            IPEndPoint remoteIpEndPoint = new IPEndPoint(IPAddress.Any, port);
            UdpClient udpClient = new UdpClient(remoteIpEndPoint);
            RecieveGui gui = new RecieveGui();

            Console.WriteLine("UDP Receiver ready.");
            byte[] init = udpClient.Receive(ref remoteIpEndPoint);
            string initPacket = Encoding.ASCII.GetString(init);
            string[] meta = initPacket.Split("\u0000");
            string fileName = meta[1];
            byte[] buffer = new byte[long.Parse(meta[2])];
            long packets = long.Parse(meta[3]);
            gui.InitReceived(packets);
            udpClient.Client.ReceiveTimeout = 50;
            using (FileStream stream = File.Create(path+fileName))
            {
                int i = 0;
                try
                {
                    while (i <= packets)
                    {
                        byte[] data = udpClient.Receive(ref remoteIpEndPoint);
                        //Buffer.BlockCopy(data,0,buffer,0,data.Length);
                        stream.Write(data);
                        i++;
                    }
                }
                catch (SocketException e)
                {
                    Console.WriteLine("\r\n\r\nTransmission timed out!\n\r");
                    double percent = i / (double)packets * 100;
                    Console.WriteLine("\r\n\r\nGot " +percent+ "% of packets\n\r");
                }
                gui.NewPacket(i);

                stream.Dispose();
            }
            gui.EndReceived();
            udpClient.Close();
        }
    }
}