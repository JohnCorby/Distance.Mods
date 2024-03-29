from gym import *
from gym.spaces import *
from socket import *
from typing import *
from struct import *

ADDR: tuple = ('localhost', 6969)


class MyEnv(Env):
    action_space: Space = None  # todo
    observation_space: Space = None  # todo

    def __init__(self) -> None:
        print('connecting...')
        self.listener: socket = socket(AF_INET, SOCK_STREAM, IPPROTO_TCP)
        self.listener.bind(ADDR)
        self.listener.listen()
        self.sock: socket = self.listener.accept()[0]
        print('connected!')

    def send(self, data: bytes) -> None:
        self.sock.send(data)

    def recv(self, size: int) -> bytes:
        data: bytes = self.sock.recv(size)
        assert data, 'socked closed'
        return data

    def step(self, action: object) -> Tuple[object, float, bool, dict]:
        print('step')
        print(f'action: {action}')
        self.send(pack('=?', True))
        # todo send actions
        # self.send(bytes(action))

        # get all the shit back
        # observation: List[Tuple[float, int]] = []
        # for i in range(256 * 256):
        #     observation.append(unpack('=fI', self.recv(8)))
        observation: bytes = self.recv(256 * 256 * calcsize('=fI'))
        reward: float = unpack('=f', self.recv(4))[0]
        done: bool = unpack('=?', self.recv(1))[0]
        # print(f'observation: {observation}', f'reward: {reward}', f'done: {done}', sep='\t')
        print(f'reward: {reward}', f'done: {done}', sep='\t')

        return observation, reward, done, {}

    def reset(self) -> object:
        print('reset')
        self.send(pack('=?', False))

        # reconnect since we close on level restart
        self.sock: socket = self.listener.accept()[0]

        # initial observation
        observation: object = self.step(None)[0]
        print(f'initial observation: {observation}')
        return observation

    def render(self, mode: str = 'human') -> None:
        raise NotImplementedError


def main():
    env: MyEnv = MyEnv()
    while True:
        observation, reward, done, info = env.step(None)
        if done:
            break
            # env.reset()


if __name__ == '__main__':
    main()
