using System.Net;
using System.Net.Sockets;
using System.Text;
namespace Receiver
{
    public class UdpService
    {
        private readonly int _port;
        public string ip;
        
        public UdpService(int port)
        {
            _port = port;
        }
        
        public Transmission ReceiveTransmission()
        {
            IPEndPoint remoteIpEndPoint = new IPEndPoint(IPAddress.Any, _port);
            UdpClient udpClient = new UdpClient(remoteIpEndPoint);
            
            Transmission transmission = new Transmission();
            Console.WriteLine("UDP Receiver ready.");
            
            while (!transmission.IsEnd)
            {
                byte[] receiveBytes  = udpClient.Receive(ref remoteIpEndPoint);
                string receiveData = Encoding.ASCII.GetString(receiveBytes);
                transmission.AddPacket(receiveData);
            }

            ip = remoteIpEndPoint.Address.ToString();
            udpClient.Close();
            return transmission;
        }
    }
}