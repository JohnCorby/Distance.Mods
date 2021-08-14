using System.IO;
using System.Windows.Forms;

namespace Distance.LevelMods {
    /// placeholder for custom objects that will be loaded in
    public class Proxy : SerialLevelEditorListener {
        public override ComponentID ID_ => (ComponentID)1337;

        public override string DisplayName_ => "LevelMods Proxy";
        public override string ComponentDescription_ => "Replaced with custom LevelMods object on load";

        public override void LevelEditorSpawned() {
            LoadPrefab();
        }

        public byte[]? data;
        private SerialComponent? entryComp;

        public override void Visit(IVisitor visitor, ISerializable prefabComp, int version) {
            {
                var label = new NGUILabelInspector.Label("This is all test stuff lol", Options.warningColor_);
                visitor.VisitLabel("Warning Label", ref label, true);
            }

            {
                var label1 = NGUILabelInspector.Label.CreateDefault($"entry comp = {entryComp}");
                visitor.VisitLabel("Info Label 1", ref label1, true);
                var label2 = NGUILabelInspector.Label.CreateDefault($"data len = {data?.Length}");
                visitor.VisitLabel("Info Label 2", ref label2, true);
            }

            visitor.VisitAction("Load Prefab", LoadPrefab, true,
                Options.Description.Format("Load custom object into this proxy"));

            // visitor.VisitReference("Entry Component", entryComp, null);
            visitor.VisitArray("Data", ref data,
                Options.Description.Format("Raw bytes of the loaded bundle"));

            entryComp ??= DataLoader.Load(data);
        }

        private void LoadPrefab() {
            using var dialog = new OpenFileDialog {
                Title = "Choose AssetBundle to load"
            };
            if (dialog.ShowDialog() != DialogResult.OK) return;
            data = File.ReadAllBytes(dialog.FileName);
            // Instantiate()
        }
    }
}
