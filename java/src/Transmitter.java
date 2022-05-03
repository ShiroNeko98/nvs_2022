import java.io.*;
import java.math.BigInteger;
import java.net.DatagramPacket;
import java.net.DatagramSocket;
import java.net.InetAddress;
import java.nio.charset.Charset;
import java.nio.charset.StandardCharsets;
import java.security.MessageDigest;
import java.security.NoSuchAlgorithmException;
import java.util.ArrayList;
import java.util.List;
import java.util.logging.FileHandler;
import java.util.logging.Level;
import java.util.logging.Logger;
import java.util.logging.SimpleFormatter;

public class Transmitter {
    private static final Logger LOG = Logger.getLogger("JavaTransmitterLog");

    private static int SLEEP = 1000;
    private static int DATA_SIZE = 1024;
    private static int PORT = 11000;

    private final DatagramSocket datagramSocket;
    private final InetAddress inetAddress;

    private final String NULL_TERMINATED = "\u0000";

    public Transmitter(DatagramSocket datagramSocket, InetAddress inetAddress) {
        this.datagramSocket = datagramSocket;
        this.inetAddress = inetAddress;
    }

    public static void main(String[] args) throws IOException, InterruptedException, NoSuchAlgorithmException {
        //configureLogger();

        if (args.length < 2) {
            if (args[0].equals("-h")) {
                printHelpText();
                System.exit(0);
            }

            LOG.log(Level.SEVERE, "Not enough or too many arguments");
            System.exit(1);
        }

        setOptionalParameters(args);

        // start transmitter
        LOG.info("Starting transmission ...");

        Transmitter transmitter = new Transmitter(new DatagramSocket(), InetAddress.getByName(args[0]));
        transmitter.sendData(args[1]);

        LOG.info("File transmitted. Closing program ...");
    }

    private static void configureLogger() throws IOException {
        FileHandler fh = new FileHandler("../../logs/java/Transmitter.log");

        SimpleFormatter formatter = new SimpleFormatter();
        fh.setFormatter(formatter);

        LOG.addHandler(fh);
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
                LOG.log(Level.SEVERE, "ERROR: not supported parameter " + param + " found");
                System.exit(1);
            }
        }
    }

    private void sendData(String filePath) throws IOException, InterruptedException, NoSuchAlgorithmException {
        File file = new File(filePath);

        // send initial packet
        for (int i = 0; i < 3; i++) {
            String initialData = "0" + NULL_TERMINATED +
                                 (file.length() / DATA_SIZE) + NULL_TERMINATED +
                                 file.getName();
            sendAndWait(initialData.getBytes());
        }

        // send file content
        byte[] bytesOfFile = sendFileContent(file);

        // send end packet with md5 hash
        MessageDigest md = MessageDigest.getInstance("MD5");
        md.update(bytesOfFile);
        String hash = new BigInteger(1, md.digest()).toString(16);

        for (int i = 0; i < 3; i++) {
            String endData = "-1" + NULL_TERMINATED + hash;
            sendAndWait(endData.getBytes());
        }
    }

    private void sendAndWait(byte[] buffer) throws IOException, InterruptedException {
        //byte[] buffer = data.getBytes();
        DatagramPacket datagramPacket = new DatagramPacket(buffer, buffer.length, inetAddress, PORT);
        datagramSocket.send(datagramPacket);

        Thread.sleep(SLEEP);
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
        int sequenceNumber = 1;
        int byteRead = 0;
        int copyStartIndex = 0;
        byte[] bytesOfFile = new byte[(int) file.length()];

        FileInputStream fileInputStream = new FileInputStream(file);
        while (byteRead != file.length()) {
            byteRead += fileInputStream.read(bytesOfFile);
        }

        /*List<Byte> bFile = new ArrayList<>();
        for (int i = 0; i < bytesOfFile.length; i++) {

            /*String dataStr = sequenceNumber + NULL_TERMINATED;
            for (byte b : dataStr.getBytes()) {
                bFile.add(b);
            }

            bFile[i] = bytesOfFile[i];

            byte[] packet = new byte[DATA_SIZE];
            for (int j = 0; j < packet.length; j++) {
                packet[j] = b
            }
            sendAndWait(bFile);
            sequenceNumber++;
        }*/

        byte[] bFile = new byte[]{49, 0, 104, 97, 108, 108, 111};

        String text = 1 + NULL_TERMINATED + "hallo";
        sendAndWait(text.getBytes(Charset.forName("ASCII")));

        fileInputStream.close();

       /* BufferedInputStream bis = new BufferedInputStream(new FileInputStream("java/src/hallo.txt"));
        int length=bis.available();
        //DatagramSocket ds = new DatagramSocket();
        byte[] buf = new byte[length];
        bis.read(buf);
        DatagramPacket dp = new DatagramPacket(buf,buf.length, InetAddress.getByName("127.0.0.1"),PORT);
        datagramSocket.send(dp);
        datagramSocket.close();
        bis.close();*/

        return bytesOfFile;
    }

    public static byte[] hexStringToByteArray(String s) {
        int len = s.length();
        byte[] data = new byte[len / 2];
        for (int i = 0; i < len; i += 2) {
            data[i / 2] = (byte) ((Character.digit(s.charAt(i), 16) << 4)
                                  + Character.digit(s.charAt(i+1), 16));
        }
        return data;
    }
}
