import java.io.IOException;
import java.net.DatagramPacket;
import java.net.DatagramSocket;
import java.net.SocketException;

public class Receiver {
    private DatagramSocket datagramSocket;
    private byte[] buffer = new byte[1024];

    public Receiver(DatagramSocket datagramSocket) {
        this.datagramSocket = datagramSocket;
    }

    public void receive() throws IOException {
        DatagramPacket datagramPacket = new DatagramPacket(buffer, buffer.length);
        datagramSocket.receive(datagramPacket);

        System.out.println("debug");
    }

    public static void main(String[] args) throws IOException {
        DatagramSocket datagramSocket = new DatagramSocket(11000);
        Receiver receiver = new Receiver(datagramSocket);
        receiver.receive();
    }
}
