using System.Net.Sockets;
using Receiver.Gui;

namespace Transmitter
{
    public class TransmissionService
    {
        private InitPacket _initPacket;
        private IList<DataPacket> _dataPackets;
        private EndPacket _endPacket;

        public TransmissionService(string message,string filename)
        {
            _dataPackets = new List<DataPacket>();
            Parse(message,filename);
        }

        public void Transmit(string ip, int port)
        {
            Console.WriteLine("Start transmitting File ...");
            UdpClient udpClient = new UdpClient();
            try
            {
                udpClient.Connect(ip,port);

                byte[] initPacketBytes = _initPacket.GetBytes();
                for (int i = 0; i < 3; i++)
                {
                    udpClient.Send(initPacketBytes,initPacketBytes.Length);
                }

                ProgressBar progress = new ProgressBar();
                
                foreach (DataPacket packet in _dataPackets)
                {
                    byte[] dataPacketBytes = packet.GetBytes();
                    progress.Report((double)int.Parse(packet.Sequence)/_initPacket.TotalPacketNumber);
                    udpClient.Send(dataPacketBytes, dataPacketBytes.Length);
                    Thread.Sleep(1000);
                }
                progress.Dispose();
                byte[] endPacketBytes = _endPacket.GetBytes();
                for (int i = 0; i < 3; i++)
                {
                    udpClient.Send(endPacketBytes, endPacketBytes.Length);
                }
                
                Console.WriteLine("\r\nFinished Transmitting File!");
                udpClient.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void Parse(string message,string filename)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(message))
                {
                    throw new Exception("Unable to create Packets, message was null or whitespace");
                }
                
                string messageMd5 = EncryptionHelper.ComputeMD5(message);
                int sequence = 1;

                IList<string> dataChunks = GetChunks(message,1024).ToList();

                foreach (string chunk in dataChunks)
                {
                    DataPacket dataPacket = new DataPacket()
                    {
                        Sequence = sequence.ToString(),
                        Data = chunk
                    };
                    sequence++;
                    _dataPackets.Add(dataPacket);
                }

                _initPacket = new InitPacket()
                {
                    Sequence = "0",
                    FileName = filename[..Math.Min(filename.Length,1024)],
                    TotalPacketNumber = sequence -1
                };

                _endPacket = new EndPacket()
                {
                    FileMD5 = messageMd5,
                    Sequence = "-1"
                };
                
                Console.WriteLine("Packets Created:\r\n Total Number: " + (sequence -1 ) +"\r\n");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        
        static IEnumerable<string> GetChunks(string str, int maxChunkSize) {
            for (int i = 0; i < str.Length; i += maxChunkSize) 
                yield return str.Substring(i, Math.Min(maxChunkSize, str.Length-i));
        }
    }
}