namespace Receiver
{
    public class ProgramArguments
    {
        public string DirPath { get; set; }
        public int Port { get; set; }

        public int Count { get; set; } = 1;
        
        private const string HelpText  = "Program Args:\r\n-dir [path]\r\n-port [port]\r\n(optional) -count [count]\r\n(optional) -h Help Text";

        public ProgramArguments(string[] args)
        {
            if (args.Length is 0 or 1 or 2 )
            {
                Console.WriteLine(HelpText);
                Environment.Exit(0);
            }
            
            try
            {
                DirPath = args[1];
                Port = int.Parse( args[3]);
            }
            catch (Exception)
            {
                Console.WriteLine(HelpText);
                Environment.Exit(0);
            }

            try
            {
                Count = int.Parse(args[5]);
            }
            catch (Exception e)
            {
                //ignore
            }
        }
    }
}