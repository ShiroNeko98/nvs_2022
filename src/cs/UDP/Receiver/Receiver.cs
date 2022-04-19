using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Receiver;

public class Receiver
{
    private readonly int _port;

    public Receiver(int port)
    {
        _port = port;
    }

    public Transmission ReceiveTransmission()
    {
        IPEndPoint remoteIpEndPoint = new IPEndPoint(IPAddress.Any, _port);
        UdpClient udpClient = new UdpClient(_port);

        Transmission transmission = new Transmission();
        while (!transmission.IsEnd)
        {
            Byte[] receiveBytes = udpClient.Receive(ref remoteIpEndPoint);
            string receiveData = Encoding.ASCII.GetString(receiveBytes);
            
            transmission.AddPacket(receiveData);
        }

        return transmission;
    }
}