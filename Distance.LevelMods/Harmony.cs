using HarmonyLib;
using Serializers;
using UnityEngine;

namespace Distance.LevelMods {
    namespace ResourceManager_ {
        /// register Proxy as custom object
        [HarmonyPatch(typeof(ResourceManager), nameof(FilloutLevelPrefabs))]
        internal static class FilloutLevelPrefabs {
            private static void Postfix(ResourceManager __instance) {
                // so the gameobject can be serialized/deserialized
                var prefab = new GameObject(Proxy.DISPLAY_NAME, typeof(Proxy));
                __instance.LevelPrefabs_.Add(prefab.name, prefab);

                // so the component can be deserialized
                BinaryDeserializer.idToSerializableTypeMap_.Add(Proxy.ID, typeof(Proxy));

                // so the gameobject shows up in the library tab
                var root = __instance.LevelPrefabFileInfosRoot_;
                root.AddChildInfo(new LevelPrefabFileInfo(Proxy.DISPLAY_NAME, prefab, root));
            }
        }
    }
}
