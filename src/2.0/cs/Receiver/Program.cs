using System.Diagnostics.CodeAnalysis;

namespace Receiver
{
    class Program
    {
        [SuppressMessage("ReSharper.DPA", "DPA0001: Memory allocation issues")]
        
        static void Main(string[] args)
        {
            Console.WriteLine("-=[UDP RECEIVER]=-");
            ProgramArguments arguments = new ProgramArguments(args);
            for (int i = 0; i < arguments.Count; i++)
            {
                UdpService service = new UdpService();
                service.ReceiveTransmission(arguments.DirPath,arguments.Port);
          
            
                LogWriter.LogWrite("Transmission:" +
                                   "\r\nFile size in Mb: " + service.FileSize + " Packet size: " + service.PacketSize + 
                                   "\r\nReceived: " + service.ReceivedPackets + " packages out of " + service.PacketCount +
                                   "\r\nPackets lost: " + service.PacketLoss + " %"+
                                   "\r\nReceiving took: " + service.TotalTime + 
                                   "\r\nData rate: "+ service.SpeedInMbps + " mb/s" +
                                   "\r\n"); 
                Console.WriteLine($"\r\nLog created for {i+1}!");
                Thread.Sleep(1000);
            }
            Environment.Exit(0);
        }
    }
}