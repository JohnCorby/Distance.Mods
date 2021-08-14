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
                var man = G.Sys.ResourceManager_!;
                var root = man.LevelPrefabFileInfosRoot_!;

                {
                    var prefab = new GameObject();
                    var comp = prefab.AddComponent<Proxy>();
                    prefab.name = comp.DisplayName_;

                    // so the gameobject can be serialized/deserialized
                    man.LevelPrefabs_[prefab.name] = prefab;
                    // so the component can be deserialized
                    BinaryDeserializer.idToSerializableTypeMap_[comp.ID_] = comp.GetType();

                    // so the gameobject shows up in the library tab
                    root.AddChildInfo(new LevelPrefabFileInfo(prefab.name, prefab, root));
                }

                {
                    // manager can be saved/loaded, but not added via tab or put on any object
                    var prefab = new GameObject();
                    var comp = prefab.AddComponent<CompManager>();
                    prefab.name = comp.DisplayName_;

                    man.LevelPrefabs_[prefab.name] = prefab;
                    BinaryDeserializer.idToSerializableTypeMap_[comp.ID_] = comp.GetType();
                }
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
