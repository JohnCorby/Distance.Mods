using System.Windows.Forms;

namespace Distance.LevelMods {
    /// placeholder for custom objects that will be loaded in
    public class Proxy : SerialLevelEditorListener {
        public const ComponentID ID = (ComponentID)1337;
        public override ComponentID ID_ => ID;

        public override string DisplayName_ => "LevelMods Proxy";
        public override string ComponentDescription_ => "Replaced with custom LevelMods object on load";

        public override void Visit(IVisitor visitor, ISerializable prefabComp, int version) {
            var label = NGUILabelInspector.Label.CreateDefault("this is an epic label!!!");
            visitor.VisitLabel("Label", ref label, true);

            visitor.VisitAction("Open File", () => {
                using var dialog = new OpenFileDialog();
                if (dialog.ShowDialog() != DialogResult.OK) return;
                // using var stream = dialog.OpenFile();
                // using var reader = new StreamReader(stream);
                // var bytes = reader.ReadToEnd();
                print($"got file {dialog.FileName}!!!!");
            });
        }
    }
}
