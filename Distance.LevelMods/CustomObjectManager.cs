using System.Collections.Generic;
using Serializers;
using UnityEngine;
using static Distance.LevelMods.Entry;

namespace Distance.LevelMods {
    public class CustomObjectManager : SerialComponent {
        public override ComponentID ID_ => (ComponentID)6969;

        public override bool AllowInspect_ => false;
        public override bool AllowCopyPaste_ => false;

        public static CustomObjectManager? instance;

        private void Awake() {
            log.Debug("awake");
            instance = this;
        }

        private void OnDestroy() {
            log.Debug("destroyed");
            instance = null;
        }


        public Dictionary<string, byte[]> datas = new();

        public override void Visit(IVisitor visitor, ISerializable prefabComp, int version) {
            log.Debug($"visiting as {visitor.GetType().Name}");
            switch (visitor) {
                case BinarySerializer serializer:
                    serializer.VisitDictionaryGeneric(nameof(datas), ref datas,
                        visitor.Visit, visitor.VisitArray);

                    foreach (var pair in datas)
                        log.Debug($"saved custom object {pair.Key}");

                    break;
                case BinaryDeserializer deserializer:
                    deserializer.VisitDictionaryGeneric(nameof(datas), ref datas,
                        visitor.Visit, visitor.VisitArray,
                        string.Empty, new byte[0]);

                    foreach (var pair in datas) {
                        log.Debug($"loading custom object {pair.Key}");

                        SerialComponent comp;
                        try {
                            comp = DataLoader.Load(pair.Value);
                        } catch (DataLoadException e) {
                            log.Exception(e);
                            continue;
                        }

                        Register(comp);
                    }

                    break;
            }
        }


        /// register custom object so it can be saved/loaded
        public static void Register(SerialComponent entryComp, byte[]? data = null) {
            var man = G.Sys.ResourceManager_!;
            man.LevelPrefabs_[entryComp.gameObject.name] = entryComp.gameObject;
            BinaryDeserializer.idToSerializableTypeMap_[entryComp.ID_] = entryComp.GetType();

            // var root = man.LevelPrefabFileInfosRoot_;
            // root.AddChildInfo(new LevelPrefabFileInfo(entryComp.gameObject.name, entryComp.gameObject, root));

            if (data == null) return;
            if (instance == null) {
                var le = G.Sys.LevelEditor_;
                var prefab = man.levelPrefabs_[nameof(CustomObjectManager)];
                var obj = le.CreateObject(prefab);

                var layer = le.WorkingLevel_.CreateAndInsertNewLayer(0, nameof(CustomObjectManager), false, true, false);
                ReferenceMap.Handle<GameObject> handle = default;
                le.AddGameObject(ref handle, obj, layer);
            }

            instance!.datas[entryComp.gameObject.name] = data;
        }
    }
}
