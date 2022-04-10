import java.io.File;
import java.io.FileInputStream;
import java.io.IOException;
import java.net.DatagramPacket;
import java.net.DatagramSocket;
import java.net.InetAddress;
import java.util.Arrays;
import java.util.Scanner;

public class Transmitter {

    private final DatagramSocket datagramSocket;
    private final InetAddress inetAddress;

    private final int PORT = 1234;

    public Transmitter(DatagramSocket datagramSocket, InetAddress inetAddress) {
        this.datagramSocket = datagramSocket;
        this.inetAddress = inetAddress;
    }

    public static void main(String[] args) throws IOException, InterruptedException {
        Transmitter transmitter = new Transmitter(new DatagramSocket(), InetAddress.getByName(args[1]));
        System.out.println("Send datagram packets to a server.");
        transmitter.sendData(args[0]);
    }

    private void sendData(String filePath) throws IOException, InterruptedException {
        File file = new File(filePath);

        // send initial packet
        for (int i = 0; i < 3; i++) {
            String initialData = "0" +
                                 file.getName() + "\u0000" +
                                 file.length();
            sendAndWait(initialData);
        }

        // send text of file
        int byteRead = 0;

        FileInputStream fileInputStream = new FileInputStream(file);
        while (byteRead != file.length()) {
            byte[] bFile = new byte[256];
            byteRead += fileInputStream.read(bFile);

            sendAndWait(byteRead + Arrays.toString(bFile));
        }
        fileInputStream.close();

        // send end packet
    }

    private void sendAndWait(String data) throws IOException, InterruptedException {
        byte[] buffer = data.getBytes();
        DatagramPacket datagramPacket = new DatagramPacket(buffer, buffer.length, inetAddress, PORT);
        datagramSocket.send(datagramPacket);

        Thread.sleep(1000);
    }
}
