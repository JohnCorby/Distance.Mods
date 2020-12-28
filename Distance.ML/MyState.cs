using System;
using UnityEngine;
using Events.Car;

namespace Distance.ML {
    public class MyState : MonoBehaviour {
        public static int Reward;

        private static readonly Texture2D TEXTURE = new(MyCamera.RENDER_TEXTURE.width, MyCamera.RENDER_TEXTURE.height);

        private enum RewardType : int {
            DEATH=-100,
            CHECKPOINT=100,
            COOLDOWN=100,
            FORWARD=10,
            BACKWARD=-10,
        }

        private void Awake() {
            var events = Utils.PlayerDataLocal!.Events_;
            events.Subscribe<Death.Data>(_ => Reward += (int) RewardType.DEATH);
            events.Subscribe<CheckpointHit.Data>(_ => Reward += (int) RewardType.CHECKPOINT);
        }

        private void Update() {
        }

        public static void FilLTexture() {

        }

        public static void ResetState() {
            Reward = 0;
        }
    }
}
