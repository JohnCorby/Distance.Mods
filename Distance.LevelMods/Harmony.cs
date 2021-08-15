using HarmonyLib;
using JetBrains.Annotations;
using Serializers;

// ReSharper disable InconsistentNaming
namespace Distance.LevelMods {
    namespace ResourceManager_ {
        /// register our components
        [HarmonyPatch(typeof(ResourceManager), nameof(FilloutLevelPrefabs))]
        internal static class FilloutLevelPrefabs {
            [UsedImplicitly]
            private static void Postfix(ResourceManager __instance) {
                // manager can be saved/loaded, but not added via tab or put on any object
                var prefab = Utils.NewPrefab(nameof(CustomObjectManager));
                var comp = prefab.AddComponent<CustomObjectManager>();

                __instance.LevelPrefabs_[prefab.name] = prefab;
                BinaryDeserializer.idToSerializableTypeMap_[comp.ID_] = comp.GetType();
            }
        }
    }

    namespace LevelEditor_ {
        /// register our tools
        [HarmonyPatch(typeof(LevelEditor), nameof(RegisterToolJobs))]
        internal static class RegisterToolJobs {
            [UsedImplicitly]
            private static void Postfix(LevelEditor __instance) {
                __instance.currentRegisteringToolType_ = typeof(LoadCustomObjectTool);
                __instance.RegisterTool(LoadCustomObjectTool.info);
            }
        }
    }

    namespace LevelLayer_ {
        /// make custom object manager layer special
        [HarmonyPatch(typeof(LevelLayer), nameof(IsTrackNodeLayer_), MethodType.Getter)]
        internal static class IsTrackNodeLayer_ {
            [UsedImplicitly]
            private static void Postfix(LevelLayer __instance, ref bool __result) {
                __result = __result || __instance.Name_ == nameof(CustomObjectManager);
            }
        }
    }
}
