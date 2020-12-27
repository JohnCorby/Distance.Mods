import socket

if __name__ == '__main__':
    ADDR = ('localhost', 6969)
    sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM, socket.IPPROTO_UDP)
    sock.connect(ADDR)


    def send(data: bytes) -> None:
        print(f'sending {data}')
        size = sock.sendto(data, ADDR)
        print(f'sent {size} bytes')


    def recv(size: int) -> bytes:
        print(f'receiving {size} bytes')
        data = sock.recvfrom(size)[0]
        print(f'received {data}')
        return data


    print("handshake...")
    send(b'ping')
    print('got response', recv(len(b'pong')))
    print("connected!")

    while True: pass
