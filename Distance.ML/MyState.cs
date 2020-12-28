using UnityEngine;
using Events.Car;
using Events.Player;
using static Distance.ML.Entry;

namespace Distance.ML {
    public class MyState : MonoBehaviour {
        private static readonly Texture2D TEXTURE = new(MyCamera.RENDER_TEXTURE.width, MyCamera.RENDER_TEXTURE.height);

        public static readonly float[,,] TEXTURE_DATA = new float[TEXTURE.width, TEXTURE.height, 2];
        public static float Reward;
        public static bool Done => Utils.PlayerDataLocal!.Finished_;

        private const float REWARD_FINISH = 100;
        private const float REWARD_DEATH = -100;
        private const float REWARD_CHECKPOINT = 50;
        private const float REWARD_COOLDOWN_SCALE = 20;
        private const float REWARD_TRICK = 20;
        private const float REWARD_SPLIT = -20;
        private const float REWARD_FORWARD_SCALE = 10;
        private const float REWARD_BACKWARD_SCALE = -10;

        private void Awake() {
            var events = Utils.PlayerDataLocal!.Events_;
            events.Subscribe<Finished.Data>(_ => {
                LOG.Debug($"finish: reward {REWARD_FINISH}");
                Reward += REWARD_FINISH;
            });
            events.Subscribe<Death.Data>(_ => {
                LOG.Debug($"death: reward {REWARD_DEATH}");
                Reward += REWARD_DEATH;
            });
            events.Subscribe<CheckpointHit.Data>(_ => {
                LOG.Debug($"checkpoint: reward {REWARD_CHECKPOINT}");
                Reward += REWARD_CHECKPOINT;
            });
            events.Subscribe<Cooldown.Data>(data => {
                LOG.Debug($"cooldown: reward {data.cooldownAmount * REWARD_COOLDOWN_SCALE}");
                Reward += data.cooldownAmount * REWARD_COOLDOWN_SCALE;
            });
            events.Subscribe<TrickComplete.Data>(data => {
                LOG.Debug($"cooldown: reward {data.cooldownAmount_ * REWARD_TRICK}");
                Reward += data.cooldownAmount_ * REWARD_TRICK;
            });
            events.Subscribe<Split.Data>(_ => {
                LOG.Debug($"split: reward {REWARD_SPLIT}");
                Reward += REWARD_SPLIT;
            });
        }

        private static void UpdateState() {
            UpdateTexture();

            LOG.Debug(TEXTURE.GetRawTextureData().Length);
            LOG.Debug(TEXTURE.width * TEXTURE.height * 4 * sizeof(float));

            for (var x = 0; x < TEXTURE.width; x++) {
                for (var y = 0; y < TEXTURE.height; y++) {
                    var pixel = TEXTURE.GetPixel(x, y);
                    TEXTURE_DATA[x, y, 0] = pixel.r;
                    TEXTURE_DATA[x, y, 1] = pixel.g;
                }
            }


        }

        private static void UpdateTexture() {
            var lastActive = RenderTexture.active;
            RenderTexture.active = MyCamera.RENDER_TEXTURE;
            TEXTURE.ReadPixels(new Rect(0, 0, TEXTURE.width, TEXTURE.height), 0, 0);
            RenderTexture.active = lastActive;

            LOG.Info(TEXTURE.GetPixel(TEXTURE.width / 2, TEXTURE.height / 2));
        }

        public static void ResetState() {
            Reward = 0;
        }
    }
}
