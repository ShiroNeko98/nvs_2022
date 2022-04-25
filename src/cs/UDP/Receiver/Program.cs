namespace Receiver
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("-=[UDP RECEIVER]=-");
            ProgramArguments arguments = new ProgramArguments(args);
            UdpService service = new UdpService(11000);
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
            
            Environment.Exit(0);
        }
    }
}