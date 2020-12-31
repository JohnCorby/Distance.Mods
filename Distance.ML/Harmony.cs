using HarmonyLib;
using UnityEngine;

namespace Distance.ML {
    namespace PlayerDataLocal_ {
        [HarmonyPatch(typeof(PlayerDataLocal), nameof(InitializeVirtual))]
        public static class InitializeVirtual {
            private static void Postfix(Component __instance) =>
                __instance.gameObject.AddComponent<Communication>();
        }
    }
}
