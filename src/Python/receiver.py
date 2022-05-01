# ----- receiver.py -----

#usage  python receiver.py file/path

import hashlib
import datetime
import time
from socket import *

host="0.0.0.0"
port = 11000
s = socket(AF_INET,SOCK_DGRAM)
s.bind((host,port))
nummer = 0

addr = (host,port)
buf=2000
#file_name,addr = s.recvfrom(buf)

data,addr = s.recvfrom(buf)
while(data[0] == 48):
    tic = time.perf_counter()
    temp_data = data
    data, addr = s.recvfrom(buf)



temp_data = temp_data.split(b'\x00')
complete = data[1]
if complete == 0:
    complete = 1
#print("startpacket:" + str(startpacket))

print("Received File:",temp_data[2])
f = open(temp_data[2],'wb')
data = data.split(b'\x00')
#-----------First package------------------

f.write(data[1])
nummer+=1
try:
    while (data):
        s.settimeout(1)
        data, addr = s.recvfrom(buf)
        data = data.split(b'\x00')
        if(data[0] != b'-1'):
            nummer+=1
            f.write(data[1])
except timeout:
    f.close()
    s.close()
    toc = time.perf_counter()
    print("File Downloaded")
    print("Number of packages received: " + str(nummer))

f = open(temp_data[2], "rb")
md5_data = f.read()
result = hashlib.md5(md5_data).hexdigest()
f.close()

#transmission ip&port, received x/y packets, zeit, localtime
l = open("log.txt",'a')
l.write("Transmission ip: " + host + "\nTransmission port: " + str(port) + "\nPackets received " + str(nummer) + "/" + str(complete)
        + "\nLocaltime: " + str(datetime.datetime.now()) + "\nTime needed for transmission: " + str(toc - tic) + "\n--------------------------------------------\n")
l.close()



print("calculated hash: " + str(result))
print("received hash data: " + str(data[1]))

if data == bytes(result, 'ascii'):
    print("packet richtig bekommen!!!")
