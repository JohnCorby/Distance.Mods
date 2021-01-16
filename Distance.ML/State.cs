using UnityEngine;
using Events.Car;
using Events.Player;
using static Distance.ML.Entry;

namespace Distance.ML {
    /// holds all state of the network
    public class State : MonoBehaviour {
        private PointsState PointsState = null!;

        /// array of points in byte form so we won't have to allocate/convert more
        public byte[] PointsBytes = null!;
        /// reward value
        public float Reward;

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
            PointsState = gameObject.AddComponent<PointsState>();
            PointsBytes = new byte[PointsState.Buffer.count * PointsState.Buffer.stride];

            var events = Utils.PlayerDataLocal!.Events_;
            events.Subscribe<Finished.Data>(OnEventFinished);
            events.Subscribe<Death.Data>(OnEventDeath);
            events.Subscribe<CheckpointHit.Data>(OnEventCheckpoint);
            events.Subscribe<Cooldown.Data>(OnEventCooldown);
            events.Subscribe<TrickComplete.Data>(OnEventTrick);
            events.Subscribe<Split.Data>(OnEventSplit);
        }

        private void OnDestroy() {
            Destroy(PointsState);

            var events = Utils.PlayerDataLocal!.Events_;
            events.Unsubscribe<Finished.Data>(OnEventFinished);
            events.Unsubscribe<Death.Data>(OnEventDeath);
            events.Unsubscribe<CheckpointHit.Data>(OnEventCheckpoint);
            events.Unsubscribe<Cooldown.Data>(OnEventCooldown);
            events.Unsubscribe<TrickComplete.Data>(OnEventTrick);
            events.Unsubscribe<Split.Data>(OnEventSplit);
        }

        private void OnEventFinished(Finished.Data data) {
            if (data.finishType_ != FinishType.Normal) return;
            LOG.Debug($"finish: reward {REWARD_FINISH}");
            Reward += REWARD_FINISH;
        }

        private void OnEventDeath(Death.Data _) {
            LOG.Debug($"death: reward {REWARD_DEATH}");
            Reward += REWARD_DEATH;
        }

        private void OnEventCheckpoint(CheckpointHit.Data _) {
            LOG.Debug($"checkpoint: reward {REWARD_CHECKPOINT}");
            Reward += REWARD_CHECKPOINT;
        }

        private void OnEventCooldown(Cooldown.Data data) {
            LOG.Debug($"cooldown: reward {data.cooldownAmount * REWARD_COOLDOWN_SCALE}");
            Reward += data.cooldownAmount * REWARD_COOLDOWN_SCALE;
        }

        private void OnEventTrick(TrickComplete.Data data) {
            LOG.Debug($"trick: reward {data.cooldownAmount_ * REWARD_TRICK}");
            Reward += data.cooldownAmount_ * REWARD_TRICK;
        }

        private void OnEventSplit(Split.Data _) {
            LOG.Debug($"split: reward {REWARD_SPLIT}");
            Reward += REWARD_SPLIT;
        }

        /// update state stuff
        public void UpdateState() {
            PointsState.UpdateState();
            PointsState.Buffer.GetData(PointsBytes);
        }

        /// reset state for next step
        public void ResetState() =>
            Reward = 0;
    }
}
