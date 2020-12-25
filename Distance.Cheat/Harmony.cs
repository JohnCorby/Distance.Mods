using HarmonyLib;

namespace Distance.Cheat {
    namespace LocalPlayerControlledCar_ {
        /// give player a bunch of cool shit every time the car object is created
        [HarmonyPatch(typeof(LocalPlayerControlledCar), nameof(Start))]
        internal static class Start {
            private static void Postfix(LocalPlayerControlledCar __instance) {
                __instance.invincible_ = true;
                __instance.minInstaRespawnTriggerTime_ = 0;
                __instance.minInstaRespawnTime_ = 0;

                var boost = __instance.GetComponent<BoostGadget>();
                boost.accelerationMul_ *= 3;
                boost.heatUpRate_ = 0;

                foreach (var gadget in __instance.GetComponents<Gadget>())
                    gadget.SetAbilityEnabled(true, false);

                if (Entry.IsFreshLevel) {
                    Entry.OnCarSpawn(__instance);
                    Entry.IsFreshLevel = false;
                }
            }
        }
    }

    // namespace CarLogic_ {
    //     /// spam teleporting player to the end
    //     [HarmonyPatch(typeof(CarLogic), nameof(Update))]
    //     internal static class Update {
    //         private static void Postfix(CarLogic __instance) {
    //             if (Entry.RaceEndLogic != null)
    //                 __instance.transform.position = Entry.RaceEndLogic.transform.position;
    //         }
    //     }
    // }

    namespace JetsGadget_ {
        /// reset grip timer so we always have good grip
        [HarmonyPatch(typeof(JetsGadget), nameof(GadgetUpdate))]
        internal static class GadgetUpdate {
            private static void Postfix(JetsGadget __instance) {
                __instance.thrusterBoostTimer_ = 0;
            }
        }
    }
}
