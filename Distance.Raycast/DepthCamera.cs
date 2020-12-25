using System;
using Reactor.API.Storage;
using UnityEngine;

namespace Distance.Raycast {
    public class DepthCamera : MonoBehaviour {
        private Camera Camera;
        private readonly RenderTexture RenderTexture =
            new(Screen.width / 2, Screen.height / 2, 24);
        private Texture2D Texture2D;

        private static readonly Shader SHADER;

        static DepthCamera() {
            var assetBundle = (AssetBundle) new Assets("shaders").Bundle;
            SHADER = assetBundle.LoadAsset<Shader>("Depth.shader");
        }

        private void Awake() {
            Camera = gameObject.AddComponent<Camera>();
            // Camera.SetReplacementShader(SHADER, "");
            // Camera.cullingMask = PhysicsEx.NoCarsRayCastLayerMask_;
            Camera.depthTextureMode = DepthTextureMode.Depth;
            Camera.targetTexture = RenderTexture;

            Texture2D = new Texture2D(RenderTexture.width, RenderTexture.height);
        }

        // private void OnPreRender() {
        //     Camera.CopyFrom(Camera.main);
        // }

        /// copy RenderTexture to cpu-readable Texture2D
        // private void OnPostRender() {
        //     var lastActive = RenderTexture.active;
        //     RenderTexture.active = RenderTexture;
        //     Texture2D.ReadPixels(new Rect(0, 0, Texture2D.width, Texture2D.height), 0, 0);
        //     RenderTexture.active = lastActive;
        // }
        private void OnGUI() {
            GUI.DrawTexture(new Rect(0, 0, RenderTexture.width, RenderTexture.height), RenderTexture);
        }
    }
}
