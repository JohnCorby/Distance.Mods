using System;
using System.IO;
using System.Reflection;
using Serializers;
using UnityEngine;

namespace Distance.LevelMods {
    public static class AssetBundleLoader {
        /// the name that we look for as the main thing to load
        private const string MAIN_NAME = "Entry";

        /// load all dlls, and prefab with name
        public static GameObject LoadPrefab(this AssetBundle bundle, string name = MAIN_NAME) {
            // load all dlls
            foreach (var asset in bundle.GetAllAssetNames()) {
                if (Path.GetExtension(asset) != ".bytes") continue;

                var dllBytes = bundle.LoadAsset<TextAsset>(asset).bytes;
                try {
                    Assembly.Load(dllBytes);
                } catch (BadImageFormatException e) {
                    Debug.LogError($"could not load dll from '{asset}': {e.Message}." +
                                   "scripts will probably not load correctly");
                }
            }

            // then find the prefab
            foreach (var asset in bundle.GetAllAssetNames()) {
                if (Path.GetExtension(asset) != ".prefab") continue;
                if (Path.GetFileNameWithoutExtension(asset) != name) continue;

                var gameObject = bundle.LoadAsset<GameObject>(asset);

                if (Serializer.GetSerializableComponents(gameObject).IsNullOrEmpty()) {
                    Debug.LogWarning($"prefab {gameObject} is not serializable");
                }

                return gameObject;
            }

            throw new ArgumentException($"given asset bundle {bundle.name} should have a prefab named {name}, but doesn't");
        }

        /// load all dlls, and find script with name
        public static Type LoadScript(this AssetBundle bundle, string name = MAIN_NAME) {
            Type retType = null!;

            foreach (var asset in bundle.GetAllAssetNames()) {
                if (Path.GetExtension(asset) != ".bytes") continue;

                var dllBytes = bundle.LoadAsset<TextAsset>(asset).bytes;
                try {
                    var dll = Assembly.Load(dllBytes);
                    if (retType != null) continue;

                    foreach (var type in dll.GetExportedTypes()) {
                        if (type.Name != name) continue;

                        if (!typeof(MonoBehaviour).IsAssignableFrom(type)) {
                            Debug.LogError($"script {type} is not a MonoBehaviour");
                        }

                        if (!typeof(SerialComponent).IsAssignableFrom(type)) {
                            Debug.LogWarning($"script {type} is not a SerialComponent");
                        }

                        retType = type;
                    }
                } catch (BadImageFormatException e) {
                    Debug.LogError($"could not load dll from '{asset}': {e.Message}." +
                                   "scripts will probably not load correctly");
                }
            }

            if (retType != null) {
                return retType;
            }

            throw new ArgumentException($"given asset bundle {bundle.name} should have script {name}, but doesn't");
        }
    }
}
