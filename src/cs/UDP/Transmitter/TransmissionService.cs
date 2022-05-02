using System.Net.Sockets;
using Receiver.Gui;

namespace Transmitter
{
    public class TransmissionService
    {
        private InitPacket _initPacket;
        public IList<DataPacket> _dataPackets;
        private EndPacket _endPacket;
        public TimeSpan elapsedTime;
        
        public TransmissionService(byte[] message,string filename)
        {
            _dataPackets = new List<DataPacket>();
            Parse(message,filename);
        }

        public void Transmit(string ip, int port)
        {
            Console.WriteLine("Start transmitting File ...");
            UdpClient udpClient = new UdpClient();
            DateTime startTime = DateTime.Now;
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
                    
                    Thread.Sleep(1);
                }
                progress.Dispose();
                byte[] endPacketBytes = _endPacket.GetBytes();
                for (int i = 0; i < 3; i++)
                {
                    udpClient.Send(endPacketBytes, endPacketBytes.Length);
                }
                
                Console.WriteLine("\r\nFinished Transmitting File!");
                elapsedTime = DateTime.Now - startTime;
                udpClient.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void Parse(byte[] message,string filename)
        {
            try
            {
                if (message.Length == 0)
                {
                    throw new Exception("Unable to create Packets, message was empty.");
                }
                
                string messageMd5 = EncryptionHelper.ComputeMD5(message);
                int sequence = 1;

                IList<byte[]> dataChunks = GetChunks(message,1024).ToList();

                foreach (byte[] chunk in dataChunks)
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
                
                Console.WriteLine("Packets Created!\t Total Number: " + (sequence -1 ) +"\r\n");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        
        static IEnumerable<byte[]> GetChunks(byte[] message, int maxChunkSize)
        {

            Stack<byte> stack = new Stack<byte>(message);
            
            while (stack.Count > 0)
            {
                byte[] bytes = new byte[maxChunkSize];
                for (int i = 0; i < maxChunkSize; i++)
                {
                    if ( stack.TryPop(out byte b))
                    {
                        bytes[i] = b;
                    }
                    else
                    {
                        break;
                    }
                }

                yield return bytes.ToArray();
            }
        }
    }
}