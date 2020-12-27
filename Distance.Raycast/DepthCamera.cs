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
            Camera.depthTextureMode = DepthTextureMode.Depth;
            Camera.targetTexture = RENDER_TEXTURE;
        }

        // /// replace all materials
        // private void Start() {
        //     var newMaterial = new Material(Shader.Find("Standard"));
        //     foreach (Renderer renderer in FindObjectsOfType<Renderer>()) {
        //         var newMaterials = new Material[renderer.materials.Length];
        //         for (var i = 0; i < newMaterials.Length; i++)
        //             newMaterials[i] = newMaterial;
        //         renderer.materials = newMaterials;
        //     }
        // }

        /// apply shader to texture
        private void OnRenderImage(RenderTexture src, RenderTexture dest) {
            Graphics.Blit(src, dest, DEPTH_MATERIAL);
        }

        /// draw texture to screen
        private void OnGUI() {
            GUI.DrawTexture(new Rect(0, 0, RENDER_TEXTURE.width, RENDER_TEXTURE.height), RENDER_TEXTURE);
        }
    }
}
