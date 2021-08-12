using System;
using System.Windows.Forms;
using UnityEngine;

namespace Distance.LevelMods {
    /// placeholder for custom objects that will be loaded in
    public class Proxy : SerialLevelEditorListener {
        public const ComponentID ID = (ComponentID)1337;
        public override ComponentID ID_ => ID;

        public const string DISPLAY_NAME = "LevelMods Proxy";
        public override string DisplayName_ => DISPLAY_NAME;
        public override string ComponentDescription_ => "Replaced with custom LevelMods object on load";

        private GameObject Prefab = null!;

        public override void LevelEditorSpawned() {
            using var dialog = new OpenFileDialog {
                Title = "Choose AssetBundle to load"
            };
            if (dialog.ShowDialog() != DialogResult.OK) return;
            try {
                var bundle = AssetBundle.LoadFromFile(dialog.FileName);
                try {
                    Prefab = bundle.LoadPrefab();
                } catch (ArgumentException) {
                    try {
                        var script = bundle.LoadScript();
                        Prefab = new GameObject(script.Name, script);
                    } catch (ArgumentException) {
                        throw new ArgumentException($"can't load prefab or script form assetbundle at {dialog.FileName}");
                    }
                }
            } catch (Exception e) {
                Entry.LOG.Error($"error loading assetbundle at {dialog.FileName}: {e}");
            }
        }

        public override void Visit(IVisitor visitor, ISerializable prefabComp, int version) {
            var label = new NGUILabelInspector.Label("You should not be seeing this label", Colors.darkRed);
            visitor.VisitLabel("WarningLabel", ref label, true);
        }
    }
}
