using Events;
using Events.Local;
using Events.Player;
using UnityEngine;
using static Distance.Cheat.Entry;

namespace Distance.Cheat {
    public class Cheats : MonoBehaviour {
        public static Cheats? instance;

        public PlayerDataLocal playerDataLocal = null!;

        private void Awake() {
            instance = this;

            playerDataLocal = GetComponent<PlayerDataLocal>();

            var events = playerDataLocal.Events_;
            // events.Subscribe<AbilityStateChanged.Data>(OnAbilityStateChanged);
            events.Subscribe<CarInstantiate.Data>(OnCarInstantiate);

            gameObject.AddComponent<Hotkeys>();

            // reset static fields
            JetsGadget.thrusterBoostFullPowerLimit_ = 1f;
            JetsGadget.thrusterBoostDepletedLimit_ = 0.4f;
        }

        private void OnDestroy() {
            var events = playerDataLocal.Events_;
            // events.Unsubscribe<AbilityStateChanged.Data>(OnAbilityStateChanged);
            events.Unsubscribe<CarInstantiate.Data>(OnCarInstantiate);

            instance = null;
        }

        // private static void OnAbilityStateChanged(AbilityStateChanged.Data data) { }

        private void OnCarInstantiate(CarInstantiate.Data data) {
            if (cheatsEnabled)
                EnableCheats(false);
        }


        public bool cheatsEnabled, cheatsEverEnabled;

        public void ToggleCheats() {
            cheatsEnabled = !cheatsEnabled;
            log.Info($"CHEATS {(cheatsEnabled ? "ON" : "OFF")}");

            if (cheatsEnabled) {
                cheatsEverEnabled = true;
                EnableCheats(true);
            } else
                DisableCheats(true);
        }

        private void EnableCheats(bool byToggle) {
            var localCar = playerDataLocal.localCar_;
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

            if (byToggle) {
                // send the funny chat message
                const string message = "I just turned cheats on. My leaderboard stuff wont be submitted. Make fun of me :)";
                StaticEvent<ChatSubmitMessage.Data>.Broadcast(new ChatSubmitMessage.Data(message));
            }
        }

        private void DisableCheats(bool byToggle) {
            var localCar = playerDataLocal!.localCar_;
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
