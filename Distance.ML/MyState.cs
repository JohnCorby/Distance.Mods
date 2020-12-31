using UnityEngine;
using Events.Car;
using Events.Player;
using static Distance.ML.Entry;

namespace Distance.ML {
    public class MyState : MonoBehaviour {
        private static readonly Texture2D TEXTURE = new(MyCamera.RENDER_TEXTURE.width, MyCamera.RENDER_TEXTURE.height,
            TextureFormat.RGBA32, false, true);

        /// texture data in form (depth, id)
        public static readonly float[,,] TEXTURE_DATA = new float[TEXTURE.width, TEXTURE.height, 2];
        /// reward value
        public static float Reward;

        /// if env is done
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
            events.Subscribe<Finished.Data>(OnEventFinished);
            events.Subscribe<Death.Data>(OnEventDeath);
            events.Subscribe<CheckpointHit.Data>(OnEventCheckpoint);
            events.Subscribe<Cooldown.Data>(OnEventCooldown);
            events.Subscribe<TrickComplete.Data>(OnEventTrick);
            events.Subscribe<Split.Data>(OnEventSplit);
        }

        private void OnDestroy() {
            var events = Utils.PlayerDataLocal!.Events_;
            events.Unsubscribe<Finished.Data>(OnEventFinished);
            events.Unsubscribe<Death.Data>(OnEventDeath);
            events.Unsubscribe<CheckpointHit.Data>(OnEventCheckpoint);
            events.Unsubscribe<Cooldown.Data>(OnEventCooldown);
            events.Unsubscribe<TrickComplete.Data>(OnEventTrick);
            events.Unsubscribe<Split.Data>(OnEventSplit);
        }

        private static void OnEventFinished(Finished.Data data) {
            if (data.finishType_ != FinishType.Normal) return;
            LOG.Debug($"finish: reward {REWARD_FINISH}");
            Reward += REWARD_FINISH;
        }

        private static void OnEventDeath(Death.Data _) {
            LOG.Debug($"death: reward {REWARD_DEATH}");
            Reward += REWARD_DEATH;
        }

        private static void OnEventCheckpoint(CheckpointHit.Data _) {
            LOG.Debug($"checkpoint: reward {REWARD_CHECKPOINT}");
            Reward += REWARD_CHECKPOINT;
        }

        private static void OnEventCooldown(Cooldown.Data data) {
            LOG.Debug($"cooldown: reward {data.cooldownAmount * REWARD_COOLDOWN_SCALE}");
            Reward += data.cooldownAmount * REWARD_COOLDOWN_SCALE;
        }

        private static void OnEventTrick(TrickComplete.Data data) {
            LOG.Debug($"cooldown: reward {data.cooldownAmount_ * REWARD_TRICK}");
            Reward += data.cooldownAmount_ * REWARD_TRICK;
        }

        private static void OnEventSplit(Split.Data _) {
            LOG.Debug($"split: reward {REWARD_SPLIT}");
            Reward += REWARD_SPLIT;
        }

        /// update state stuff
        public static void UpdateState() {
            UpdateTexture();

            for (var x = 0; x < TEXTURE.width; x++) {
                for (var y = 0; y < TEXTURE.height; y++) {
                    var pixel = TEXTURE.GetPixel(x, y);
                    TEXTURE_DATA[x, y, 0] = pixel.r;
                    TEXTURE_DATA[x, y, 1] = pixel.g;
                }
            }
        }

        /// copy render texture to cpu-readable texture
        private static void UpdateTexture() {
            var lastActive = RenderTexture.active;
            RenderTexture.active = MyCamera.RENDER_TEXTURE;
            TEXTURE.ReadPixels(new Rect(0, 0, TEXTURE.width, TEXTURE.height), 0, 0);
            RenderTexture.active = lastActive;

            LOG.Info(TEXTURE.GetPixel(TEXTURE.width / 2, TEXTURE.height / 2));
        }

        /// reset state for next step
        public static void ResetState() {
            Reward = 0;
        }
    }
}
