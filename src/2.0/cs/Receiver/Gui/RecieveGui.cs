using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Receiver.Gui;

public class RecieveGui
{

    protected static int OrigRow;
    protected static int OrigCol;

    private int _packets;

    public DateTime DateTime;
    
    public RecieveGui()
    {
        Console.Clear();
        _packets = 0;
        OrigRow = Console.CursorTop;
        OrigCol = Console.CursorLeft;
    }
    
    protected static void WriteAt(string s, int x, int y)
    {
        try
        {
            Console.SetCursorPosition(OrigCol+x, OrigRow+y);
            Console.Write(s);
        }
        catch (ArgumentOutOfRangeException e)
        {
            Console.Clear();
            Console.WriteLine(e.Message);
        }
    }

    public  void InitReceived(long x)
    {
        DateTime = DateTime.Now;
        WriteAt("Transmisstion RecieveStarted",0,0);
        WriteAt("Packets received: ",0,1);
        WriteAt(_packets.ToString(),23,1);
        WriteAt("/" +x.ToString(),40,1);
    }

    public  void NewPacket(int p)
    {
        _packets += p;
        WriteAt(_packets.ToString(),23,1);
    }

    public void EndReceived()
    {
        TimeSpan time = DateTime.Now - DateTime;
        WriteAt("Finished! Time Elapsed: " + time,0,15);
    }

}