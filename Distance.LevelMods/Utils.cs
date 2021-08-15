using System;
using Reactor.API.Storage;
using UnityEngine;

namespace Distance.LevelMods {
    public static class Utils {
        /// goofy way to create a prefab at runtime
        public static GameObject NewPrefab(string? name = null, params Type[] components) {
            var bundle = (AssetBundle)new Assets("empty").Bundle;
            var prefab = bundle.LoadAsset<GameObject>("Empty");
            bundle.Unload(false);
            prefab.name = name;
            foreach (var componentType in components)
                prefab.AddComponent(componentType);
            return prefab;
        }
    }
}
