using System.Reflection;
using System.Text;

namespace Receiver;

public class LogWriter
{
    private static string m_exePath = string.Empty;
    
    public LogWriter(string logMessage)
    {
        LogWrite(logMessage);
    }
    public static void LogWrite(string logMessage)
    {
        m_exePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        if (!File.Exists(m_exePath + "\\" + "log.txt"))
        { 
            string s = ""; 
            s = DateTime.Now.ToString("h:mm:ss tt"); FileStream fs = File.Create(m_exePath + "\\" + "log.txt"); 
            Byte[] info = new UTF8Encoding(true).GetBytes("File Created:"+s+"\r\n"); 
            fs.Write(info, 0, info.Length);fs.Close(); 
        }
        try
        {
            using (StreamWriter w = File.AppendText(m_exePath + "\\" + "log.txt"))
            {
                Log(logMessage, w);
            }
        }
        catch (Exception e)
        {
        }
    }

    public static void Log(string logMessage, TextWriter txtWriter)
    {
        try
        {
            txtWriter.Write("\r\nLog Entry : ");
            txtWriter.WriteLine("{0} {1}", DateTime.Now.ToLongTimeString(), DateTime.Now.ToLongDateString());
            txtWriter.WriteLine("  :");
            txtWriter.WriteLine("  :{0}", logMessage);
            txtWriter.WriteLine("-------------------------------");
        }
        catch (Exception e)
        {
        }
    }
}