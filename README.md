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

