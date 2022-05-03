# ----- receiver.py -----

#usage  python receiver.py file/path

import hashlib
import codecs
import datetime
import struct
import time
from socket import *




host="0.0.0.0"
port = 11000
s = socket(AF_INET,SOCK_DGRAM)
s.bind((host,port))
nummer = 0

addr = (host,port)
buf=4000

data,addr = s.recvfrom(buf)
while(data[0] == 48):
    tic = time.perf_counter()
    temp_data = data
    data, addr = s.recvfrom(buf)

temp_data = temp_data.split(b'\x00')
complete = data[1]
if complete == 0:
    complete = 1

print("Received File:",temp_data[2])
f = open(temp_data[2],'wb')
data = data.split(b'\x00')
#-----------First package------------------

temp_ir = list(data[1].removeprefix(b'[').removesuffix(b']').split(b','))
#print(temp_ir)
#temp_ir = temp_ir[:-1]
#while temp_ir[-1] == b'0':
    #temp_ir.pop()
#print(temp_ir)
#print(temp_ir)
#for i in reversed(temp_ir):
    #k = i.decode("utf-8").removeprefix('[').removesuffix(']')
    #print(k)
    #f.write(chr(int(k)).encode('utf-8'))
    #f.write(chr(int(i.deleteprefix('['))))
nummer+=1

try:
    while (data):
        s.settimeout(2)
        data, addr = s.recvfrom(buf)
        data = data.split(b'\x00')
        if(data[0] != b'-1'):
            nummer+=1
            temp_ir.extend(data[1].removeprefix(b'[').removesuffix(b']').split(b','))
            #temp_ir = temp_ir[:-1]
            #while temp_ir[-1] == b'0':
                #temp_ir.pop()
            ##for i in reversed(temp_ir):
                ##k = i.decode("utf-8").removeprefix('[').removesuffix(']')
                # print(k)
                ##f.write(chr(int(k)).encode('utf-8'))
except timeout:
    #packet_bytes = temp_ir[0]
    byte_arr = []
    some_bytes = bytearray(byte_arr)
    while temp_ir[-1] == b'0':
        temp_ir.pop()
    for i in reversed(temp_ir):
        k = int(i.decode("utf-8"))
        some_bytes.append(k)
        #print(k)
    #f.write(temp_ir)
    f.write(some_bytes)
    ##for byte in reversed(temp_ir):
        #print(byte)
        #f.write(byte)
    #end_data = bytes(temp_ir)
    #print(temp_ir)
    #f.write(end_data)
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
