from gym import *
from socket import *
from enum import *
from typing import *

ADDR: tuple = ('localhost', 6969)


class Packet(Enum):
    STEP = b'\x00'
    RESET = b'\x01'


class Env(Env):
    action_space: Space = None  # todo
    observation_space: Space = None  # todo

    def __init__(self) -> None:
        print('connecting...')
        listener: socket = socket(AF_INET, SOCK_STREAM, IPPROTO_TCP)
        listener.bind(ADDR)
        listener.listen()
        self.sock: socket = listener.accept()
        print('connected!')

    def send(self, data: bytes) -> None:
        print(f'sending {data}')
        size = self.sock.send(data)
        print(f'sent {size} bytes')

    def recv(self, size: int) -> bytes:
        print(f'receiving {size} bytes')
        data = self.sock.recv(size)
        print(f'received {data}')
        return data

    def step(self, action: object) -> Tuple[object, float, bool, dict]:
        # do step with actions todo
        self.send(Packet.STEP.value)
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
        self.send(Packet.RESET.value)

        # todo
        observation: object = object(self.recv(1024))
        return observation

    def render(self, mode: str = 'human') -> None:
        raise NotImplementedError


if __name__ == '__main__':
    env = Env()
    while True:
        env.step(None)
