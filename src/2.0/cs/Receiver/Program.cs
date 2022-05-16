namespace Receiver
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("-=[UDP RECEIVER]=-");
            ProgramArguments arguments = new ProgramArguments(args);
            UdpService.ReceiveTransmission(arguments.DirPath,arguments.Port);
          
            
           /* LogWriter.LogWrite("Transmission on Port: " + arguments.Port + " from " + service.ip +
                               "\r\nReceived: " + transmission._dataPackets.Count + " packages out of " + transmission._initialPacket.FileSize +
                               "\r\nPackets lost: " + (transmission._initialPacket.FileSize - transmission._dataPackets.Count) +
                               "\r\nReceiving took: " + transmission.timeElapsed + 
                               "\r\n"); */
            //Console.WriteLine("\r\nLog created!");
            Console.ReadLine();
            Environment.Exit(0);
        }
    }
}