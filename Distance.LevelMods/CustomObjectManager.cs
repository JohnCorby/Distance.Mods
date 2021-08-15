using System.Collections.Generic;
using Serializers;
using static Distance.LevelMods.Entry;

namespace Distance.LevelMods {
    /// this holds the data for all the custom objects.
    /// it will be serialized into the level on save
    public class CustomObjectManager : SerialComponent {
        public override ComponentID ID_ => (ComponentID)int.MinValue;

        public override bool AllowInspect_ => false;
        public override bool AllowCopyPaste_ => false;

        private void Awake() => log.Debug("awake");
        private void OnDestroy() => log.Debug("destroyed");


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

                        Register(comp, pair.Value);
                    }

                    break;
            }
        }

        /// find custom objects and add it to the datas list
        public void InitDatas(IEnumerable<CustomObject> customObjects) {
            datas.Clear();
            foreach (var customObject in customObjects)
                datas[customObject.name] = customObject.data;
        }


        /// register custom object so it can be saved/loaded
        public static void Register(SerialComponent entryComp, byte[] data) {
            entryComp.gameObject.AddComponent<CustomObject>().data = data;

            var man = G.Sys.ResourceManager_!;
            man.LevelPrefabs_[entryComp.name] = entryComp.gameObject;
            BinaryDeserializer.idToSerializableTypeMap_[entryComp.ID_] = entryComp.GetType();

            var root = man.LevelPrefabFileInfosRoot_;
            var info = new LevelPrefabFileInfo(entryComp.name, entryComp.gameObject, root);
            if (root.childInfos_.RemoveAll(info1 => info1.Name_ == info.Name_) > 0)
                G.Sys.LevelEditor_.DoFramesLater(2, () => G.Sys.LevelEditor_.SetToolText(
                    $"Updated prefab for custom object {entryComp.name}"));
            root.AddChildInfo(info);
        }
    }
}
