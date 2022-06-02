# ----- receiver.py -----
#usage  python receiver.py file/path
import datetime
import time
from socket import *

#socket settings
host='0.0.0.0'
#port = int(sys.argv[1])
port = 11000
s = socket(AF_INET,SOCK_DGRAM)
s.bind((host,port))
addr = (host,port)
port_send = 12000
buf=5000
nummer = 0

t = socket(AF_INET, SOCK_DGRAM)
a = bytearray(b'')
#receiving startpacket
data,addr = s.recvfrom(buf)
addr2 = (addr[0], port_send)
t.sendto(b'ACK',addr2)
temp_data = data

#split startpacket to get the filename and the number of total packets
temp_data = temp_data.split(b'\x00')
complete =int(temp_data[3].decode("utf-8"))
if complete == 0:#if the file occuopies 1 packagage we initialize at 1
    complete = 1

file_size = int(temp_data[2].decode("utf-8"))
print("Received File:",temp_data[1])
f = open(temp_data[1],'wb')

#-----------First package------------------
tic = time.perf_counter()
data,addr = s.recvfrom(buf)
nummer+=1
t.sendto(b'ACK', addr2)
try:
    while (data):
        a.extend(data)
        s.settimeout(1)
        data, addr = s.recvfrom(buf)
        t.sendto(b'ACK', addr2)
        nummer+=1
except timeout:
    toc = time.perf_counter()
    f.write(a)
    f.close()
    s.close()
    t.close()
    print("File Downloaded")
    print("Number of packages received: " + str(nummer))

#transmission ip&port, received x/y packets, zeit, localtime
time = toc - tic - 1
temp_size = file_size/1000000
speed = temp_size/time
l = open("log.txt",'a')
l.write("Transmission ip: " + host + "\nTransmission port: " + str(port) + "\nPackets received " + str(nummer) + "/" + str(complete)
        + "\nLocaltime: " + str(datetime.datetime.now()) + "\nTime needed for transmission: " + str(time) + "\nSpeed : " + str(speed) + "mbps" + "\n--------------------------------------------\n")
l.close()