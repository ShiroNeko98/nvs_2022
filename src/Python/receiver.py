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
#file_name,addr = s.recvfrom(buf)

data,addr = s.recvfrom(buf)
while(data[0] == 48):
    cazzo = data
    data, addr = s.recvfrom(buf)



cazzo = cazzo.split(b'\x00')

#print("startpacket:" + str(startpacket))

print("Received File:",cazzo[2])
f = open(cazzo[2],'wb')
data = data.split(b'\x00')
#-----------First package------------------

f.write(data[1])
nummer+=1
try:
    while (data):
        s.settimeout(2)
        data, addr = s.recvfrom(buf)
        data = data.split(b'\x00')
        if(data[0] != b'-1'):
            nummer+=1
            f.write(data[1])
except timeout:
    f.close()
    s.close()
    #print("endpacket:" + str(data))
    print("File Downloaded")
    print("Number of packages received: " + str(nummer))

f = open(cazzo[2], "rb")
md5_data = f.read()
result = hashlib.md5(md5_data).hexdigest()
f.close()



print("calculated hash: " + str(result))
print("received hash data: " + str(data[1]))

if data == bytes(result, 'ascii'):
    print("packet richtig bekommen!!!")
