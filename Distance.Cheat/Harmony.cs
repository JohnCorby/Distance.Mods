using HarmonyLib;
using static Distance.Cheat.Entry;

namespace Distance.Cheat {
    namespace LocalPlayerControlledCar_ {
        /// give player a bunch of cool shit every time the car object is created
        [HarmonyPatch(typeof(LocalPlayerControlledCar), nameof(Start))]
        public static class Start {
            private static void Postfix(LocalPlayerControlledCar __instance) {
                Car = __instance;
            }
        }
    }
}
