using HarmonyLib;
using JetBrains.Annotations;
using Serializers;
using UnityEngine;

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

    namespace Level_ {
        /// temporarily add the manager when we save
        [HarmonyPatch(typeof(Level), nameof(SaveToPath), typeof(string), typeof(bool))]
        internal static class SaveToPath {
            [UsedImplicitly]
            private static void Prefix(Level __instance) {
                var le = G.Sys.levelEditor_!;

                var prefab = G.Sys.ResourceManager_.levelPrefabs_[nameof(CustomObjectManager)];
                var obj = le.CreateObject(prefab);

                var layer = __instance.CreateAndInsertNewLayer(0, nameof(CustomObjectManager), false, true, false);
                ReferenceMap.Handle<GameObject> handle = default;
                le.AddGameObject(ref handle, obj, layer);
            }

            [UsedImplicitly]
            private static void Postfix(Level __instance) {
                var layer = __instance.GetLayer(nameof(CustomObjectManager));
                if (layer != null) __instance.TryDeleteLayer(layer, true);
            }
        }
    }
}
