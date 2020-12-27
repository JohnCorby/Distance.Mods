using Reactor.API.Storage;
using UnityEngine;

namespace Distance.Raycast {
    public class DepthCamera : MonoBehaviour {
        public static readonly RenderTexture RENDER_TEXTURE = new(256, 256, 0);
        private static Camera Camera = null!;
        private static readonly Material DEPTH_MATERIAL;

        static DepthCamera() {
            var assetBundle = (AssetBundle) new Assets("assets").Bundle;
            DEPTH_MATERIAL = assetBundle.LoadAsset<Material>("Depth.mat");
        }

        private void Awake() {
            Camera = gameObject.AddComponent<Camera>();
            Camera.cullingMask = PhysicsEx.NoCarsRayCastLayerMask_;
            Camera.targetTexture = RENDER_TEXTURE;

            ReplaceMaterials();
        }

        private static void ReplaceMaterials() {
            var material = DEPTH_MATERIAL;

            foreach (var renderer in FindObjectsOfType<Renderer>()) {
                var materials = new Material[renderer.materials.Length];
                for (var i = 0; i < materials.Length; i++) materials[i] = material;

                renderer.materials = materials;
            }
        }

        /// draw texture to screen
        private void OnGUI() {
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), RENDER_TEXTURE);
        }
    }
}
