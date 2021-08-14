using HarmonyLib;
using JetBrains.Annotations;
using Serializers;
using UnityEngine;

namespace Distance.LevelMods {
    namespace ResourceManager_ {
        /// register our components
        [HarmonyPatch(typeof(ResourceManager), nameof(FilloutLevelPrefabs))]
        internal static class FilloutLevelPrefabs {
            [UsedImplicitly]
            private static void Postfix() {
                // manager can be saved/loaded, but not added via tab or put on any object
                var prefab = new GameObject(nameof(CustomObjectManager));
                prefab.SetActive(false);
                var comp = prefab.AddComponent<CustomObjectManager>();

                var man = G.Sys.ResourceManager_!;
                man.LevelPrefabs_[prefab.name] = prefab;
                BinaryDeserializer.idToSerializableTypeMap_[comp.ID_] = comp.GetType();
            }
        }
    }

    namespace LevelEditor_ {
        /// register our tools
        [HarmonyPatch(typeof(LevelEditor), nameof(RegisterToolJobs))]
        internal static class RegisterToolJobs {
            [UsedImplicitly]
            private static void Postfix() {
                G.Sys.LevelEditor_.currentRegisteringToolType_ = typeof(LoadCustomObjectTool);
                G.Sys.LevelEditor_.RegisterTool(LoadCustomObjectTool.info);
            }
        }
    }

    // namespace Deserializer_ {
    //     [HarmonyPatch(typeof(Deserializer), nameof(VisitGameObject))]
    //     internal static class VisitGameObject {
    //         [UsedImplicitly]
    //         private static void Postfix() {
    //             Entry.log.Debug("TODO");
    //         }
    //     }
    // }
}
