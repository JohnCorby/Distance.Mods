using UnityEngine;

namespace Test {
    public class Test : MonoBehaviour {
        public Material StandardMaterial;

        private RenderTexture RenderTexture;
        private Texture2D Texture2D;

        private void Awake() {
            var camera = gameObject.AddComponent<Camera>();
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = Color.black;

            RenderTexture = new RenderTexture(camera.pixelWidth, camera.pixelHeight,
                0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
            Texture2D = new Texture2D(RenderTexture.width, RenderTexture.height,
                TextureFormat.RGBAFloat, false, true);

            camera.targetTexture = RenderTexture;
        }

        private int ID;

        private void Update() {
            var lastActive = RenderTexture.active;
            RenderTexture.active = RenderTexture;
            Texture2D.ReadPixels(new Rect(0, 0, RenderTexture.width, RenderTexture.height), 0, 0, RenderTexture);
            RenderTexture.active = lastActive;

            var pixel = Texture2D.GetPixel(Texture2D.width / 2, Texture2D.height / 2);
            print($"got pixel {pixel}");

            StandardMaterial.SetInt("_ID", ID);
            print($"id should be {ID}");
            ID = (ID + 1) % 10;
        }
    }
}
