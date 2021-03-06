namespace Receiver
{
    public  class Packet
    {
        public  int Sequence { get; set; }

        public static Type GetType(string s)
        {
            string[] parts = s.Split("\u0000");
            string sequence = parts[0];
            return sequence switch
            {
                "0" => typeof(InitialPacket),
                "-1" => typeof(EndPacket),
                _ => typeof(DataPacket)
            };
        }
    }

    public class InitialPacket : Packet 
    {
        public InitialPacket(string s)
        {
            Parse(s);
        }
        
        public string FileName { get; set; }
        public int FileSize { get; set; }
        
        public void Parse(string s)
        {
            string[] parts = s.Split("\u0000");
            Sequence = int.Parse(parts[0]);
            FileSize = int.Parse(parts[1]);
            FileName = parts[2];
        }
    }
    
    public class DataPacket : Packet 
    {

        public DataPacket(string s)
        {
            Parse(s);
        }
        
        public byte[] Data { get; set; }
        
        public void Parse(string s)
        {
            string[] parts = s.Split("\u0000");
            Sequence = int.Parse(parts[0]);
            List<byte> bytes = new List<byte>();
            Data = Array.ConvertAll(parts[1].Replace("[", "").Replace("]", "").Split(","), s => (byte) int.Parse(s));
        }
    }
    
    public class EndPacket : Packet 
    {

        public EndPacket(string s)
        {
            Parse(s);
        }
        
        public string FileMd5 { get; set; }
        
        public void Parse(string s)
        {
            string[] parts = s.Split("\u0000");
            Sequence = int.Parse(parts[0]);
            FileMd5 = parts[1];
        }
    }
}