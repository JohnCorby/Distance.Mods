using HarmonyLib;

namespace Distance.Cheat {
    namespace PlayerDataLocal_ {
        [HarmonyPatch(typeof(PlayerDataLocal), nameof(InitializeVirtual))]
        public static class InitializeVirtual {
            private static void Postfix(PlayerDataLocal __instance) =>
                __instance.gameObject.AddComponent<Cheats>();
        }
    }
}
