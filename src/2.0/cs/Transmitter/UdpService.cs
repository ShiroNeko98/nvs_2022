using System.Net.Sockets;
using System.Text;
using Receiver;
using Receiver.Gui;

namespace Transmitter
{
    public static class UdpService
    {
        private const int BufferSize = 4096;

        public static void TransmitMessage(string filePath, string ip, int port)
        {
            try
            {
                FileInfo fi = new FileInfo(filePath);
                long fileSize = fi.Length;
                long packets =  (long)Math.Ceiling((double)(fi.Length / BufferSize));
                using (Stream stream = File.Open(filePath,FileMode.Open))
                {
                    byte[] buffer = new byte[BufferSize];
                    UdpClient client = new UdpClient();
                    client.Connect(ip,port);
                    

                    string initPacket = "0000\u0000"  + Path.GetFileName(filePath) + "\u0000"  + fileSize + "\u0000" + packets;
                    client.Send(Encoding.ASCII.GetBytes(initPacket));
                    Thread.Sleep(5);
                    while ((stream.Read(buffer, 0, BufferSize)) > 0)
                    {
                        client.Send(buffer);
                    }
                    
                    client.Close();
                }
            }
            catch (Exception e)
            {
                LogWriter.LogWrite("Error: " + e.Message);
                throw;
            }
        }
    }
}
