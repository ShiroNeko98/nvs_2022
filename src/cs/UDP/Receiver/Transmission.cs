using System.Security.Cryptography;
using System.Text;

namespace Receiver;

public interface ITransmission
{
    public void AddPacket(string s);
    public bool CheckTransmission();
    public void WriteTransmission(string dirToWrite);
   
}

public class Transmission : ITransmission
{
    public bool IsEnd;
    private InitialPacket _initialPacket;
    private EndPacket _endPacket;
    private IList<DataPacket> _dataPackets;
    
    public void AddPacket(string s)
    {
        if (_initialPacket == null)
        {
            try
            {
                InitialPacket initialPacket = new InitialPacket();
                initialPacket.Parse(s);
                _initialPacket = initialPacket;
                return;
            }
            catch (Exception)
            {
                return;
            }
        }

        try
        {
            DataPacket dataPacket = new DataPacket();
            dataPacket.Parse(s);
            _dataPackets.Add(dataPacket);
            return;
        }
        catch (Exception)
        {
            //ignore
        }

        try
        {
            EndPacket endPacket = new EndPacket();
            endPacket.Parse(s);
            _endPacket = endPacket;
            IsEnd = true;
        }
        catch (Exception)
        {
            //ignore
        }
    }

    public bool CheckTransmission()
    {
        if (!IsEnd)
        {
            throw new Exception("no end in sight");
        }
        
        _dataPackets = _dataPackets.OrderBy(d => d.Sequence).ToList();
        string data = _dataPackets.Aggregate("", (current, packet) => current + packet.Data);
        string hash = EncryptionHelper.ComputeMD5(data);
        return _endPacket.FileMD5.Equals(hash,StringComparison.InvariantCultureIgnoreCase);
    }
    
    public void WriteTransmission(string dirToWrite)
    {
        _dataPackets = _dataPackets.OrderBy(d => d.Sequence).ToList();
        string data = _dataPackets.Aggregate("", (current, packet) => current + packet.Data);

        string path = dirToWrite + Path.DirectorySeparatorChar + _initialPacket.Filename;
        File.WriteAllText(path,data);
    }
    
}