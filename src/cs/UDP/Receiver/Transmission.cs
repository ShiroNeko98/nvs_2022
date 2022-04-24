using Receiver.Gui;
namespace Receiver
{

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
        private ProgressBar _progressBar;
        private IList<DataPacket> _dataPackets = new List<DataPacket>();

        public void AddPacket(string s)
        {

            Type packetType = Packet.GetType(s);
            _progressBar ??= new ProgressBar();
            
            if (_initialPacket == null && packetType == typeof(InitialPacket) )
            {
                try
                {
                    _initialPacket = new InitialPacket(s);
                    Console.WriteLine($"Received init Packet for File: {_initialPacket.FileName}\r\n Expecting: {_initialPacket.FileSize} Packets.");
                    return;
                }
                catch (Exception)
                {
                    return;
                }
            }

            if (packetType == typeof(EndPacket))
            {
                try
                {
                    _endPacket = new EndPacket(s);
                    _progressBar.Dispose();
                    Console.WriteLine("Received end Packet!");
                    IsEnd = true;
                    return;
                }
                catch (Exception)
                {
                    // ignored
                    return;
                }
            }

            if (packetType == typeof(DataPacket))
            {
                try
                {
                    DataPacket dataPacket = new DataPacket(s);
                    _dataPackets.Add(dataPacket);
                  
                    if (_initialPacket != null) 
                        _progressBar.Report((double)dataPacket.Sequence / _initialPacket.FileSize);
                }
                catch (Exception)
                {
                    // ignored
                }
            }
        }

        public bool CheckTransmission()
        {
            if (!IsEnd)
            {
                throw new Exception("Transmission has not ended yet.");
            }
            
            _dataPackets = _dataPackets.OrderBy(d => d.Sequence).ToList();
            string data = _dataPackets.Aggregate("", (current, packet) => current + packet.Data);
            string hash = EncryptionHelper.ComputeMD5(data);

            bool result = _endPacket.FileMD5.Equals(hash, StringComparison.InvariantCultureIgnoreCase);

            if (result)
            {
                return true;
            }
            Console.WriteLine("Received packages: " + _dataPackets.Count);
            return false;
        }

        public void WriteTransmission(string dirToWrite)
        {
            _dataPackets = _dataPackets.OrderBy(d => d.Sequence).ToList();
            string data = _dataPackets.Aggregate("", (current, packet) => current + packet.Data);

            string path = dirToWrite + Path.DirectorySeparatorChar +_initialPacket.FileName;
            File.WriteAllText(path,data);
            Console.WriteLine("File saved!");
        }

        public string GetTransmissionContent()
        {
            if (!IsEnd)
            {
                throw new Exception("Transmission has not ended yet.");
            }
            _dataPackets = _dataPackets.OrderBy(d => d.Sequence).ToList();
            string data = _dataPackets.Aggregate("", (current, packet) => current + packet.Data);
            return data;
        }
    }
}