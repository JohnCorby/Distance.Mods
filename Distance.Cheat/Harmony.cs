using HarmonyLib;
using static Distance.Cheat.Entry;

namespace Distance.Cheat {
    namespace LocalPlayerControlledCar_ {
        [HarmonyPatch(typeof(LocalPlayerControlledCar), nameof(Start))]
        public static class Start {
            private static void Postfix(LocalPlayerControlledCar __instance) {
                Car = __instance;
            }
        }

        [HarmonyPatch(typeof(LocalPlayerControlledCar), nameof(OnDisable))]
        public static class OnDisable {
            private static void Postfix() {
                LOG.Info("CHEATS OFF");

                JetsGadget.thrusterBoostFullPowerLimit_ = 1f;
                JetsGadget.thrusterBoostDepletedLimit_ = 0.4f;
            }
        }
    }
}
