using System;
using Reactor.API.Storage;
using UnityEngine;

namespace Distance.ML {
    public class MyCamera : MonoBehaviour {
        /// internal camera render texture
        public static readonly RenderTexture RENDER_TEXTURE = new(256, 256,
            0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
        private static Camera Camera = null!;
        private static readonly Shader STANDARD_SHADER, INVISIBLE_SHADER;

        private enum ID {
            NORMAL,
            KILL_GRID,
            END,
            CHECKPOINT,
            COOLDOWN,
            TELEPORTER,
        }

        public static readonly int NUM_IDS = Enum.GetNames(typeof(ID)).Length;

        static MyCamera() {
            var assetBundle = (AssetBundle) new Assets("assets").Bundle;
            STANDARD_SHADER = assetBundle.LoadAsset<Shader>("Standard.shader");
            INVISIBLE_SHADER = assetBundle.LoadAsset<Shader>("Invisible.shader");
        }

        private void Awake() {
            Camera = gameObject.AddComponent<Camera>();

            Camera.cullingMask = Camera.main.cullingMask & ~PhysicsEx.carLayerMask_;

            Camera.clearFlags = CameraClearFlags.SolidColor;
            Camera.backgroundColor = Color.black;

            Camera.targetTexture = RENDER_TEXTURE;
        }

        private void Start() {
            PreprocessScene();
        }

        /// modify the scene so that it will make sense to the agent
        private static void PreprocessScene() {
            static void Init(Renderer renderer, ID id) {
                foreach (var material in renderer.materials) {
                    material.shader = STANDARD_SHADER;
                    material.SetInt("_ID", (int) id);
                    material.SetInt("_NumIDs", NUM_IDS);
                }
            }

            static void Invisible(Renderer renderer) {
                foreach (var material in renderer.materials) {
                    material.shader = INVISIBLE_SHADER;
                }
            }

            foreach (var renderer in Resources.FindObjectsOfTypeAll<Renderer>()) {
                // if (!renderer.HasAnyComponent(typeof(Collider))) {
                    // Invisible(renderer);
                    // continue;
                // }

                ID id;
                if (renderer.HasAnyComponent(typeof(KillGrid), typeof(KillGridBox), typeof(KillGridFollower)))
                    id = ID.KILL_GRID;
                else if (renderer.HasAnyComponent(typeof(RaceEndLogic))) id = ID.END;
                else if (renderer.HasAnyComponent(typeof(CheckpointLogicBase))) id = ID.CHECKPOINT;
                else if (renderer.HasAnyComponent(typeof(TriggerCooldownLogic))) id = ID.COOLDOWN;
                else if (renderer.HasAnyComponent(typeof(TeleporterEntrance), typeof(TeleporterExit))) id = ID.TELEPORTER;
                else id = ID.NORMAL;

                Init(renderer, id);
            }
        }

        /// update position based on car
        private void Update() {
            var parentTransform = Utils.PlayerDataLocal!.Car_.transform;
            transform.position = parentTransform.position + parentTransform.up;
            transform.rotation = parentTransform.rotation;
        }

        /// draw texture to screen
        private void OnGUI() {
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), RENDER_TEXTURE);
        }
    }
}
