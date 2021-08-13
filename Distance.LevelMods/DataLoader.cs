using System;
using System.IO;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using UnityEngine;
using static Distance.LevelMods.Entry;

namespace Distance.LevelMods {
    public static class DataLoader {
        private class TryNext : Exception {
            internal TryNext(Exception e) : base(e.Message, e) { }
        }

        public static GameObject Load(byte[] data, string entryName) {
            // try script from dll
            try {
                return LoadScriptFromDll(data, entryName);
            } catch (TryNext e) {
                log.Error(e.Message);
            }

            // not a dll, try prefab
            try {
                return LoadPrefab(data, entryName);
            } catch (TryNext e) {
                log.Error(e.Message);
            }

            // bundle but no prefab, try script
            try {
                return LoadScriptFromBundle(data, entryName);
            } catch (TryNext e) {
                log.Error(e.Message);
            }

            throw new InvalidOperationException("cant load data as dll, " +
                                                "bundle with script, " +
                                                "or bundle with prefab");
        }

        /// treat data as dll, load entry script
        private static GameObject LoadScriptFromDll(byte[] data, string entryName) {
            Assembly dll;
            try {
                dll = Assembly.Load(data);
            } catch (BadImageFormatException) {
                throw new TryNext(new BadImageFormatException("data not a dll"));
            }

            var entryTypes = dll.GetExportedTypes().Where(type => type.Name == entryName).ToArray();
            var entryType = GetEntryType(entryTypes, $"dll {dll}", entryName);

            return new GameObject(entryType.FullName, entryType);
        }

        /// get entry types from bundle
        private static Type[] GetEntryTypes(AssetBundle bundle, string entryName) => bundle.GetAllAssetNames()
            .Where(asset => asset.EndsWith(".dll.bytes"))
            .Select(asset => Assembly.Load(bundle.LoadAsset<TextAsset>(asset).bytes))
            .SelectMany(dll => dll.GetExportedTypes())
            .Where(type => type.Name == entryName)
            .ToArray();

        /// get entry type from many types and check it
        private static Type GetEntryType(Type[] entryTypes, string container, string entryName) {
            switch (entryTypes.Length) {
                case < 1:
                    throw new InvalidOperationException($"{container} does not have entry type {entryName}");
                case > 1:
                    throw new InvalidOperationException($"{container} has multiple entry types " +
                                                        entryTypes.Join(type1 => type1.FullName));
            }

            var entryType = entryTypes[0];

            if (!typeof(MonoBehaviour).IsAssignableFrom(entryType))
                log.Error($"entry type {entryType.FullName} in {container} is not a MonoBehaviour");
            if (!typeof(SerialComponent).IsAssignableFrom(entryType))
                log.Warning($"entry type {entryType.FullName} in {container} is not a SerialComponent");
            return entryType;
        }

        /// init prefab by setting asset bundle field, if possible
        private static GameObject InitEntryPrefab(GameObject entryPrefab, AssetBundle bundle, Type entryType) {
            var comp = entryPrefab.GetComponent(entryType) ??
                       throw new InvalidOperationException($"entry prefab {entryPrefab.name} in bundle {bundle.name} " +
                                                           $"does not have entry component {entryType.FullName}");
            var fields = entryType.GetFields(BindingFlags.Instance | BindingFlags.NonPublic)
                .Where(info => info.FieldType == typeof(AssetBundle)).ToArray();
            switch (fields.Length) {
                case < 1:
                    log.Warning($"entry type {entryType.FullName} in bundle {bundle.name} " +
                                "does not has assetbundle field to init");
                    break;
                case > 1:
                    log.Warning($"entry type {entryType.FullName} in bundle {bundle.name} " +
                                "has multiple assetbundle fields " +
                                fields.Join(field => field.Name));
                    break;
            }

            var field = fields[0];
            field.SetValue(comp, bundle);

            return entryPrefab;
        }


        /// treat data as bundle, load dlls, find entry script
        private static GameObject LoadScriptFromBundle(byte[] data, string entryName) {
            var bundle = AssetBundle.LoadFromMemory(data) ??
                         throw new TryNext(new BadImageFormatException("data not an assetbundle"));

            var entryTypes = GetEntryTypes(bundle, entryName);
            var entryType = GetEntryType(entryTypes, $"bundle {bundle.name}", entryName);
            return InitEntryPrefab(new GameObject(entryType.FullName, entryType), bundle, entryType);
        }

        /// treat data as bundle, load dlls, find entry script, find entry prefab
        private static GameObject LoadPrefab(byte[] data, string entryName) {
            var bundle = AssetBundle.LoadFromMemory(data) ??
                         throw new TryNext(new BadImageFormatException("data not an assetbundle"));

            var entryTypes = GetEntryTypes(bundle, entryName);
            var entryType = GetEntryType(entryTypes, $"bundle {bundle.name}", entryName);

            var entryPrefabAsset =
                bundle.GetAllAssetNames().SingleOrDefault(asset1 =>
                    Path.GetExtension(asset1) == ".prefab" &&
                    Path.GetFileNameWithoutExtension(asset1) == entryType.FullName) ??
                throw new TryNext(new InvalidOperationException(
                    $"bundle {bundle.name} does not have entry prefab {entryType.FullName}"));

            return InitEntryPrefab(bundle.LoadAsset<GameObject>(entryPrefabAsset), bundle, entryType);
        }
    }
}
