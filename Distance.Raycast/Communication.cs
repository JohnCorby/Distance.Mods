using System.Net;
using System.Net.Sockets;
using UnityEngine;

namespace Distance.Raycast {
    /// reading/writing data via ipc (so loopback and port)
    public class Communication : MonoBehaviour {
        private const int PORT = 6969;

        private static readonly UdpClient CLIENT;

        static Communication() {
            CLIENT = new UdpClient(new IPEndPoint(IPAddress.Loopback, PORT));
        }

        private static void SendObservation(RenderTexture renderTexture) {
            var texture = CopyToCpuTexture(renderTexture);
            // texture.GetRawTextureData()

            // todo
        }

        /// get cpu-readable Texture2D from RenderTexture
        private static Texture2D CopyToCpuTexture(RenderTexture renderTexture) {
            var lastActive = RenderTexture.active;
            RenderTexture.active = renderTexture;

            var texture2D = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RHalf);
            texture2D.ReadPixels(new Rect(0, 0, texture2D.width, texture2D.height), 0, 0);

            RenderTexture.active = lastActive;
            return texture2D;
        }
    }
}
