using System.Net;
using System.Net.Sockets;
using JetBrains.Annotations;
using UnityEngine;
using static Distance.Raycast.Entry;

namespace Distance.Raycast {
    /// reading/writing data via ipc (so loopback and port)
    public class Communication : MonoBehaviour {
        private const int PORT = 6969;

        private static readonly UdpClient CLIENT = new();

        static Communication() {
            CLIENT.Connect(new IPEndPoint(IPAddress.Loopback, PORT));

            return;
            // handshake
            CLIENT.Send(new byte[] {0xff}, 1);
            IPEndPoint remoteEp = null;
            CLIENT.Receive(ref remoteEp);
        }

        private void LateUpdate() {
            var renderTexture = DepthCamera.RenderTexture;
            if (!renderTexture) return;

            SendObservation(renderTexture);
        }

        private static void SendObservation([NotNull] RenderTexture renderTexture) {
            var textureData = GetTextureData(renderTexture);
            CLIENT.Send(textureData, textureData.Length);
        }

        /// get byte data from texture
        private static byte[] GetTextureData([NotNull] RenderTexture renderTexture) {
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
