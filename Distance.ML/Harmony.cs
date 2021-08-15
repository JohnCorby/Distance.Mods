using HarmonyLib;
using JetBrains.Annotations;
using UnityEngine;

// ReSharper disable InconsistentNaming
namespace Distance.ML {
    namespace PlayerDataLocal_ {
        [HarmonyPatch(typeof(PlayerDataLocal), nameof(InitializeVirtual))]
        public static class InitializeVirtual {
            [UsedImplicitly]
            private static void Postfix(Component __instance) =>
                __instance.gameObject.AddComponent<Communication>();
        }
    }
}
