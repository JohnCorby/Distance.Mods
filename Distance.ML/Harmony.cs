using HarmonyLib;
using JetBrains.Annotations;
using UnityEngine;

namespace Distance.ML {
    namespace PlayerDataLocal_ {
        [HarmonyPatch(typeof(PlayerDataLocal), nameof(InitializeVirtual))]
        public static class InitializeVirtual {
            [UsedImplicitly]
            // ReSharper disable once InconsistentNaming
            private static void Postfix(Component __instance) =>
                __instance.gameObject.AddComponent<Communication>();
        }
    }
}
