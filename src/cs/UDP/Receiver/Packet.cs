namespace Receiver;

public interface IPacket
{
    public Packet Parse(string s);
}

public abstract class Packet
{
    public int Sequence { get; set; }
    public string PacketMD5 { get; set; }
}

public class InitialPacket : Packet , IPacket
{
    public string Filename { get; private init; }
    public int Filesize { get; private init; }
    
    public Packet Parse(string s)
    {
        string[] parts = s.Split("\u0000");
        
        InitialPacket initialPacket = new InitialPacket()
        {
            Sequence = int.Parse(parts[0]),
            Filesize = int.Parse(parts[1]),
            Filename = parts[2],
            PacketMD5 = parts[3]
            
        };

        string hash = EncryptionHelper.ComputeMD5(initialPacket.Filename);
        if (initialPacket.PacketMD5.Equals(hash,StringComparison.InvariantCultureIgnoreCase))
        {
            return initialPacket;
        }

        throw new Exception("my body is broken");
    }
}

public class DataPacket : Packet, IPacket
{
    public string Data { get; set; }
    
    public Packet Parse(string s)
    {
        string[] parts = s.Split("\u0000");
        
        DataPacket dataPacket = new DataPacket()
        {
            Sequence = int.Parse(parts[0]),
            Data = parts[1],
            PacketMD5 = parts[2]
        };
        
        string hash = EncryptionHelper.ComputeMD5(dataPacket.Data);
        if (dataPacket.PacketMD5.Equals(hash,StringComparison.InvariantCultureIgnoreCase))
        {
            return dataPacket;
        }
        throw new Exception("my body is broken");
    }
}

public class EndPacket : Packet , IPacket
{
    public string FileMD5 { get; set; }
    
    public Packet Parse(string s)
    {
        string[] parts = s.Split("\u0000");
        EndPacket endPacket = new EndPacket()
        {
            Sequence = int.Parse(parts[0]),
            FileMD5 = parts[1]
        };

        return endPacket;
    }
}