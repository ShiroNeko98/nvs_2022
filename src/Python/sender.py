# ----- sender.py ------

# usage python sender.py localhost test.txt
import os
from socket import *
import sys
import hashlib

s = socket(AF_INET, SOCK_DGRAM)
host = sys.argv[1]
port = 11000
buf = 1024
sequenznummer = 0

addr = (host, port)

path = sys.argv[2]
size = os.path.getsize(path)
gesamtanzahl = int(size / buf) + 1

file_name = sys.argv[2].encode('utf-8')

# ----Startpaket senden-----

# s.sendto(file_name,addr)
startpaket = str(sequenznummer).encode('utf-8') + b'\u0000' + str(gesamtanzahl).encode('utf-8') + b'\u0000' + file_name
s.sendto(file_name, addr)

print("Sending startpacket....")
s.sendto(startpaket, addr)
sequenznummer = sequenznummer + 1

# ----Open file for md5 hash-----
f = open(sys.argv[2], "rb")
md5_data = f.read(size)
result = hashlib.md5(md5_data).hexdigest()
f.close()

# ----Open file for transmission-----
##print(result)
f = open(sys.argv[2], "rb")

data = f.read(buf)
packet = str(sequenznummer).encode('utf-8') + b'\u0000' + data

while (data):
    print("Sending....")
    print(packet)
    if (s.sendto(packet, addr)):
        data = f.read(buf)
        if (data):
            sequenznummer = sequenznummer + 1
            packet = str(sequenznummer).encode('utf-8') + b'\u0000' + data


endpacket = b'-1' + b'\u0000' + result.encode('utf-8')
print("Sending endpacket....")
s.sendto(endpacket, addr)

s.close()
f.close()
