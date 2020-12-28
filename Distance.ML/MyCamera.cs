using Reactor.API.Storage;
using UnityEngine;

namespace Distance.ML {
    public class MyCamera : MonoBehaviour {
        public static readonly RenderTexture RENDER_TEXTURE = new(256, 256, 0);
        private static Camera Camera = null!;
        private static readonly Shader STANDARD_SHADER, PP_SHADER;

        private enum Id : int {
            NORMAL,
            KILL_GRID,
            END,
            CHECKPOINT,
            COOLDOWN,
        }

        static MyCamera() {
            var assetBundle = (AssetBundle) new Assets("assets").Bundle;
            STANDARD_SHADER = assetBundle.LoadAsset<Shader>("Standard.shader");
            PP_SHADER = assetBundle.LoadAsset<Shader>("PostProcessing.shader");
        }

        private void Awake() {
            Camera = gameObject.AddComponent<Camera>();
            Camera.cullingMask = Camera.main.cullingMask & ~PhysicsEx.carLayerMask_;
            Camera.targetTexture = RENDER_TEXTURE;
        }

        private void Start() {
            PreprocessScene();
        }

        /// modify the scene so that it will make sense to
        private static void PreprocessScene() {
            static void Init(Renderer renderer, Id id) {
                foreach (var material in renderer.materials) {
                    material.shader = STANDARD_SHADER;
                    material.SetInt("_ID", (int) id);
                }
            }

            foreach (var renderer in Resources.FindObjectsOfTypeAll<Renderer>()) {
                Id id;
                if (renderer.HasComponentInChildren<KillGrid>() ||
                    renderer.HasComponentInChildren<KillGridBox>()) id = Id.KILL_GRID;
                else if (renderer.HasComponentInChildren<RaceEndLogic>()) id = Id.END;
                else if (renderer.HasComponentInChildren<CheckpointLogic>()) id = Id.CHECKPOINT;
                else if (renderer.HasComponentInChildren<TriggerCooldownLogic>()) id = Id.COOLDOWN;
                else id = Id.NORMAL;

                Init(renderer, id);
            }
        }

        /// update position based on car
        private void Update() {
            var parentTransform = Utils.PlayerDataLocal!.Car_.transform;
            transform.position = parentTransform.position + parentTransform.up;
            transform.rotation = parentTransform.rotation;
        }

        // /// apply post processing
        // private void OnRenderImage(RenderTexture src, RenderTexture dest) {
        //     Graphics.Blit(src, dest, PP_MATERIAL);
        // }

        /// draw texture to screen
        private void OnGUI() {
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), RENDER_TEXTURE);
        }
    }
}
