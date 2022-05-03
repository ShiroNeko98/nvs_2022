# ----- receiver.py -----
#usage  python receiver.py file/path
import hashlib
import datetime
import time
from socket import *

#socket settings
host="0.0.0.0"
port = 11000
s = socket(AF_INET,SOCK_DGRAM)
s.bind((host,port))
addr = (host,port)
buf=4000
nummer = 0

#receiving startpacket
data,addr = s.recvfrom(buf)
while(data[0] == 48):
    tic = time.perf_counter()
    temp_data = data
    data, addr = s.recvfrom(buf)

#split startpacket to get the filename and the number of total packets
temp_data = temp_data.split(b'\x00')
complete =int(temp_data[1].decode("utf-8"))
if complete == 0:#if the file occuopies 1 packagage we initialize at 1
    complete = 1

print("Received File:",temp_data[2])
f = open(temp_data[2],'wb')

#-----------First package------------------
data = data.split(b'\x00')
temp_ir = list(data[1].removeprefix(b'[').removesuffix(b']').split(b','))
nummer+=1
#wile loop to iterate between the packages
try:
    while (data):
        s.settimeout(2)
        data, addr = s.recvfrom(buf)
        data = data.split(b'\x00')
        if(data[0] != b'-1'):
            nummer+=1
            temp_ir.extend(data[1].removeprefix(b'[').removesuffix(b']').split(b','))
except timeout:
    byte_arr = []
    some_bytes = bytearray(byte_arr)
    while temp_ir[-1] == b'0':
        temp_ir.pop()
    for i in temp_ir:
        k = int(i.decode("utf-8"))
        some_bytes.append(k)
    f.write(some_bytes)
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
