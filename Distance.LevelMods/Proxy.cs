using System.IO;
using System.Windows.Forms;

namespace Distance.LevelMods {
    /// placeholder for custom objects that will be loaded in
    public class Proxy : SerialLevelEditorListener {
        public override ComponentID ID_ => (ComponentID)1337;

        public override string DisplayName_ => "LevelMods Proxy";
        public override string ComponentDescription_ => "Replaced with custom LevelMods object on load";

        public override void LevelEditorSpawned() {
            using var dialog = new OpenFileDialog {
                Title = "Choose AssetBundle to load"
            };
            if (dialog.ShowDialog() != DialogResult.OK) return;
            var prefab = DataLoader.Load(File.ReadAllBytes(dialog.FileName), "Entry");
            Entry.log.Debug($"prefab is {prefab}");
            // Instantiate()
        }

        public override void Visit(IVisitor visitor, ISerializable prefabComp, int version) {
            var label = new NGUILabelInspector.Label("You should not be seeing this label", Colors.darkRed);
            visitor.VisitLabel("WarningLabel", ref label, true);
        }
    }
}
