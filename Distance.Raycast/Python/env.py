import socket

import gym


class Env(gym.Env):
    ADDR: tuple = ('localhost', 6969)
    PACKET_STEP: bytes = b'\x00'
    PACKET_RESET: bytes = b'\x01'

    action_space: gym.Space = None  # todo
    observation_space: gym.Space = None  # todo

    def __init__(self) -> None:
        self.sock: socket.socket = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
        self.sock.bind(self.ADDR)

        # handshake
        # the contents dont matter. it just needs to be length 1
        self.send(b'0')
        self.recv(1)

    def send(self, data: bytes) -> int:
        return self.sock.sendto(data, self.ADDR)

    def recv(self, size: int) -> bytes:
        return self.sock.recvfrom(size)[0]

    def step(self, action: object) -> tuple:
        # do step with actions todo
        self.send(self.PACKET_STEP)
        self.send(bytes(action))

        # get all the shit back todo
        observation: object = object(self.recv(1024))
        reward: float = self.recv(4)
        done: bool = False if self.recv(1) == b'\x00' else True
        info: dict = {}

        if done:
            self.reset()  # fixme this will make the level loop, so maybe remove this later

        return observation, reward, done, info

    def reset(self) -> object:
        self.send(self.PACKET_RESET)

        # todo
        observation: object = object(self.recv(1024))
        return observation

    def render(self, mode: str = 'human') -> None:
        pass
