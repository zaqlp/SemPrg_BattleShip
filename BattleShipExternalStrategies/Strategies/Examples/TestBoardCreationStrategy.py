import socket

# Change if needed.
HOST = "127.0.1.1"
PORT = 65431

s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
s.connect((HOST, PORT))
while True:
    data = str(s.recv(1024))
    print(data)
    if data.find("Turn off") > -1:
        s.close()
        break
    if data.find("Send") > -1:
        s.sendall(bytes("0,0+0,2+0,4+0,6+0,8+0,9+2,0+2,1+2,3+2,4+2,6+2,7+2,8+4,0+4,1+4,2+4,4+4,5+4,6+4,7<EOF>", "UTF-8"))

