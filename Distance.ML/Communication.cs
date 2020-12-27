using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using static Distance.ML.Entry;

namespace Distance.ML {
    /// reading/writing data via ipc (so loopback and port)
    public class Communication : MonoBehaviour {
        private static readonly IPEndPoint ADDR = new(IPAddress.Loopback, 6969);
        private static readonly byte[] PACKET_STEP = {0};
        private static readonly byte[] PACKET_RESET = {1};

        private static readonly Socket SOCK = new(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        private void Awake() {
            // handshake
            LOG.Info("handshake...");
            Send("ping".Encode());
            LOG.Info("got response " + Recv("pong".Encode().Length).Decode());
            LOG.Info("connected!");
        }

        private void Update() {
            var packet = Recv(1);
            if (packet == PACKET_STEP) {
                Step();
            } else if (packet == PACKET_RESET) {
                Reset();
            }
        }

        private void Step() {
            // todo
        }

        private void Reset() {
            // todo
        }

        private static void Send(byte[] data) {
            LOG.Debug($"sending {data.Decode()}");
            var size = SOCK.SendTo(data, ADDR);
            LOG.Debug($"sent {size} bytes");
        }

        private static byte[] Recv(int size) {
            LOG.Debug($"receiving {size} bytes");
            var data = new byte[size];
            var remoteEp = (EndPoint) new IPEndPoint(IPAddress.Any, 0);
            SOCK.ReceiveFrom(data, ref remoteEp);
            LOG.Debug($"received {data.Decode()}");
            return data;
        }

        private static void SendObservation(RenderTexture renderTexture) {
            var textureData = GetTextureData(renderTexture);
            Send(textureData);
        }

        /// get byte data from texture
        private static byte[] GetTextureData(RenderTexture renderTexture) {
            var lastActive = RenderTexture.active;
            RenderTexture.active = renderTexture;
            var tex = new Texture2D(renderTexture.width, renderTexture.height);
            Destroy(tex); // this doesnt happen automatically for some reason
            tex.ReadPixels(new Rect(0, 0, tex.width, tex.height), 0, 0);
            RenderTexture.active = lastActive;

            LOG.Info(tex.GetPixel(tex.width / 2, tex.height / 2));

            return new byte[0];
        }
    }
}
