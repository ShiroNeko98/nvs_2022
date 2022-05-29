### java Transmitter
Check whether Java is installed on the machine with: `java -version`
1. Navigate to `nvs_2022/java/src/` directory
2. `javac Transmitter.java`
3. `java Transmitter localhost test.txt`

`java Transmitter <ip address> <path of file> [ optional ]`

Optional:
-h: print help page  
-s <integer>: size of data, default: 1472  
-p <integer>: port number, default: 11000  
-sl <integer>: sleep timer in milliseconds, default: 1000


### cs Receiver
`Receiver.exe -dir <path> -port <port>`  
optional: -h for help text

### cs Transmitter
Transmitter.exe -ip [remote ip address] -port [remote port] -file [file path]  
optional: -h for help text

## Python receiver
python receiver.py port


## Fehlererkennung bei UDP Paketverdopplung
Der Fall tritt auf wenn wir keine Sequenznummer im Datenpaket haben:
1. Transmitter schickt das x. Paket
2. UDP verdoppelte das Paket, dh. der Receiver bekommt 2 Mal das gleiche Paket
3. Receiver bekommt 1. x Paket und sendet Sequenznummer x als ACK zurück
4. Bevor Transmitter das Paket x+1 schickt, bekommt der Receiver bereits das Paket x ein zweites Mal
5. Receiver bekommt 2. x Paket und sendet Sequenznummer x als ACK zurück
6. Transmitter arbeitet weiter und schickt Paket x+1
7. Transmitter bekommt x als Sequenznummer, erwartet aber x+1
8. ...

