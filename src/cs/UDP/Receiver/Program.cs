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
            if (transmission.CheckTransmission())
            {
                Console.WriteLine("\r\n\r\nReceived File:\r\n\r\n");
                Console.Write(transmission.GetTransmissionContent());
                transmission.WriteTransmission(arguments.DirPath);
            }
            else
            {
                Console.Write("Content received: " + transmission.GetTransmissionContent());
                Console.WriteLine("\r\nTransmission failed check!");
            }

            LogWriter.LogWrite("Transmission on Port: " + arguments.Port + " from " + IPAdresse +
                               "\r\nReceived: " + Transmission._dataPackets.Count + " packages out of " + Transmission._initialPacket.FileSize +
                               "\r\nPackets lost: " + Transmission._initialPacket.FileSize - Transmission._dataPackets.Count +
                               "\r\nReceiving took: " + Dauer + 
                               "\r\n");
            Console.WriteLine("\r\nLog created!");
            Environment.Exit(0);
        }
    }
}