using System;
using System.Linq;
using Reactor.API.Storage;
using UnityEngine;
using static Distance.ML.Entry;

namespace Distance.ML {
    /// holds state for points data
    public class PointsState : MonoBehaviour {
        private Camera mainCamera = null!;
        private Camera camera = null!;
        private RenderTexture texture = null!;

        /// buffer that holds points data
        private ComputeBuffer buffer = null!;
        /// array of points in byte form so we won't have to allocate/convert more
        public byte[] bytes = null!;

        private enum ID : uint {
            Normal,
            KillGrid,
            Laser,
            Saw,
            End,
            Checkpoint,
            Cooldown,
            Teleporter,
        }

        private static readonly uint numIds = (uint)Enum.GetNames(typeof(ID)).Length;

        private static readonly Shader standardShader, invisibleShader;
        private static readonly ComputeShader processShader;

        /// first called only on level start, so we good in terms of asset loading
        static PointsState() {
            var assetBundle = (AssetBundle)new Assets("assets").Bundle;
            standardShader = assetBundle.LoadAsset<Shader>("Standard.shader");
            invisibleShader = assetBundle.LoadAsset<Shader>("Invisible.shader");
            processShader = assetBundle.LoadAsset<ComputeShader>("Process.compute");
            assetBundle.Unload(false);

            Shader.SetGlobalInt("_NumIDs", (int)numIds);
            processShader.SetInt("NumIDs", (int)numIds);
        }

        private void Awake() {
            mainCamera = Camera.main!;
            mainCamera.enabled = false;

            camera = gameObject.AddComponent<Camera>();
            camera.enabled = false;
            camera.cullingMask = mainCamera.cullingMask & ~PhysicsEx.carLayerMask_;
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = Color.black;

            texture = new RenderTexture(256, 256, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
            camera.targetTexture = texture;
            buffer = new ComputeBuffer(texture.width * texture.height, sizeof(float) + sizeof(uint));
            bytes = new byte[buffer.count * buffer.stride];

            processShader.SetTexture(0, "Input", texture);
            processShader.SetBuffer(0, "Output", buffer);
        }

        private void OnDestroy() {
            mainCamera.enabled = true;

            Destroy(camera);

            texture.Release();
            buffer.Release();
        }

        /// modify the scene so that it will make sense to the agent
        private void Start() {
            foreach (var renderer in Resources.FindObjectsOfTypeAll<Renderer>()) {
                void Init(ID id) {
                    foreach (var material in renderer.materials) {
                        material.shader = standardShader;
                        material.SetInt("_ID", (int)id);
                    }
                }

                void Invisible() {
                    foreach (var material in renderer!.materials) {
                        material.shader = invisibleShader;
                    }
                }

                bool Has(params Type[] types) => types.Any(type =>
                    renderer!.GetComponentInParent(type) | renderer.GetComponentInChildren(type));


                // todo add more specific things and planes with the IDs instead of just assigning them like this
                if (Has(typeof(KillGridBox), typeof(KillGridFollower)))
                    Init(ID.KillGrid);
                else if (Has(typeof(RaceEndLogic))) Init(ID.End);
                else if (Has(typeof(LaserLogic))) Init(ID.Laser);
                else if (Has(typeof(SharpObject))) Init(ID.Saw);
                else if (Has(typeof(CheckpointLogicBase))) Init(ID.Checkpoint);
                else if (Has(typeof(TriggerCooldownLogic))) Init(ID.Cooldown);
                else if (Has(typeof(TeleporterEntrance), typeof(TeleporterExit)))
                    Init(ID.Teleporter);
                else if (!Has(typeof(Collider))) Invisible();
                else Init(ID.Normal);
            }
        }

        private bool drawOverlay = true;

        private void Update() {
            // overlay toggle
            if (Input.GetKey(KeyCode.M) && Input.GetKeyDown(KeyCode.G)) {
                drawOverlay = !drawOverlay;
                mainCamera.enabled = !drawOverlay;
                log.Debug($"overlay {(drawOverlay ? "on" : "off")}");
            }
        }

        /// draw texture to screen
        private void OnGUI() {
            if (drawOverlay)
                GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), texture);
        }


        /// update state stuff
        public void UpdateState() {
            var parentTransform = Utils.playerDataLocal!.Car_.transform;
            transform.position = parentTransform.position + parentTransform.up;
            transform.rotation = parentTransform.rotation;

            camera.Render();
            processShader.Dispatch(0, texture.width / 8, texture.height / 8, 1);
            buffer.GetData(bytes);
        }
    }
}
