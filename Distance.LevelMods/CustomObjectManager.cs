using System.Collections.Generic;
using Serializers;
using UnityEngine;

namespace Distance.LevelMods {
    public class CustomObjectManager : SerialComponent {
        public override ComponentID ID_ => (ComponentID)6969;

        public override bool AllowInspect_ => false;
        public override bool AllowCopyPaste_ => false;

        public static CustomObjectManager? instance;
        private void Awake() => instance = this;
        private void OnDestroy() => instance = null;


        public Dictionary<string, byte[]> datas = null!;

        public override void Visit(IVisitor visitor, ISerializable prefabComp, int version) {
            switch (visitor) {
                case Serializer serializer:
                    serializer.VisitDictionaryGeneric(nameof(datas), ref datas,
                        (string name, ref string val, string options) => visitor.Visit(name, ref val, options),
                        (string name, ref byte[] val, string options) =>
                            visitor.VisitArray(name, ref val, options));
                    break;
                case BinaryDeserializer deserializer:
                    deserializer.VisitDictionaryGeneric(nameof(datas), ref datas,
                        (string name, ref string val, string options) => visitor.Visit(name, ref val, options),
                        (string name, ref byte[] val, string options) => visitor.VisitArray(name, ref val, options),
                        string.Empty, new byte[0]);
                    break;
            }
        }


        /// register custom object so it can be saved/loaded
        public static void Register(SerialComponent entryComp, byte[] data) {
            var man = G.Sys.ResourceManager_!;
            man.LevelPrefabs_[entryComp.gameObject.name] = entryComp.gameObject;
            BinaryDeserializer.idToSerializableTypeMap_[entryComp.ID_] = entryComp.GetType();

            // var root = man.LevelPrefabFileInfosRoot_;
            // root.AddChildInfo(new LevelPrefabFileInfo(entryComp.gameObject.name, entryComp.gameObject, root));

            if (instance == null) {
                var le = G.Sys.LevelEditor_;
                var prefab = man.levelPrefabs_[nameof(CustomObjectManager)];
                var obj = le.CreateObject(prefab);
                ReferenceMap.Handle<GameObject> handle = default;
                le.AddGameObject(ref handle, obj, le.WorkingLevel_.ActiveLayer_);
            }

            instance!.datas[entryComp.gameObject.name] = data;
        }
    }
}
