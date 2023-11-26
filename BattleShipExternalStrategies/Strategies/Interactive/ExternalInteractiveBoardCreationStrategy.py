import socket

# Change if needed.
HOST = "127.0.1.1"
PORT = 65431

s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
s.connect((HOST, PORT))

while True:
    data = str(s.recv(1024)).replace("<EOF>", "")
    print(data)
    if data.find("Turn off") > -1:
        s.close()
        break
    if data.find("Send") > -1:
        places = ""
        while True:
            print("Enter new place as X,Y\nWrite \"end\" to end.")
            new_place = input()
            if new_place == "end":
                break
            if len(places) > 0:
                places += "+"
            places += new_place
        s.sendall(bytes(places + "<EOF>", "UTF-8"))

