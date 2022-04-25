# ----- receiver.py -----

#!/usr/bin/env python

from socket import *
import sys
import select

host="127.0.0.1"
port = 11000
s = socket(AF_INET,SOCK_DGRAM)
s.bind((host,port))

addr = (host,port)
buf=2000

startpacket,addr = s.recvfrom(buf)
data,addr = s.recvfrom(buf)
print("Received File:",data.strip())
f = open('new file.txt','wb')

data,addr = s.recvfrom(buf)
try:
    while(data.decode('utf-8')):
        f.write(data)
        s.settimeout(2)
        data,addr = s.recvfrom(buf)
except timeout:
    f.close()
    s.close()
    print("File Downloaded")
