using HarmonyLib;
using JetBrains.Annotations;
using Serializers;
using UnityEngine;

namespace Distance.LevelMods {
    namespace ResourceManager_ {
        /// register levelmods components
        [HarmonyPatch(typeof(ResourceManager), nameof(FilloutLevelPrefabs))]
        internal static class FilloutLevelPrefabs {
            [UsedImplicitly]
            private static void Postfix() {
                var man = G.Sys.ResourceManager_!;
                var root = man.LevelPrefabFileInfosRoot_;

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
                    // simple prefab used for the library tab
                    var prefab = new GameObject();
                    var comp = prefab.AddComponent<CompLoader>();
                    prefab.name = comp.DisplayName_;

                    root.AddChildInfo(new LevelPrefabFileInfo(prefab.name, prefab, root));

                    // this comp should not be saved or loaded
                    // and neither should the prefab
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
