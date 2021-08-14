using System;
using Serializers;
using UnityEngine;
using static Distance.LevelMods.Entry;

namespace Distance.LevelMods {
    public class CompManager : SerialComponent {
        public override ComponentID ID_ => (ComponentID)6969;

        public override string DisplayName_ => $"Custom {nameof(SerialComponent)} Manager";
        public override string ComponentDescription_ => $"Used to manage custom {nameof(SerialComponent)}s in the level";
        public override bool AllowInspect_ => false;
        public override bool AllowCopyPaste_ => false;

        public static CompManager? instance;
        private void Awake() => instance = this;
        private void OnDestroy() => instance = null;

        /// register custom object so it can be saved/loaded
        public static void Register(SerialComponent comp) {
            comp.gameObject.name = comp.DisplayName_;

            var man = G.Sys.ResourceManager_!;
            man.LevelPrefabs_[comp.DisplayName_] = comp.gameObject;
            BinaryDeserializer.idToSerializableTypeMap_[comp.ID_] = comp.GetType();

            // var root = man.LevelPrefabFileInfosRoot_;
            // root.AddChildInfo(new LevelPrefabFileInfo(comp.DisplayName_, comp.gameObject, root));
        }
    }
}
