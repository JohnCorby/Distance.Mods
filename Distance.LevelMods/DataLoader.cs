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
        /// load data as
        /// dll,
        /// bundle with entry script, or
        /// bundle with entry prefab (that has entry script)
        public static GameObject? Load(byte[] data, string entryName) {
            try {
                // todo check is url???

                try {
                    return LoadFromDll(data, entryName);
                } catch (BadImageFormatException) {
                    log.Info("data is not dll. trying to load as assetbundle");
                }

                return LoadFromBundle(data, entryName);
            } catch (Exception e) {
                log.Error($"error loading data: {e.Message}");
                return null;
            }
        }


        /// treat data as dll, load entry script
        private static GameObject LoadFromDll(byte[] data, string entryName) {
            var dll = Assembly.Load(data);
            var entryTypes = dll.GetExportedTypes().Where(type => type.Name == entryName).ToArray();

            var entryType = GetEntryType(entryTypes, $"dll {dll}", entryName);
            return new GameObject(entryType.Namespace, entryType);
        }

        /// treat data as bundle, load dlls, find entry script.
        /// load entry prefab or create new
        private static GameObject LoadFromBundle(byte[] data, string entryName) {
            var bundle = AssetBundle.LoadFromMemory(data)!;
            try {
                var entryTypes = bundle.GetAllAssetNames()
                    .Where(asset => asset.EndsWith(".dll.bytes"))
                    .Select(asset => Assembly.Load(bundle.LoadAsset<TextAsset>(asset).bytes))
                    .SelectMany(dll => dll.GetExportedTypes())
                    .Where(type => type.Name == entryName)
                    .ToArray();

                var entryType = GetEntryType(entryTypes, $"bundle {bundle.name}", entryName);
                return GetEntryPrefab(bundle, entryType);
            } catch (Exception) {
                bundle.Unload(true);
                throw;
            }
        }


        /// check entry types, and then check and return single entry type
        private static Type GetEntryType(IList<Type> entryTypes, string container, string entryName) {
            switch (entryTypes.Count) {
                case < 1:
                    throw new InvalidOperationException($"{container} does not have entry type {entryName}");
                case > 1:
                    throw new InvalidOperationException($"{container} has multiple entry types " +
                                                        entryTypes.Join(type => type.FullName));
            }

            var entryType = entryTypes[0];

            if (!typeof(SerialComponent).IsAssignableFrom(entryType))
                throw new InvalidOperationException(
                    $"entry type {entryType.FullName} in {container} is not a {nameof(SerialComponent)}");
            return entryType;
        }

        /// get entry prefab from bundle
        /// or create minimal gameobject from entry type
        private static GameObject GetEntryPrefab(AssetBundle bundle, Type entryType) {
            var asset = bundle.GetAllAssetNames()
                .SingleOrDefault(asset =>
                    asset.EndsWith(".prefab") &&
                    Path.GetFileNameWithoutExtension(asset) == entryType.Namespace?.ToLower()
                );
            if (asset == null) {
                log.Info($"no preset found in bundle {bundle.name}. making minimal one from entry type");
                return new GameObject(entryType.Namespace, entryType);
            }

            var entryPrefab = bundle.LoadAsset<GameObject>(asset);

            var entryComp = entryPrefab.GetComponent(entryType) ??
                       throw new InvalidOperationException(
                           $"entry prefab {entryPrefab.name} in bundle {bundle.name} " +
                           $"does not have entry component {entryType.FullName}"
                       );

            // init bundle field
            {
                var fields = entryType.GetFields(BindingFlags.Instance | BindingFlags.NonPublic)
                    .Where(info => info.FieldType == typeof(AssetBundle)).ToArray();
                switch (fields.Length) {
                    case < 1:
                        log.Warning($"entry type {entryType.FullName} in bundle {bundle.name} " +
                                    "does not has assetbundle field to init");
                        return entryPrefab;
                    case > 1:
                        log.Warning($"entry type {entryType.FullName} in bundle {bundle.name} " +
                                    "has multiple assetbundle fields " +
                                    fields.Join(field => field.Name));
                        return entryPrefab;
                }

                var field = fields[0];
                field.SetValue(entryComp, bundle);
            }

            return entryPrefab;
        }
    }
}
