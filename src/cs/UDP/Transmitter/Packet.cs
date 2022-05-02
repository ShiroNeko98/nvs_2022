using System.Text;
namespace Transmitter
{
    public class InitPacket
    {
        public string Sequence { get; set; }
        public int TotalPacketNumber { get; set; }
        public string FileName { get; set; }
        
        public byte[] GetBytes()
        {
            string packetString = Sequence + "\u0000"
                                           + TotalPacketNumber + "\u0000"
                                           + FileName + "\u0000";
            return Encoding.ASCII.GetBytes(packetString);
        }
    }

    public class DataPacket
    {
        public string Sequence { get; set; }
        public byte[] Data { get; set; }

        public byte[] GetBytes()
        {
            //this is dumb! 
            // like Arrays.toString() in java i guess ?? 
            string sb = "[" + string.Join(",", Data) + "]";
            
            string packetString = Sequence + "\u0000" 
                                           + sb + "\u0000";
            return Encoding.ASCII.GetBytes(packetString);
        }
    }

    public class EndPacket
    {
        public string Sequence { get; set; }
        public string FileMD5 { get; set; }

        public byte[] GetBytes()
        {
            string packetString = Sequence + "\u0000" 
                                           + FileMD5;
            return Encoding.ASCII.GetBytes(packetString);
        }
    }
}