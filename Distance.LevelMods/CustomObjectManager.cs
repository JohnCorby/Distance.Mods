using System;
using Serializers;
using UnityEngine;
using static Distance.LevelMods.Entry;

namespace Distance.LevelMods {
    public class CustomObjectManager : SerialComponent {
        public override ComponentID ID_ => (ComponentID)6969;

        public override bool AllowInspect_ => false;
        public override bool AllowCopyPaste_ => false;

        public static CustomObjectManager? instance;
        private void Awake() => instance = this;
        private void OnDestroy() => instance = null;

        /// register custom object so it can be saved/loaded
        public static void Register(SerialComponent entryComp) {
            var man = G.Sys.ResourceManager_!;
            man.LevelPrefabs_[entryComp.gameObject.name] = entryComp.gameObject;
            BinaryDeserializer.idToSerializableTypeMap_[entryComp.ID_] = entryComp.GetType();

            // var root = man.LevelPrefabFileInfosRoot_;
            // root.AddChildInfo(new LevelPrefabFileInfo(entryComp.gameObject.name, entryComp.gameObject, root));

            // todo make it work on loads too, using instance or something
        }
    }
}
