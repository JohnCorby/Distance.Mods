using System;
using Serializers;
using UnityEngine;
using static Distance.LevelMods.Entry;

namespace Distance.LevelMods {
    public class CompManager : SerialLevelEditorListener {
        public override ComponentID ID_ => (ComponentID)6969;

        public override string DisplayName_ => $"Custom {nameof(SerialComponent)} Manager";
        public override string ComponentDescription_ => $"Used to manage custom {nameof(SerialComponent)}s in the level";

        public static CompManager? instance;
        private void Awake() => instance = this;
        private void OnDestroy() => instance = null;

        public override void LevelEditorSpawned() {
            log.Debug("we are spawned in the level editor");
        }

        /// register custom object so it can be saved/loaded
        public static void Register(GameObject prefab, Type type) {
            if (!type.IsAssignableFrom(typeof(SerialComponent)))
                throw new ArgumentException($"type {type.FullName} is not a {nameof(SerialComponent)}");

            var comp = (SerialComponent)prefab.GetComponent(type) ??
                       throw new NullReferenceException($"no component of type {type.FullName} on prefab {prefab.name}");

            prefab.name = comp.DisplayName_;

            var man = G.Sys.ResourceManager_!;
            man.LevelPrefabs_.Add(prefab.name, prefab);

            BinaryDeserializer.idToSerializableTypeMap_.Add(comp.ID_, comp.GetType());

            // var root = man.LevelPrefabFileInfosRoot_;
            // root.AddChildInfo(new LevelPrefabFileInfo(prefab.name, prefab, root));
        }
    }
}
