using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using Serializers;
using UnityEngine;
using static Distance.LevelMods.Entry;

namespace Distance.LevelMods {
    public static class AssetBundleLoader {
        public class Error : SystemException {
            public enum Type {
                NOT_A_DLL,
                NOT_A_BUNDLE,
                NO_ENTRY_TYPE,
                MULTIPLE_ENTRY_TYPES,
                NO_ENTRY_PREFAB
            }

            public Type Type;

            public Error(Type type) {
                this.type = type;
            }
        }

        /// the name that we look for as the main thing to load
        private const string ENTRY_NAME = "Entry";

        /// treat data as dll, load entry script
        public static GameObject LoadScriptFromDll(byte[] data, string entryName = ENTRY_NAME) {
            Assembly dll;
            try {
                dll = Assembly.Load(data);
            } catch (BadImageFormatException) {
                throw new NotDll();
            }

            var entryTypes = dll.GetExportedTypes().Where(type => type.Name == entryName).ToArray();
            switch (entryTypes.Length) {
                case < 1:
                    throw new InvalidOperationException($"dll {dll} does not have entry type {entryName}");
                case > 1:
                    throw new InvalidOperationException($"dll {dll} has multiple entry types " +
                                                        entryTypes.Join(type => type.FullName));
            }

            var entryType = entryTypes[0];

            if (!typeof(MonoBehaviour).IsAssignableFrom(entryType)) {
                LOG.Error($"script {entryType.FullName} in dll {dll} is not a MonoBehaviour");
            }

            if (!typeof(SerialComponent).IsAssignableFrom(entryType)) {
                LOG.Warning($"script {entryType.FullName} in dll {dll} is not a SerialComponent");
            }


            return new GameObject(entryType.FullName, entryType);
        }

        /// treat data as bundle, load dlls, find entry script
        public static GameObject LoadScriptFromBundle(byte[] data, string entryName = ENTRY_NAME) {
            var bundle = AssetBundle.LoadFromMemory(data) ??
                         throw new BadImageFormatException("data not an assetbundle");

            var entryTypes = bundle.GetAllAssetNames()
                .Where(asset => asset.EndsWith(".dll.bytes"))
                .Select(asset => Assembly.Load(bundle.LoadAsset<TextAsset>(asset).bytes))
                .SelectMany(dll => dll.GetExportedTypes())
                .Where(type => type.Name == entryName)
                .ToArray();
            switch (entryTypes.Length) {
                case < 1:
                    throw new InvalidOperationException($"bundle {bundle.name} does not have entry type {entryName}");
                case > 1:
                    throw new InvalidOperationException($"bundle {bundle.name} has multiple entry types " +
                                                        entryTypes.Join(type1 => type1.FullName));
            }

            var entryType = entryTypes[0];

            if (!typeof(MonoBehaviour).IsAssignableFrom(entryType)) {
                LOG.Error($"entry type {entryType} in bundle {bundle} is not a MonoBehaviour");
            }

            if (!typeof(SerialComponent).IsAssignableFrom(entryType)) {
                LOG.Warning($"entry type {entryType} in bundle {bundle} is not a SerialComponent");
            }

            return new GameObject(entryType.FullName, entryType);
        }

        /// treat data as bundle, load dlls, find entry script, find entry prefab
        public static GameObject LoadPrefab(byte[] data, string entryName = ENTRY_NAME) {
            var bundle = AssetBundle.LoadFromMemory(data) ??
                         throw new BadImageFormatException("data not an assetbundle");

            // get entry type
            var entryTypes = bundle.GetAllAssetNames()
                .Where(asset => asset.EndsWith(".dll.bytes"))
                .Select(asset => Assembly.Load(bundle.LoadAsset<TextAsset>(asset).bytes))
                .SelectMany(dll => dll.GetExportedTypes())
                .Where(type => type.Name == entryName)
                .ToArray();
            switch (entryTypes.Length) {
                case < 1:
                    throw new InvalidOperationException($"bundle {bundle.name} does not have entry type {entryName}");
                case > 1:
                    throw new InvalidOperationException($"bundle {bundle.name} has multiple entry types " +
                                                        entryTypes.Join(type1 => type1.FullName));
            }

            var entryType = entryTypes[0];

            if (!typeof(MonoBehaviour).IsAssignableFrom(entryType)) {
                LOG.Error($"entry type {entryType.FullName} in bundle {bundle.name} is not a MonoBehaviour");
            }

            if (!typeof(SerialComponent).IsAssignableFrom(entryType)) {
                LOG.Warning($"entry type {entryType.FullName} in bundle {bundle.name} is not a SerialComponent");
            }

            // then load the prefab
            var asset =
                bundle.GetAllAssetNames().SingleOrDefault(asset1 =>
                    Path.GetExtension(asset1) == ".prefab" &&
                    Path.GetFileNameWithoutExtension(asset1) == entryType.FullName) ??
                throw new InvalidOperationException(
                    $"bundle {bundle.name} does not have entry prefab {entryType.FullName}");
            var entryPrefab = bundle.LoadAsset<GameObject>(asset);

            if (!entryPrefab.GetComponent(entryType)) {
                LOG.Error(
                    $"entry prefab {entryPrefab} in bundle {bundle.name} does not have entry component {entryType.FullName}");
            }

            throw new ArgumentException(
                $"bundle {bundle.name} should have a prefab {entryName}, but doesn't");
        }
    }
}
