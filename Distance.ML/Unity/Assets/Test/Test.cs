using System;
using System.Linq;
using UnityEngine;

namespace Test {
    public class Test : MonoBehaviour {
        private struct Point {
            private readonly float Depth;
            private readonly uint ID;

            public override string ToString() {
                return string.Format("({0},{1})", Depth, ID);
            }
        }

        public Material StandardMaterial;
        public ComputeShader ProcessShader;

        private Camera Camera;
        private RenderTexture RenderTexture;
        private ComputeBuffer PointsBuffer;
        private Point[] Points;
        private byte[] Bytes;

        private const uint NUM_IDS = 10;

        private void Awake() {
            Camera = gameObject.AddComponent<Camera>();
            Camera.clearFlags = CameraClearFlags.SolidColor;
            Camera.backgroundColor = Color.black;
            Camera.farClipPlane = 10;

            RenderTexture = new RenderTexture(8, 8,
                0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
            Camera.targetTexture = RenderTexture;

            PointsBuffer = new ComputeBuffer(RenderTexture.width * RenderTexture.height, sizeof(float) + sizeof(uint));
            Points = new Point[PointsBuffer.count];
            Bytes = new byte[PointsBuffer.count * PointsBuffer.stride];

            ProcessShader.SetTexture(0, "Input", RenderTexture);
            ProcessShader.SetBuffer(0, "Output", PointsBuffer);
            ProcessShader.SetInt("NumIDs", (int) NUM_IDS);

            Shader.SetGlobalInt("_NumIDs", (int) NUM_IDS);
        }

        private void OnDestroy() {
            Destroy(Camera);

            RenderTexture.Release();
            PointsBuffer.Release();
        }

        private uint ID;

        private void Update() {
            ProcessShader.Dispatch(0, RenderTexture.width / 8, RenderTexture.height / 8, 1);
            PointsBuffer.GetData(Points);
            PointsBuffer.GetData(Bytes);

            // print(Points[RenderTexture.width / 2 + RenderTexture.height / 2 * RenderTexture.width]);
            print(string.Join(", ", Points.Select(b => b.ToString()).ToArray()));
            print(string.Join(", ", Bytes.Select(b => b.ToString()).ToArray()));

            StandardMaterial.SetInt("_ID", (int) ID);
            // print("id should be " + ID);
            ID = (ID + 1) % NUM_IDS;
        }

        private void OnGUI() {
            GUI.DrawTexture(new Rect(0, 0, RenderTexture.width, RenderTexture.height), RenderTexture);
        }
    }
}
