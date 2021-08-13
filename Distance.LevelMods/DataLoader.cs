using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using UnityEngine;
using static Distance.LevelMods.Entry;

namespace Distance.LevelMods {
    public static class DataLoader {
        private const string entryName = "Entry";

        /// load data as
        /// dll,
        /// bundle with entry script, or
        /// bundle with entry prefab (that has entry script)
        public static Component? Load(byte[] data) {
            try {
                // todo check is url???

                try {
                    return LoadFromDll(data);
                } catch (BadImageFormatException) {
                    log.Info("data is not dll. trying to load as assetbundle");
                }

                return LoadFromBundle(data);
            } catch (Exception e) {
                log.Error($"error loading data: {e.Message}");
                return null;
            }
        }


        /// treat data as dll, load entry script
        private static Component LoadFromDll(byte[] data) {
            var dll = Assembly.Load(data);
            var entryTypes = dll.GetExportedTypes().Where(type => type.Name == entryName).ToArray();

            var entryType = GetEntryType(entryTypes, $"dll {dll}");
            var entryPrefab = new GameObject(entryType.Namespace);
            return entryPrefab.AddComponent(entryType);
        }

        /// treat data as bundle, load dlls, find entry script.
        /// load entry prefab or create new
        private static Component LoadFromBundle(byte[] data) {
            var bundle = AssetBundle.LoadFromMemory(data)!;
            try {
                var entryTypes = bundle.GetAllAssetNames()
                    .Where(asset => asset.EndsWith(".dll.bytes"))
                    .Select(asset => Assembly.Load(bundle.LoadAsset<TextAsset>(asset).bytes))
                    .SelectMany(dll => dll.GetExportedTypes())
                    .Where(type => type.Name == entryName)
                    .ToArray();

                var entryType = GetEntryType(entryTypes, $"bundle {bundle.name}");
                return GetEntryComp(bundle, entryType);
            } catch (Exception) {
                bundle.Unload(true);
                throw;
            }
        }


        /// check entry types, and then check and return single entry type
        private static Type GetEntryType(IList<Type> entryTypes, string container) {
            switch (entryTypes.Count) {
                case < 1:
                    throw new Exception($"{container} does not have entry type {entryName}");
                case > 1:
                    throw new Exception($"{container} has multiple entry types " +
                                        entryTypes.Join(type => type.FullName));
            }

            var entryType = entryTypes[0];

            if (!typeof(SerialComponent).IsAssignableFrom(entryType))
                throw new Exception(
                    $"entry type {entryType.FullName} in {container} is not a {nameof(SerialComponent)}");
            return entryType;
        }

        /// get entry comp from bundle
        /// by loading entry prefab
        /// or making minimal gameobject from entry type
        private static Component GetEntryComp(AssetBundle bundle, Type entryType) {
            var asset = bundle.GetAllAssetNames()
                .SingleOrDefault(asset =>
                    asset.EndsWith(".prefab") &&
                    Path.GetFileNameWithoutExtension(asset) == entryType.Namespace?.ToLower());
            Component entryComp;
            if (asset == null) {
                log.Info($"no entry prefab found in bundle {bundle.name}. making minimal one from entry type");
                var entryPrefab = new GameObject(entryType.Namespace);
                entryComp = entryPrefab.AddComponent(entryType);
            } else {
                var entryPrefab = bundle.LoadAsset<GameObject>(asset);
                entryComp = entryPrefab.GetComponent(entryType) ??
                            throw new Exception($"entry prefab {entryPrefab.name} in bundle {bundle.name} " +
                                                $"does not have entry component {entryType.FullName}");
            }

            // init bundle field
            {
                var fields = entryType.GetFields(BindingFlags.Instance | BindingFlags.NonPublic)
                    .Where(info => info.FieldType == typeof(AssetBundle)).ToArray();
                switch (fields.Length) {
                    case < 1:
                        log.Warning($"entry type {entryType.FullName} in bundle {bundle.name} " +
                                    "does not has assetbundle field to init");
                        return entryComp;
                    case > 1:
                        log.Warning($"entry type {entryType.FullName} in bundle {bundle.name} " +
                                    "has multiple assetbundle fields " +
                                    fields.Join(field => field.Name));
                        return entryComp;
                }

                var field = fields[0];
                field.SetValue(entryComp, bundle);
            }

            return entryComp;
        }
    }
}
