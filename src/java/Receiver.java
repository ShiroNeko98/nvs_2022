import java.io.IOException;
import java.net.DatagramPacket;
import java.net.DatagramSocket;
import java.net.SocketException;

public class Receiver {

    private final String NULL_TERMINATED = "\u0000";

    private final DatagramSocket datagramSocket;
    private final byte[] buffer = new byte[256];

    public Receiver(DatagramSocket datagramSocket) {
        this.datagramSocket = datagramSocket;
    }

    public void receiveThenSend() {
        while (true) {
            try {
                DatagramPacket datagramPacket = new DatagramPacket(buffer, buffer.length);

                // wait until receive something
                datagramSocket.receive(datagramPacket);

                // print client data
                String messageFromClient = new String(datagramPacket.getData(), 0, datagramPacket.getLength());
                System.out.println("Message from client: " + messageFromClient);

                messageFromClient.split(String.valueOf((char) 0));
            } catch (IOException e) {
                e.printStackTrace();
                break;
            }
        }
    }

    public static void main(String[] args) throws SocketException {
        Receiver receiver = new Receiver(new DatagramSocket(1234));
        receiver.receiveThenSend();
    }
}
