from gym import *
from gym.spaces import *
from socket import *
from enum import *
from typing import *

ADDR: tuple = ('localhost', 6969)


class Packet(Enum):
    STEP = b'\00'
    RESET = b'\01'


class Env(Env):
    action_space: Space = None  # todo
    observation_space: Space = None  # todo

    def __init__(self) -> None:
        print('connecting...')
        self.listener: socket = socket(AF_INET, SOCK_STREAM, IPPROTO_TCP)
        self.listener.bind(ADDR)
        self.listener.listen()
        self.sock: socket = self.listener.accept()
        print('connected!')

    def send(self, data: bytes) -> None:
        self.sock.send(data)

    def recv(self, size: int) -> bytes:
        return self.sock.recv(size)

    def step(self, action: object) -> Tuple[object, float, bool, dict]:
        # todo do step with actions
        self.send(Packet.STEP.value)
        self.send(bytes(action))

        # todo get all the shit back
        observation: object = object(self.recv(1024))
        reward: float = self.recv(4)
        done: bool = False if self.recv(1) == b'\x00' else True
        info: dict = {}

        if done:
            self.reset()  # todo this will make the level loop, so maybe remove this later

        return observation, reward, done, info

    def reset(self) -> object:
        self.send(Packet.RESET.value)

        # reconnect since we close on level restart
        self.sock: socket = self.listener.accept()

        # todo initial observation
        observation, _, _, _ = self.step(None)
        return observation

    def render(self, mode: str = 'human') -> None:
        raise NotImplementedError


if __name__ == '__main__':
    env: Env = Env()
    while True: pass
