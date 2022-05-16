using Receiver;

namespace Transmitter
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("-=[UDP TRANSMITTER]=-");
            ProgramArguments programArguments = new ProgramArguments(args);
            
            UdpService.TransmitMessage(programArguments.FilePath,programArguments.Ip,programArguments.Port);
            
            LogWriter.LogWrite("Transmission on Port: " + programArguments.Port + " to " + programArguments.Ip +
                               "\r\nSent: " +  
                               "\r\nSending took: " +
                               "\r\n");
            Console.WriteLine("\r\nLog created!");
        }
    }
}