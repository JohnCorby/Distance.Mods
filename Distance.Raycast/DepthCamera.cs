using Reactor.API.Storage;
using UnityEngine;

namespace Distance.Raycast {
    public class DepthCamera : MonoBehaviour {
        private RenderTexture RenderTexture;
        private Camera Camera;

        private static readonly Material DEPTH_MATERIAL;

        static DepthCamera() {
            var assetBundle = (AssetBundle) new Assets("assets").Bundle;
            DEPTH_MATERIAL = assetBundle.LoadAsset<Material>("Depth.mat");
        }

        private void Awake() {
            RenderTexture = new RenderTexture(256, 256, 0, RenderTextureFormat.RHalf);

            Camera = gameObject.AddComponent<Camera>();
            Camera.cullingMask = PhysicsEx.NoCarsRayCastLayerMask_;
            Camera.depthTextureMode = DepthTextureMode.Depth;
            Camera.targetTexture = RenderTexture;
        }

        // /// apply shader to texture
        private void OnRenderImage(RenderTexture src, RenderTexture dest) {
            Graphics.Blit(src, dest, DEPTH_MATERIAL);
        }

        /// draw texture to screen
        private void OnGUI() {
            GUI.DrawTexture(new Rect(0, 0, RenderTexture.width, RenderTexture.height), RenderTexture);
        }
    }
}
