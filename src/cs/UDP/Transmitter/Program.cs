namespace Transmitter
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("-=[UDP TRANSMITTER]=-");
            ProgramArguments programArguments = new ProgramArguments(args);
            
            string message = File.ReadAllText(programArguments.FilePath);

            TransmissionService transmissionService = new TransmissionService(message, Path.GetFileName(programArguments.FilePath));
            transmissionService.Transmit(programArguments.Ip,programArguments.Port);
        }
    }
}