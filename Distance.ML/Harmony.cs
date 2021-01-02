using HarmonyLib;
using UnityEngine;
using static Distance.ML.Entry;

namespace Distance.ML {
    namespace PlayerDataLocal_ {
        [HarmonyPatch(typeof(PlayerDataLocal), nameof(InitializeVirtual))]
        public static class InitializeVirtual {
            private static void Postfix(Component __instance) {
                if (!G.Sys.GameManager_.SoloAndNotOnline_) {
                    LOG.Error("must be in solo game");
                    return;
                }
                __instance.gameObject.AddComponent<Communication>();
            }
        }
    }
}
