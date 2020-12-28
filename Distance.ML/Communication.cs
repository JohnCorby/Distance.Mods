using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using static Distance.ML.Entry;

namespace Distance.ML {
    /// reading/writing data via ipc (so loopback and port)
    public class Communication : MonoBehaviour {
        private static readonly IPEndPoint ADDR = new(IPAddress.Loopback, 6969);
        private static Socket Sock = null!;

        private enum Packet : byte {
            STEP,
            RESET,
        }

        private void Awake() {
            if (!G.Sys.GameManager_.SoloAndNotOnline_) throw new ArgumentException("must be in solo game");

            gameObject.AddComponent<MyCamera>();
            gameObject.AddComponent<MyState>();

            LOG.Info("connecting...");
            Sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try {
                Sock.Connect(ADDR);
                LOG.Info("connected!");
            } catch (SocketException e) {
                LOG.Error($"error connecting: {e}");
                this.Destroy();
            }
        }

        private void OnDestroy() {
            Sock.Close();
            GetComponent<MyCamera>().Destroy();
            GetComponent<MyState>().Destroy();
            LOG.Debug("communication destroyed");
        }

        /// process packet and perform step
        private void LateUpdate() {
            var data = Utils.PlayerDataLocal!;
            if (!data.CarInputEnabled_ || data.CarLogic_.IsDying_) return;

            var packet = (Packet) Recv(1)[0];
            switch (packet) {
                case Packet.STEP:
                    LOG.Debug("step");
                    // todo get action
                    MyState.UpdateState();

                    LOG.Debug($"reward: {MyState.Reward}\tdone: {MyState.Done}");
                    // send results
                    // todo observation
                    Send(BitConverter.GetBytes(MyState.Reward));
                    Send(BitConverter.GetBytes(MyState.Done));
                    break;
                case Packet.RESET:
                    LOG.Debug("step");
                    G.Sys.GameManager_.RestartLevel();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            MyState.ResetState();
        }

        private void Send(byte[] data) {
            try {
                Sock.Send(data);
            } catch (SocketException e) {
                LOG.Error($"send error: {e}");
                this.Destroy();
            }
        }

        private byte[] Recv(int size) {
            try {
                var data = new byte[size];
                Sock.Receive(data);
                return data;
            } catch (SocketException e) {
                LOG.Error($"recv error: {e}");
                this.Destroy();
                return null!;
            }
        }
    }
}
