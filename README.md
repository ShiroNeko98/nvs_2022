### java Transmitter
1. javac Transmitter.java
2. java Transmitter test.txt localhost

java Transmitter <ip address> <path of file> { optional }

Optional:
-h: print help page  
-s <integer>: size of data, default: 1472  
-p <integer>: port number, default: 11000  
-sl <integer>: sleep timer in milliseconds, default: 1000


### cs Receiver
Receiver.exe -dir <path>  
default port 11000  
optional: -h for help text

### cs Transmitter
Transmitter.exe -ip <remote ip address> -port <remote port> -file <file path>  
optional: -h for help text

