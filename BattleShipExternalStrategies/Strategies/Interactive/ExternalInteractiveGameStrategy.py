import socket

# Change if needed.
HOST = "127.0.1.1"
PORT = 65432

s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
s.connect((HOST, PORT))

while True:
    data = str(s.recv(1024)).replace("<EOF>", "")
    print(data)
    if data.find("Turn off") > -1:
        s.close()
        break
    if data.find("move") > -1:
        move = input()
        s.sendall(bytes(move + "<EOF>", "UTF-8"))

