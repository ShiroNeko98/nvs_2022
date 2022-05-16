namespace Transmitter
{
    public class ProgramArguments
    {
        public string Ip { get; set; }
        public int Port { get; set; }
        public string FilePath { get; set; }

        public int Count { get; set; } = 1;
        
        private const string HelpText  = "Program Args:\r\n-ip [remote ip address]\r\n-port [remote port]\r\n-file [file path]\r\n -count [count]\r\n(optional) -h Help Text";

        public ProgramArguments(string[] args)
        {
            if (args.Length is 0 or 1)
            {
                Console.WriteLine(HelpText);
                Environment.Exit(0);
            }
            
            try
            {
                if (args[0].Equals("-ip"))
                {
                    Ip = args[1];
                }

                if (args[2].Equals("-port"))
                {
                    Port = int.Parse(args[3]);
                }

                if (args[4].Equals("-file"))
                {
                    FilePath = args[5];
                }

                
            }
            catch (Exception)
            {
                Console.WriteLine(HelpText);
                Environment.Exit(0);
            }
        }
    }
}