using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Receiver
{
    public  class UdpService
    {

        public long PacketSize { get; set; }
        public long PacketCount { get; set; }
        public double FileSize { get; set; }
        public TimeSpan TotalTime { get; set; }
        public int ReceivedPackets { get; set; }
        public double SpeedInMbps { get; set; }
        public double PacketLoss { get; set; }

        public  void ReceiveTransmission(string path, int port)
        {
            try
            {
                IPEndPoint remoteIpEndPoint = new IPEndPoint(IPAddress.Any, port);

                UdpClient udpClient = new UdpClient(remoteIpEndPoint);

                Console.WriteLine("UDP Receiver ready.");
                byte[] init = udpClient.Receive(ref remoteIpEndPoint);
                string initPacket = Encoding.ASCII.GetString(init);
                string[] meta = initPacket.Split("\u0000");
                string fileName = meta[1];
                string seq = meta[0];
                int packets = int.Parse(meta[3]);
                DateTime dateTime = DateTime.Now;

                //udpClient.Client.ReceiveTimeout = 500;

                IPEndPoint sendRemoteIpEndPoint = remoteIpEndPoint;
                sendRemoteIpEndPoint.Port = 12000;
                UdpClient sendClient = new UdpClient();
                sendClient.Send(Encoding.ASCII.GetBytes(seq), sendRemoteIpEndPoint);
                int i = 1;
                Console.WriteLine("Send wait to: " + remoteIpEndPoint.Address);
                Console.WriteLine("Init received. Packets incoming: " + packets );
                using (FileStream stream = File.Create(path+fileName))
                {
                    try
                    {
                        while (i <= packets)
                        {
                            byte[] dataPacketWithSeq = udpClient.Receive(ref remoteIpEndPoint);
                            string dataPacketString = Encoding.ASCII.GetString(dataPacketWithSeq);
                            string[] dataPacketStringArray = dataPacketString.Split("\u0000");
                            byte[] data = Convert.FromBase64String(dataPacketStringArray[1]);
                            stream.Write(data);
                            sendClient.Send(Encoding.ASCII.GetBytes(dataPacketStringArray[0]), sendRemoteIpEndPoint);
                            i++;
                        }
                    }
                    catch (SocketException e)
                    {
                        Console.WriteLine("\r\n\r\nTransmission timed out!\n\r");
                        double percent = i / (double)packets * 100;
                        Console.WriteLine("\r\n\r\nGot " +percent+ "% of packets\n\r");
                    }

                    stream.Dispose();
                }

                TotalTime = DateTime.Now - dateTime;
                PacketCount = long.Parse(meta[3]);
                FileSize = (double)long.Parse(meta[2])/1000000;
                PacketSize =  long.Parse(meta[2]) / PacketCount;
                ReceivedPackets = i -1;
                SpeedInMbps = FileSize * 1000/(TotalTime.TotalMilliseconds );
                double packetLoss = i - 1 / (double) packets * 100;
                if (packetLoss > 100)
                {
                    PacketLoss = 0;
                }
                else
                {
                    PacketLoss = 100 - packetLoss;
                }

                udpClient.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
