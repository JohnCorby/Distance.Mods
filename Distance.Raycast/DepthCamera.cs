using System;
using Reactor.API.Storage;
using UnityEngine;

namespace Distance.Raycast {
    public class DepthCamera : MonoBehaviour {
        public static readonly RenderTexture RENDER_TEXTURE = new(256, 256, 0);
        private static Camera Camera = null!;
        private static readonly Material MATERIAL, PP_MATERIAL;

        static DepthCamera() {
            var assetBundle = (AssetBundle) new Assets("assets").Bundle;
            MATERIAL = new Material(Shader.Find("Standard"));
            PP_MATERIAL = assetBundle.LoadAsset<Material>("PostProcessing.mat");
        }

        private void Awake() {
            Camera = gameObject.AddComponent<Camera>();
            Camera.cullingMask = PhysicsEx.NoCarsRayCastLayerMask_;
            Camera.targetTexture = RENDER_TEXTURE;

            ReplaceMaterials();
        }

        /// replace all materials in scene with standard one
        /// makes post processing work for transparent stuff
        private static void ReplaceMaterials() {
            foreach (var renderer in FindObjectsOfType<Renderer>()) {
                var materials = new Material[renderer.materials.Length];
                for (var i = 0; i < materials.Length; i++)
                    materials[i] = MATERIAL;
                renderer.materials = materials;
            }
        }

        /// apply post processing
        private void OnRenderImage(RenderTexture src, RenderTexture dest) {
            Graphics.Blit(src, dest, PP_MATERIAL);
        }

        /// draw texture to screen
        private void OnGUI() {
            GUI.DrawTexture(new Rect(0, 0, RENDER_TEXTURE.width, RENDER_TEXTURE.height), RENDER_TEXTURE);
        }
    }
}
