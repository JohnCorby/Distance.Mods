using Events.Player;
using UnityEngine;
using static Distance.Cheat.Entry;

namespace Distance.Cheat {
    public class Cheats : MonoBehaviour {
        public PlayerDataLocal PlayerDataLocal = null!;

        private void Awake() {
            PlayerDataLocal = GetComponent<PlayerDataLocal>();

            var events = PlayerDataLocal.Events_;
            // events.Subscribe<AbilityStateChanged.Data>(OnAbilityStateChanged);
            events.Subscribe<CarInstantiate.Data>(OnCarInstantiate);

            gameObject.AddComponent<Hotkeys>();
        }

        private void OnDestroy() {
            var events = PlayerDataLocal.Events_;
            // events.Unsubscribe<AbilityStateChanged.Data>(OnAbilityStateChanged);
            events.Unsubscribe<CarInstantiate.Data>(OnCarInstantiate);
        }

        // private static void OnAbilityStateChanged(AbilityStateChanged.Data data) { }

        private void OnCarInstantiate(CarInstantiate.Data data) {
            if (CheatsEnabled)
                EnableCheats(false);
        }


        private bool CheatsEnabled;

        /// toggles cheats
        public void ToggleCheats() {
            CheatsEnabled = !CheatsEnabled;
            LOG.Info($"CHEATS {(CheatsEnabled ? "ON" : "OFF")}");

            // to prevent leaderboard times and other bad stuff
            var flags = new CheatFlags();
            flags.Set(ECheat.DeathProof, CheatsEnabled);
            G.Sys.CheatsManager_.UpdateCheats(flags);

            if (CheatsEnabled)
                EnableCheats(true);
            else
                DisableCheats(true);
        }

        private void EnableCheats(bool byToggle) {
            var localCar = PlayerDataLocal.localCar_;
            localCar.invincible_ = true;
            localCar.minInstaRespawnTriggerTime_ = 0;
            localCar.minInstaRespawnTime_ = 0;

            var boost = localCar.GetComponent<BoostGadget>();
            boost.accelerationMul_ = 3;
            boost.heatUpRate_ = 0;

            if (byToggle) {
                JetsGadget.thrusterBoostFullPowerLimit_ = 3;
                JetsGadget.thrusterBoostDepletedLimit_ = 3;
            }

            foreach (var gadget in localCar.GetComponents<Gadget>())
                gadget.SetAbilityEnabled(true, false);
        }

        private void DisableCheats(bool byToggle) {
            var localCar = PlayerDataLocal!.localCar_;
            localCar.invincible_ = false;
            localCar.minInstaRespawnTriggerTime_ = 0.05f;
            localCar.minInstaRespawnTime_ = 0.35f;

            var boost = localCar.GetComponent<BoostGadget>();
            boost.accelerationMul_ = 1f;
            boost.heatUpRate_ = 0.11f;

            if (byToggle) {
                JetsGadget.thrusterBoostFullPowerLimit_ = 1f;
                JetsGadget.thrusterBoostDepletedLimit_ = 0.4f;
            }

            foreach (var gadget in localCar.GetComponents<Gadget>())
                gadget.SetAbilityEnabled(false, false);
        }
    }
}
