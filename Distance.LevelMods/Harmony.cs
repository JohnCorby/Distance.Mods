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
                    man.LevelPrefabs_.Add(prefab.name, prefab);
                    // so the component can be deserialized
                    BinaryDeserializer.idToSerializableTypeMap_.Add(comp.ID_, comp.GetType());

                    // so the gameobject shows up in the library tab
                    root.AddChildInfo(new LevelPrefabFileInfo(prefab.name, prefab, root));
                }

                {
                    // simple prefab used for the library tab
                    var prefab = new GameObject();
                    var comp = prefab.AddComponent<CompLoader>();
                    prefab.name = comp.DisplayName_;

                    root.AddChildInfo(new LevelPrefabFileInfo(prefab.name, prefab, root));

                    // this comp can go on any gameobject, regardless of if it's on that object's prefab
                    man.AddedComponentsPrefab_.AddComponent(comp.GetType());
                    BinaryDeserializer.idToSerializableTypeMap_.Add(comp.ID_, comp.GetType());

                    // we dont add it to level prefabs because the prefab itself should never be saved or loaded
                }

                {
                    // manager can be saved/loaded, but not added via tab or put on any object
                    var prefab = new GameObject();
                    var comp = prefab.AddComponent<CompManager>();
                    prefab.name = comp.DisplayName_;

                    man.LevelPrefabs_.Add(prefab.name, prefab);
                    BinaryDeserializer.idToSerializableTypeMap_.Add(comp.ID_, comp.GetType());
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
