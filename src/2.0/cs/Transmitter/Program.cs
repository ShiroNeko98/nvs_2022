using System.Net.Sockets;
using Receiver;

namespace Transmitter
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("-=[UDP TRANSMITTER]=-");
            ProgramArguments programArguments = new ProgramArguments(args);

            UdpService service = new UdpService();
            service.TransmitMessage(programArguments.FilePath,programArguments.Ip,programArguments.Port,programArguments.BufferSize);
        }
    }
}