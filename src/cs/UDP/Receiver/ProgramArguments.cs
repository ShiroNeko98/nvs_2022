namespace Receiver
{
    public class ProgramArguments
    {
        public string DirPath { get; set; }
        private const string HelpText  = "Program Args:\r\n-dir [path]\r\n(optional) -h Help Text";

        public ProgramArguments(string[] args)
        {
            if (args.Length is 0 or 1)
            {
                Console.WriteLine(HelpText);
                Environment.Exit(0);
            }

            DirPath = args[1];

            try
            {
                
            }
            catch (Exception)
            {
                Console.WriteLine(HelpText);
                Environment.Exit(0);
            }
        }
    }
}