import java.io.File;
import java.io.FileInputStream;
import java.io.IOException;
import java.net.DatagramPacket;
import java.net.DatagramSocket;
import java.net.InetAddress;
import java.net.SocketTimeoutException;
import java.rmi.UnexpectedException;
import java.util.concurrent.TimeoutException;

public class Transmitter {
    private static int DATA_SIZE = 4096;
    private static int PORT = 11000;

    private final DatagramSocket datagramSocket;
    private final InetAddress inetAddress;

    private final String NULL_TERMINATED = "\u0000";

    private int timeOutRetry = 3;
    private int packetErrorRetry = 1;

    public Transmitter(DatagramSocket datagramSocket, InetAddress inetAddress) {
        this.datagramSocket = datagramSocket;
        this.inetAddress = inetAddress;
    }

    public static void main(String[] args) throws IOException, TimeoutException {
        if (args.length < 2) {
            if (args[0].equals("-h")) {
                printHelpText();
                System.exit(0);
            }

            System.out.println("Not enough or too many arguments");
            System.exit(1);
        }

        setOptionalParameters(args);

        // start transmitter
        System.out.println("Starting transmission ...");

        DatagramSocket datagramSocket = new DatagramSocket(12000);
        datagramSocket.setSoTimeout(2000); // in milliseconds

        Transmitter transmitter = new Transmitter(datagramSocket, InetAddress.getByName(args[0]));
        transmitter.sendData(args[1]);

        System.out.println("File transmitted. Closing program ...");
    }

    private static void printHelpText() {
        System.out.print(
                "The program must be executed like: java Transmitter <ip address> <path of file> [optional]\n\n" +
                "Optional:\n" +
                "-h ... print help page\n" +
                "-s <integer> ... set size of data\n" +
                "-p <integer> ... set port\n");
    }

    private static void setOptionalParameters(String[] args) {
        for (int i = 2; i < args.length; i++) {
            String param = args[i];

            if (param.equals("-s")) {
                i++;
                DATA_SIZE = Integer.parseInt(args[i]);
            } else if (param.equals("-p")) {
                i++;
                PORT = Integer.parseInt(args[i]);
            } else {
                System.out.println("ERROR: not supported parameter " + param + " found");
                System.exit(1);
            }
        }
    }

    private void sendData(String filePath) throws IOException, TimeoutException {
        File file = new File(filePath);

        // send initial packet
        int packetCount = (int) Math.ceil((double) file.length() / DATA_SIZE);
        String initialData = "0" + NULL_TERMINATED +
                             file.getName() + NULL_TERMINATED +
                             file.length() + NULL_TERMINATED +
                             packetCount;
        DatagramPacket datagramPacket =
                new DatagramPacket(initialData.getBytes(), initialData.getBytes().length, inetAddress, PORT);
        datagramSocket.send(datagramPacket);

        // send file content
        sendFileContent(file);
    }

    private boolean sendAndWait(byte[] buffer) throws IOException, TimeoutException {
        DatagramPacket datagramPacket = new DatagramPacket(buffer, buffer.length, inetAddress, PORT);
        datagramSocket.send(datagramPacket);

        try {
            datagramPacket = new DatagramPacket(new byte[3], 3);
            datagramSocket.receive(datagramPacket);
        } catch (SocketTimeoutException e) {
            if (timeOutRetry > 0) {
                timeOutRetry--;
                sendAndWait(buffer);
            } else {
                throw new TimeoutException(new String(datagramPacket.getData()));
            }
        }

        return new String(datagramPacket.getData()).equals("ACK");
    }

    /**
     * Send content of file after splitting into multiple packets with fixed size.
     *
     * @param file file to be sent
     * @throws IOException problem with finding or reading file
     */
    private void sendFileContent(File file) throws IOException, TimeoutException {
        int byteRead = 0;

        FileInputStream fileInputStream = new FileInputStream(file);
        while (byteRead != file.length()) {
            byte[] bFile = new byte[DATA_SIZE];
            byteRead += fileInputStream.read(bFile);

            if (!sendAndWait(bFile)) {
                if (packetErrorRetry > 0) {
                    System.out.println("Error during transmission or on receiver side\n" +
                                       "Retry sending packet once ...");
                    packetErrorRetry--;
                    sendAndWait(bFile);
                } else {
                    throw new UnexpectedException("Transmission not acknowledged by receiver");
                }
            }
        }
    }
}
