import java.io.File;
import java.io.FileInputStream;
import java.io.IOException;
import java.net.DatagramPacket;
import java.net.DatagramSocket;
import java.net.InetAddress;

public class Transmitter {
    private static int SLEEP = 1000;
    private static int DATA_SIZE = 10; // 4096
    private static int PORT = 11000;

    private final DatagramSocket datagramSocket;
    private final InetAddress inetAddress;

    private final String NULL_TERMINATED = "\u0000";

    public Transmitter(DatagramSocket datagramSocket, InetAddress inetAddress) {
        this.datagramSocket = datagramSocket;
        this.inetAddress = inetAddress;
    }

    public static void main(String[] args) throws IOException, InterruptedException {
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

        Transmitter transmitter = new Transmitter(new DatagramSocket(), InetAddress.getByName(args[0]));
        transmitter.sendData(args[1]);

        System.out.println("File transmitted. Closing program ...");
    }

    private static void printHelpText() {
        System.out.print(
                "The program must be executed like: java Transmitter <ip address> <path of file> [optional]\n\n" +
                "Optional:\n" +
                "-h ... print help page\n" +
                "-s <integer> ... set size of data\n" +
                "-p <integer> ... set port\n" +
                "-sl <integer> ... sleep timer");
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
            } else if (param.equals("-sl")) {
                i++;
                SLEEP = Integer.parseInt(args[i]);
            } else {
                System.out.println("ERROR: not supported parameter " + param + " found");
                System.exit(1);
            }
        }
    }

    private void sendData(String filePath) throws IOException, InterruptedException {
        File file = new File(filePath);

        // send initial packet
        int packetCount = (int) Math.ceil(file.length()) / DATA_SIZE;
        String initialData = "0" + NULL_TERMINATED +
                             file.getName() + NULL_TERMINATED +
                             file.length() + NULL_TERMINATED +
                             packetCount;
        sendAndWait(initialData.getBytes());

        // send file content
        sendFileContent(file);
    }

    private boolean sendAndWait(byte[] buffer) throws IOException, InterruptedException {
        DatagramPacket datagramPacket = new DatagramPacket(buffer, buffer.length, inetAddress, PORT);
        datagramSocket.send(datagramPacket);

        DatagramSocket socket = new DatagramSocket(12000);
        datagramPacket = new DatagramPacket(buffer, buffer.length);
        socket.receive(datagramPacket);

        return buffer.length != 0;
        // TODO timeout after 500 ms -> resend packet
        // TODO after 3 consecutive timeouts -> end program
    }

    /**
     * Send content of file after splitting into multiple packets with fixed size.
     *
     * @param file file to be sent
     * @return content of file in bytes
     * @throws IOException          problem with finding or reading file
     * @throws InterruptedException problem with sending packets
     */
    private byte[] sendFileContent(File file) throws IOException, InterruptedException {
        int byteRead = 0;
        int copyStartIndex = 0;
        byte[] bytesOfFile = new byte[(int) file.length()];

        FileInputStream fileInputStream = new FileInputStream(file);
        while (byteRead != file.length()) {
            byte[] bFile = new byte[DATA_SIZE];
            byteRead += fileInputStream.read(bFile);

            // collect read bytes for md5 hash at the end of transmission
            for (int i = 0; i < bFile.length; i++) {
                if (bFile[i] == 0) {
                    // EOF reached => no need for further copy
                    byte[] lastDataPacket = new byte[(int) (file.length() % DATA_SIZE)];
                    for (i = 0; i < lastDataPacket.length; i++) {
                        lastDataPacket[i] = bFile[i];
                    }

                    sendAndWait(lastDataPacket);

                    fileInputStream.close();
                    return bytesOfFile;
                }

                bytesOfFile[copyStartIndex + i] = bFile[i];
            }
            copyStartIndex = byteRead;

            sendAndWait(bFile);
        }

        throw new RuntimeException();
    }
}
