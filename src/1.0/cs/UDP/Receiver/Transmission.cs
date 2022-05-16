using Receiver.Gui;
namespace Receiver
{

    public interface ITransmission
    {
        public void AddPacket(string s);
        public bool CheckTransmission(string dirToWrite);
        public void WriteTransmission(string dirToWrite);
    }

    public class Transmission : ITransmission
    {

        public bool IsEnd;
        public TimeSpan timeElapsed;
        private DateTime _time;
        public  InitialPacket _initialPacket;
        private EndPacket _endPacket;
        private ProgressBar _progressBar;
        public  IList<DataPacket> _dataPackets = new List<DataPacket>();

        public void AddPacket(string s)
        {

            Type packetType = Packet.GetType(s);
            _progressBar ??= new ProgressBar();
            
            if (_initialPacket == null && packetType == typeof(InitialPacket) )
            {
                try
                {
                    _time = DateTime.Now;
                    _initialPacket = new InitialPacket(s);
                    Console.WriteLine($"Received init Packet for File: {_initialPacket.FileName}\r\nExpecting: {_initialPacket.FileSize} Packets.");
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
                    timeElapsed =  DateTime.Now - _time;
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

        public bool CheckTransmission(string dirToWrite)
        {
            if (!IsEnd)
            {
                throw new Exception("Transmission has not ended yet.");
            }
            
            _dataPackets = _dataPackets.OrderBy(d => d.Sequence).ToList();
            List<byte> bytes =  new List<byte>();
            byte[] data = _dataPackets.Aggregate(bytes, (list, packet) =>
            {
                list.AddRange(packet.Data);
                return list;
            }).ToArray();

            data = TrimEnd(data);
            
            string hash = EncryptionHelper.ComputeMD5(data.ToArray());
            
            bool result = _endPacket.FileMd5.Equals(hash, StringComparison.InvariantCultureIgnoreCase);

            if (result)
            {
                WriteTransmission(dirToWrite);
            }
            
            Console.WriteLine("Received Hash: " + _endPacket.FileMd5);
            Console.WriteLine("Computed Hash: " + hash);
            Console.WriteLine("Received packages: " + _dataPackets.Count);
            return result;
        }

        public void WriteTransmission(string dirToWrite)
        {
            _dataPackets = _dataPackets.OrderBy(d => d.Sequence).ToList();
            List<byte> bytes =  new List<byte>();
            byte[] data = _dataPackets.Aggregate(bytes, (list, packet) =>
            {
                list.AddRange(packet.Data);
                return list;
            }).ToArray();

            data = TrimEnd(data);

            string path = dirToWrite + Path.DirectorySeparatorChar +_initialPacket.FileName;
            File.WriteAllBytes(path,data.ToArray());
            Console.WriteLine("File saved!");
        }
        
        private static byte[] TrimEnd(byte[] array)
        {
            int lastIndex = Array.FindLastIndex(array, b => b != 0);
            Array.Resize(ref array, lastIndex + 1);
            return array;
        }
    }
}