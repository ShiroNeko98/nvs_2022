using System;
using Receiver;

static void Main(string[] args)
{
    Receiver.Receiver receiver = new Receiver.Receiver(int.Parse(args[0]));
    Transmission transmission = receiver.ReceiveTransmission();
    if (transmission.CheckTransmission())
    {
        transmission.WriteTransmission(args[1]);
    }
    Environment.Exit(0);
}