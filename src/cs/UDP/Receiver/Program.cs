namespace Receiver
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("-=[UDP RECEIVER]=-");
            ProgramArguments arguments = new ProgramArguments(args);
            UdpService service = new UdpService(Int32.Parse(arguments.Port));
            Transmission transmission = service.ReceiveTransmission();
            if (!transmission.CheckTransmission(arguments.DirPath))
            {
                Console.WriteLine("\r\nTransmission failed check!");
            }
            
            LogWriter.LogWrite("Transmission on Port: " + arguments.Port + " from " + service.ip +
                               "\r\nReceived: " + transmission._dataPackets.Count + " packages out of " + transmission._initialPacket.FileSize +
                               "\r\nPackets lost: " + (transmission._initialPacket.FileSize - transmission._dataPackets.Count) +
                               "\r\nReceiving took: " + transmission.timeElapsed + 
                               "\r\n");
            Console.WriteLine("\r\nLog created!");
            Environment.Exit(0);
        }
    }
}