import java.io.File;
import java.io.FileInputStream;
import java.io.IOException;
import java.net.DatagramPacket;
import java.net.DatagramSocket;
import java.net.InetAddress;
import java.security.MessageDigest;
import java.security.NoSuchAlgorithmException;
import java.util.Arrays;

public class Transmitter {

    private final DatagramSocket datagramSocket;
    private final InetAddress inetAddress;

    private final int PORT = 1234;
    private final String NULL_TERMINATED = "\u0000";

    public Transmitter(DatagramSocket datagramSocket, InetAddress inetAddress) {
        this.datagramSocket = datagramSocket;
        this.inetAddress = inetAddress;
    }

    public static void main(String[] args) throws IOException, InterruptedException, NoSuchAlgorithmException {
        if (args.length != 2) {
            System.out.println("Not enough or too many arguments");
            System.exit(1);
        }

        // start transmitter
        System.out.println("Starting transmitter ...");

        Transmitter transmitter = new Transmitter(new DatagramSocket(), InetAddress.getByName(args[1]));
        transmitter.sendData(args[0]);

        System.out.println("File transmitted. Closing program ...");
    }

    private void sendData(String filePath) throws IOException, InterruptedException, NoSuchAlgorithmException {
        File file = new File(filePath);

        // send initial packet
        for (int i = 0; i < 3; i++) {
            String initialData = "0" + NULL_TERMINATED +
                                 file.getName() + NULL_TERMINATED +
                                 file.length();
            sendAndWait(initialData);
        }

        // send text of file
        int byteRead = 0;
        int copyStartIndex = 0;
        byte[] bytesOfFile = new byte[(int) file.length()];

        FileInputStream fileInputStream = new FileInputStream(file);
        while (byteRead != file.length()) {
            byte[] bFile = new byte[256];
            byteRead += fileInputStream.read(bFile);

            // collect read bytes for md5 hash at the end of transmission
            for (int i = 0; i < bFile.length; i++) {
                if (bFile[i] == 0) {
                    // EOF reached => no need for further copy
                    break;
                }

                bytesOfFile[copyStartIndex + i] = bFile[i];
            }
            copyStartIndex = byteRead;

            sendAndWait(byteRead + NULL_TERMINATED + Arrays.toString(bFile));
        }
        fileInputStream.close();

        // send end packet with md5 hash
        MessageDigest md = MessageDigest.getInstance("MD5");
        md.update(bytesOfFile);
        byte[] hash = md.digest();

        for (int i = 0; i < 3; i++) {
            String endData = "-1" + new String(hash);
            sendAndWait(endData);
        }
    }

    private void sendAndWait(String data) throws IOException, InterruptedException {
        byte[] buffer = data.getBytes();
        DatagramPacket datagramPacket = new DatagramPacket(buffer, buffer.length, inetAddress, PORT);
        datagramSocket.send(datagramPacket);

        Thread.sleep(1000);
    }
}
