using System;
using System.Windows.Forms;
using UnityEngine;

namespace Distance.LevelMods {
    /// placeholder for custom objects that will be loaded in
    public class Proxy : SerialLevelEditorListener {
        public const ComponentID id = (ComponentID)1337;
        public override ComponentID ID_ => id;

        public const string displayName = "LevelMods Proxy";
        public override string DisplayName_ => displayName;
        public override string ComponentDescription_ => "Replaced with custom LevelMods object on load";

        private GameObject prefab = null!;

        public override void LevelEditorSpawned() {
            using var dialog = new OpenFileDialog {
                Title = "Choose AssetBundle to load"
            };
            if (dialog.ShowDialog() != DialogResult.OK) return;
            try {
                var bundle = AssetBundle.LoadFromFile(dialog.FileName);
                try {
                    prefab = bundle.LoadPrefab();
                } catch (ArgumentException) {
                    try {
                        var script = bundle.LoadScript();
                        prefab = new GameObject(script.Name, script);
                    } catch (ArgumentException) {
                        throw new ArgumentException($"can't load prefab or script form assetbundle at {dialog.FileName}");
                    }
                }
            } catch (Exception e) {
                Entry.log.Error($"error loading assetbundle at {dialog.FileName}: {e}");
            }
        }

        public override void Visit(IVisitor visitor, ISerializable prefabComp, int version) {
            var label = new NGUILabelInspector.Label("You should not be seeing this label", Colors.darkRed);
            visitor.VisitLabel("WarningLabel", ref label, true);
        }
    }
}
