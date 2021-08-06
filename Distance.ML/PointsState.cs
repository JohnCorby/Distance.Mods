using System;
using Reactor.API.Storage;
using UnityEngine;
using static Distance.ML.Entry;

namespace Distance.ML {
    /// holds state for points data
    public class PointsState : MonoBehaviour {
        private Camera MainCamera = null!;
        private Camera Camera = null!;
        private RenderTexture Texture = null!;

        /// buffer that holds points data
        private ComputeBuffer Buffer = null!;
        /// array of points in byte form so we won't have to allocate/convert more
        public byte[] Bytes = null!;

        private enum ID : uint {
            NORMAL,
            KILL_GRID,
            END,
            CHECKPOINT,
            COOLDOWN,
            TELEPORTER,
        }

        private static readonly uint NUM_IDS = (uint)Enum.GetNames(typeof(ID)).Length;

        private static readonly Shader STANDARD_SHADER, INVISIBLE_SHADER;
        private static readonly ComputeShader PROCESS_SHADER;

        /// first called only on level start, so we good in terms of asset loading
        static PointsState() {
            var assetBundle = (AssetBundle)new Assets("assets").Bundle;
            STANDARD_SHADER = assetBundle.LoadAsset<Shader>("Standard.shader");
            INVISIBLE_SHADER = assetBundle.LoadAsset<Shader>("Invisible.shader");
            PROCESS_SHADER = assetBundle.LoadAsset<ComputeShader>("Process.compute");

            Shader.SetGlobalInt("_NumIDs", (int)NUM_IDS);
            PROCESS_SHADER.SetInt("NumIDs", (int)NUM_IDS);
        }

        private void Awake() {
            MainCamera = Camera.main!;
            MainCamera.enabled = false;

            Camera = gameObject.AddComponent<Camera>();
            Camera.enabled = false;
            Camera.cullingMask = MainCamera.cullingMask & ~PhysicsEx.carLayerMask_;
            Camera.clearFlags = CameraClearFlags.SolidColor;
            Camera.backgroundColor = Color.black;

            Texture = new RenderTexture(256, 256, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
            Camera.targetTexture = Texture;
            Buffer = new ComputeBuffer(Texture.width * Texture.height, sizeof(float) + sizeof(uint));
            Bytes = new byte[Buffer.count * Buffer.stride];

            PROCESS_SHADER.SetTexture(0, "Input", Texture);
            PROCESS_SHADER.SetBuffer(0, "Output", Buffer);
        }

        private void OnDestroy() {
            MainCamera.enabled = true;

            Destroy(Camera);

            Texture.Release();
            Buffer.Release();
        }

        /// modify the scene so that it will make sense to the agent
        private void Start() {
            static void Init(Renderer renderer, ID id) {
                foreach (var material in renderer.materials) {
                    material.shader = STANDARD_SHADER;
                    material.SetInt("_ID", (int)id);
                }
            }

            static void Invisible(Renderer renderer) {
                foreach (var material in renderer.materials) {
                    material.shader = INVISIBLE_SHADER;
                }
            }

            foreach (var renderer in Resources.FindObjectsOfTypeAll<Renderer>()) {
                if (!renderer.HasAnyComponent(typeof(Collider))) {
                    Invisible(renderer);
                    continue;
                }

                ID id;
                // todo add more specific things and planes with the IDs instead of just assigning them like this
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

        private bool DrawOverlay = true;

        private void Update() {
            // overlay toggle
            if (Input.GetKey(KeyCode.M) && Input.GetKeyDown(KeyCode.G)) {
                DrawOverlay = !DrawOverlay;
                MainCamera.enabled = !DrawOverlay;
                LOG.Debug($"overlay {(DrawOverlay ? "on" : "off")}");
            }
        }

        /// draw texture to screen
        private void OnGUI() {
            if (DrawOverlay)
                GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Texture);
        }


        /// update state stuff
        public void UpdateState() {
            var parentTransform = Utils.PlayerDataLocal!.Car_.transform;
            transform.position = parentTransform.position + parentTransform.up;
            transform.rotation = parentTransform.rotation;

            Camera.Render();
            PROCESS_SHADER.Dispatch(0, Texture.width / 8, Texture.height / 8, 1);
            Buffer.GetData(Bytes);
        }
    }
}
