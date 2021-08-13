using System;
using System.IO;
using System.Windows.Forms;
using UnityEngine;
using static Distance.LevelMods.Entry;

namespace Distance.LevelMods {
    /// placeholder for custom objects that will be loaded in
    public class Proxy : SerialLevelEditorListener {
        public override ComponentID ID_ => (ComponentID)1337;

        public override string DisplayName_ => "LevelMods Proxy";
        public override string ComponentDescription_ => "Replaced with custom LevelMods object on load";

        public override void OnLevelEditorEnter() {
            log.Debug("level editor enter");

            if (data != null) {
                entryComp = DataLoader.Load(data);
            }
        }

        public override void LevelEditorSpawned() {
            LoadPrefab();
        }

        public byte[]? data;
        public SerialComponent? entryComp;

        public override void Visit(IVisitor visitor, ISerializable prefabComp, int version) {
            var label = new NGUILabelInspector.Label("This is all test stuff lol", Options.warningColor_);
            visitor.VisitLabel("Warning Label", ref label, true);

            visitor.VisitAction("Load Prefab", LoadPrefab, true,
                Options.Description.Format("Load custom object into this proxy"));

            visitor.VisitReference("Entry Component", entryComp, null);
        }

        private void LoadPrefab() {
            using var dialog = new OpenFileDialog {
                Title = "Choose AssetBundle to load"
            };
            if (dialog.ShowDialog() != DialogResult.OK) return;
            data = File.ReadAllBytes(dialog.FileName);
            entryComp = DataLoader.Load(data);
            log.Debug($"entry component is {entryComp}");
            // Instantiate()
        }
    }
}
