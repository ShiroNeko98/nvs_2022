using Receiver;

namespace Transmitter
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("-=[UDP TRANSMITTER]=-");
            ProgramArguments programArguments = new ProgramArguments(args);
            
            byte[] message = File.ReadAllBytes(programArguments.FilePath);

            TransmissionService transmissionService = new TransmissionService(message, Path.GetFileName(programArguments.FilePath));
            transmissionService.Transmit(programArguments.Ip,programArguments.Port);
            
            LogWriter.LogWrite("Transmission on Port: " + programArguments.Port + " to " + programArguments.Ip +
                               "\r\nSent: " + transmissionService._dataPackets.Count +
                               "\r\nReceiving took: " + transmissionService.elapsedTime + 
                               "\r\n");
            Console.WriteLine("\r\nLog created!");
        }
    }
}