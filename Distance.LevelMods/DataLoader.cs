using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using UnityEngine;
using static Distance.LevelMods.Entry;

namespace Distance.LevelMods {
    public class DataLoadException : Exception {
        public DataLoadException(Exception e) : base($"error loading data: {e.Message}") => log.Exception(this);
    }

    public static class DataLoader {
        private const string entryName = "Entry";

        /// load data as
        /// dll,
        /// bundle with entry script, or
        /// bundle with entry prefab (that has entry script)
        public static SerialComponent Load(byte[] data) {
            try {
                // todo check is url???

                try {
                    return LoadFromDll(data);
                } catch (BadImageFormatException) {
                    log.Debug("data is not dll. trying to load as assetbundle");
                }

                return LoadFromBundle(data);
            } catch (Exception e) {
                throw new DataLoadException(e);
            }
        }


        /// treat data as dll, load entry script
        private static SerialComponent LoadFromDll(byte[] data) {
            var dll = Assembly.Load(data);
            var entryTypes = dll.GetExportedTypes().Where(type => type.Name == entryName).ToArray();

            var entryType = GetEntryType(entryTypes, $"dll {dll}");
            var entryPrefab = new GameObject(entryType.Namespace);
            return (SerialComponent)entryPrefab.AddComponent(entryType);
        }

        /// treat data as bundle, load dlls, find entry script.
        /// load entry prefab or create new
        private static SerialComponent LoadFromBundle(byte[] data) {
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
            } finally {
                bundle.Unload(false);
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
        private static SerialComponent GetEntryComp(AssetBundle bundle, Type entryType) {
            var asset = bundle.GetAllAssetNames()
                .SingleOrDefault(asset =>
                    asset.EndsWith(".prefab") &&
                    Path.GetFileNameWithoutExtension(asset) == entryType.Namespace?.ToLower());
            SerialComponent entryComp;
            if (asset == null) {
                log.Debug($"no entry prefab found in bundle {bundle.name}. making minimal one from entry type");
                var entryPrefab = new GameObject(entryType.Namespace);
                entryComp = (SerialComponent)entryPrefab.AddComponent(entryType);
            } else {
                var entryPrefab = bundle.LoadAsset<GameObject>(asset);
                entryComp = (SerialComponent)entryPrefab.GetComponent(entryType) ??
                            throw new Exception($"entry prefab {entryPrefab.name} in bundle {bundle.name} " +
                                                $"does not have entry component {entryType.FullName}");
            }

            // call init methods
            {
                const string name = "LoadAssets";
                try {
                    entryType.InvokeMember(name,
                        BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic |
                        BindingFlags.DeclaredOnly | BindingFlags.InvokeMethod,
                        null, entryComp, new object[] { bundle });
                } catch (MissingMethodException) {
                    log.Warning($"entry type {entryType.FullName} in bundle {bundle.name} " +
                                $"does not have {name}({nameof(AssetBundle)}) method");
                } catch (AmbiguousMatchException) {
                    log.Warning($"entry type {entryType.FullName} in bundle {bundle.name} " +
                                $"has multiple {name}({nameof(AssetBundle)}) methods");
                }
            }

            return entryComp;
        }
    }
}
