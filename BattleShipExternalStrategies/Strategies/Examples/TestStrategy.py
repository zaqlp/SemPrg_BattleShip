import socket

# Change if needed.
HOST = "127.0.1.1"
PORT = 65432

s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
s.connect((HOST, PORT))
i = 0
j = 0
while True:
    data = str(s.recv(1024))
    print(data)
    if data.find("Turn off") > -1:
        s.close()
        break
    if data.find("move") > -1:
        print(str(i) + "," + str(j))
        s.sendall(bytes(str(i) + "," + str(j) + "<EOF>", "UTF-8"))
        if i == 9:
            if j == 9:
                j = 0
            else:
                j += 1
            i = 0
        else:
            i += 1

