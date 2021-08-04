using HarmonyLib;

namespace Distance.Cheat {
    namespace PlayerDataLocal_ {
        [HarmonyPatch(typeof(PlayerDataLocal), nameof(InitializeVirtual))]
        public static class InitializeVirtual {
            private static void Postfix(PlayerDataLocal __instance) =>
                __instance.gameObject.AddComponent<Cheats>();
        }
    }

    namespace GameMode_ {
        [HarmonyPatch(typeof(GameMode), nameof(UploadScoreAndReplay))]
        public static class UploadScoreAndReplay {
            /// skip method (return false) if cheat instance exists and cheats were ever enabled
            private static bool Prefix() {
                if (Cheats.Instance != null && Cheats.Instance.CheatsEverEnabled) {
                    Entry.LOG.Debug("skipping upload because cheats were enabled at some point");
                    return false;
                }

                return true;
            }
        }
    }

    namespace MyNamespace {
        [HarmonyPatch(typeof(CheatsManager), nameof(OnEventSceneLoadFinished))]
        public static class OnEventSceneLoadFinished {
            /// copied from the original, except always sets gameplayCheatsRecognized_ to true so they always work
            private static bool Prefix(CheatsManager __instance) {
                __instance.gameplayCheatsRecognized_ = true;
                __instance.UpdateEnabledFlags();
                __instance.anyGameplayCheatsUsedThisLevel_ = __instance.AnyGameplayCheatsCurrentlyUsed_;

                // always skip the method so this is the replacement method
                return false;
            }
        }
    }
}
