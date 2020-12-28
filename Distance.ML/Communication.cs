using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using static Distance.ML.Entry;

namespace Distance.ML {
    /// reading/writing data via ipc (so loopback and port)
    public class Communication : MonoBehaviour {
        private static readonly IPEndPoint ADDR = new(IPAddress.Loopback, 6969);
        private static readonly Socket SOCK = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        private enum Packet : byte {
            STEP,
            RESET,
        }

        private void Awake() {
            gameObject.AddComponent<MyCamera>();
            gameObject.AddComponent<MyState>();

            LOG.Info("connecting...");
            try {
                SOCK.Connect(ADDR);
                LOG.Info("connected!");
            } catch (SocketException e) {
                LOG.Error($"error connecting: {e}");
                this.Destroy();
            }
        }

        private void OnDestroy() {
            SOCK.Close();
        }

        private void LateUpdate() {
            var data = Utils.PlayerDataLocal!;
            if (data.CarLogic_.)
            if (!data.IsCarRelevant_) return;

            var packet = (Packet) Recv(1)[0];
            switch (packet) {
                case Packet.STEP:
                    // todo
                    break;
                case Packet.RESET:
                    // todo
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static void Send(byte[] data) {
            LOG.Debug($"sending {data.Decode()}");
            var size = SOCK.Send(data);
            LOG.Debug($"sent {size} bytes");
        }

        private static byte[] Recv(int size) {
            LOG.Debug($"receiving {size} bytes");
            var data = new byte[size];
            SOCK.Receive(data);
            LOG.Debug($"received {data.Decode()}");
            return data;
        }

        private static void SendObservation() {
            var textureData = GetTextureData();
            Send(textureData);
        }

        /// get byte data from texture
        private static byte[] GetTextureData() {
            var lastActive = RenderTexture.active;
            RenderTexture.active = MyCamera.RENDER_TEXTURE;
            TEXTURE.ReadPixels(new Rect(0, 0, TEXTURE.width, TEXTURE.height), 0, 0);
            RenderTexture.active = lastActive;

            LOG.Info(TEXTURE.GetPixel(TEXTURE.width / 2, TEXTURE.height / 2));

            return new byte[0];
        }
    }
}
