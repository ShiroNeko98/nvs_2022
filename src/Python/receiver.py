# ----- receiver.py -----

#usage  python receiver.py file/path

import hashlib
from socket import *
import sys
import select

host="0.0.0.0"
port = 11000
s = socket(AF_INET,SOCK_DGRAM)
s.bind((host,port))
nummer = 0

addr = (host,port)
buf=2000
file_name,addr = s.recvfrom(buf)
startpacket,addr = s.recvfrom(buf)
#print("startpacket:" + str(startpacket))

print("Received File:",file_name)
f = open(sys.argv[1],'wb')

#-----------First package------------------
data,addr = s.recvfrom(buf)
f.write(data[data.find(b"\u0000")+1:].removeprefix(b'u0000'))

try:
    while(data):
        s.settimeout(2)
        data, addr = s.recvfrom(buf)
        nummer+=1
        if(data[0] != 45):  #just checks if first bytearray is -1, aka endpacket
            f.write(data[data.find(b"\u0000")+1:].removeprefix(b'u0000'))
except timeout:
    f.close()
    s.close()
    #print("endpacket:" + str(data))
    print("File Downloaded")
    print("Number of packages received: " + str(nummer))

f = open(sys.argv[1], "rb")
md5_data = f.read()
result = hashlib.md5(md5_data).hexdigest()
f.close()


data = data[data.find(b"\u0000")+1:].removeprefix(b'u0000')

if data == bytes(result, 'ascii'):
    print("packet richtig bekommen!!!")
