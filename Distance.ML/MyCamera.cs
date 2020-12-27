using System.Collections;
using System.Reflection;
using Reactor.API.Storage;
using UnityEngine;

namespace Distance.ML {
    public class MyCamera : MonoBehaviour {
        public static readonly RenderTexture RENDER_TEXTURE = new(256, 256, 0);
        private static Camera Camera = null!;
        private static readonly Material STANDARD_MATERIAL;
        private static readonly Material PP_MATERIAL;
        private static readonly int CAR_LAYER_MASK;

        static MyCamera() {
            var assetBundle = (AssetBundle) new Assets("assets").Bundle;
            STANDARD_MATERIAL = assetBundle.LoadAsset<Material>("Standard.mat");
            PP_MATERIAL = assetBundle.LoadAsset<Material>("PostProcessing.mat");

            CAR_LAYER_MASK = (int) typeof(PhysicsEx)
                .GetField("carLayerMask_", BindingFlags.Static | BindingFlags.NonPublic)?
                .GetValue(null)!;
        }

        private IEnumerator Start() {
            Camera = gameObject.AddComponent<Camera>();
            Camera.main.CopyFrom(Camera);
            Camera.cullingMask = Camera.main.cullingMask & ~CAR_LAYER_MASK;
            Camera.targetTexture = RENDER_TEXTURE;

            yield return new WaitForSeconds(3);
            ReplaceMaterials();
        }

        /// replace all materials in scene with standard one
        /// makes post processing work for transparent stuff
        private static void ReplaceMaterials() {
            foreach (var renderer in FindObjectsOfType<Renderer>()) {
                var materials = new Material[renderer.materials.Length];
                for (var i = 0; i < materials.Length; i++)
                    materials[i] = STANDARD_MATERIAL;
                renderer.materials = materials;
            }
        }

        // /// apply post processing
        // private void OnRenderImage(RenderTexture src, RenderTexture dest) {
        //     Graphics.Blit(src, dest, PP_MATERIAL);
        // }

        /// draw texture to screen
        private void OnGUI() {
            GUI.DrawTexture(new Rect(0, 0, RENDER_TEXTURE.width, RENDER_TEXTURE.height), RENDER_TEXTURE);
        }
    }
}
