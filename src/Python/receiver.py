# ----- receiver.py -----

#usage  python receiver.py file/path


from socket import *
import sys
import select

host="0.0.0.0"
port = 11000
s = socket(AF_INET,SOCK_DGRAM)
s.bind((host,port))

addr = (host,port)
buf=2000

startpacket,addr = s.recvfrom(buf)
print("startpacket:" + str(startpacket))
data,addr = s.recvfrom(buf)
print("Received File:",data.strip(b'1'))
f = open(sys.argv[1],'wb')



try:
    while(data.decode('utf-8')):
        f.write(data)
        s.settimeout(2)
        data,addr = s.recvfrom(buf)
except timeout:
    f.close()
    s.close()
    print("endpacket:" + str(data))
    print("File Downloaded")
