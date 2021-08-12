using UnityEngine;
using Events.Car;
using Events.Player;
using static Distance.ML.Entry;

namespace Distance.ML {
    /// holds all state of the network
    public class State : MonoBehaviour {
        public PointsState pointsState = null!;
        public InputsState inputsState = null!;

        /// reward value
        public float reward;

        /// if env is done
        public static bool done => Utils.playerDataLocal!.Finished_;

        private const float rewardFinish = 100;
        private const float rewardDeath = -100;
        private const float rewardCheckpoint = 50;
        private const float rewardCooldownScale = 20;
        private const float rewardTrick = 20;
        private const float rewardSplit = -20;
        private const float rewardForwardScale = 10;
        private const float rewardBackwardScale = -10;

        private void Awake() {
            pointsState = gameObject.AddComponent<PointsState>();
            inputsState = gameObject.AddComponent<InputsState>();

            var events = Utils.playerDataLocal!.Events_;
            events.Subscribe<Finished.Data>(OnEventFinished);
            events.Subscribe<Death.Data>(OnEventDeath);
            events.Subscribe<CheckpointHit.Data>(OnEventCheckpoint);
            events.Subscribe<Cooldown.Data>(OnEventCooldown);
            events.Subscribe<TrickComplete.Data>(OnEventTrick);
            events.Subscribe<Split.Data>(OnEventSplit);
        }

        private void OnDestroy() {
            Destroy(pointsState);

            var events = Utils.playerDataLocal!.Events_;
            events.Unsubscribe<Finished.Data>(OnEventFinished);
            events.Unsubscribe<Death.Data>(OnEventDeath);
            events.Unsubscribe<CheckpointHit.Data>(OnEventCheckpoint);
            events.Unsubscribe<Cooldown.Data>(OnEventCooldown);
            events.Unsubscribe<TrickComplete.Data>(OnEventTrick);
            events.Unsubscribe<Split.Data>(OnEventSplit);
        }

        private void OnEventFinished(Finished.Data data) {
            if (data.finishType_ != FinishType.Normal) return;
            log.Debug($"finish: reward {rewardFinish}");
            reward += rewardFinish;
        }

        private void OnEventDeath(Death.Data _) {
            log.Debug($"death: reward {rewardDeath}");
            reward += rewardDeath;
        }

        private void OnEventCheckpoint(CheckpointHit.Data _) {
            log.Debug($"checkpoint: reward {rewardCheckpoint}");
            reward += rewardCheckpoint;
        }

        private void OnEventCooldown(Cooldown.Data data) {
            log.Debug($"cooldown: reward {data.cooldownAmount * rewardCooldownScale}");
            reward += data.cooldownAmount * rewardCooldownScale;
        }

        private void OnEventTrick(TrickComplete.Data data) {
            log.Debug($"trick: reward {data.cooldownAmount_ * rewardTrick}");
            reward += data.cooldownAmount_ * rewardTrick;
        }

        private void OnEventSplit(Split.Data _) {
            log.Debug($"split: reward {rewardSplit}");
            reward += rewardSplit;
        }

        /// update state stuff
        public void UpdateState() {
            inputsState.UpdateState();
            pointsState.UpdateState();
        }

        /// reset state for next step
        public void ResetState() => reward = 0;
    }
}
