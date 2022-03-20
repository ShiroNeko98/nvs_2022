import java.io.IOException;
import java.net.*;
import java.util.Scanner;

public class Transmitter {

    private final DatagramSocket datagramSocket;
    private final InetAddress inetAddress;

    public Transmitter(DatagramSocket datagramSocket, InetAddress inetAddress) {
        this.datagramSocket = datagramSocket;
        this.inetAddress = inetAddress;
    }

    public void sendThenReceive() {
        Scanner scanner = new Scanner(System.in);

        while (true) {
            try {
                // read input from console
                String messageToSend = scanner.nextLine();
                byte[] buffer = messageToSend.getBytes();

                DatagramPacket datagramPacket = new DatagramPacket(buffer, buffer.length, inetAddress, 1234);
                datagramSocket.send(datagramPacket);

                // wait for reply from receiver and override datagramPacket
                datagramSocket.receive(datagramPacket);
                String messageFromServer = new String(datagramPacket.getData(), 0, datagramPacket.getLength());
                System.out.println("The server says you said: " + messageFromServer);
            } catch (IOException e) {
                e.printStackTrace();
                break;
            }
        }
    }

    public static void main(String[] args) throws SocketException, UnknownHostException {
        Transmitter transmitter = new Transmitter(new DatagramSocket(), InetAddress.getByName("localhost"));
        System.out.println("Send datagram packets to a server.");
        transmitter.sendThenReceive();
    }
}
