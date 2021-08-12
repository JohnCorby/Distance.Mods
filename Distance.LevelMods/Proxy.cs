using System.Windows.Forms;

namespace Distance.LevelMods {
    /// placeholder for custom objects that will be loaded in
    public class Proxy : SerialLevelEditorListener {
        public int CoolValue = 1337;
        public string DefaultString = "";

        public const ComponentID ID = (ComponentID)1337;
        public override ComponentID ID_ => ID;

        public const string DISPLAY_NAME = "LevelMods Proxy";
        public override string DisplayName_ => DISPLAY_NAME;
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
            }, Options.Description.Format("Prompts you to open a file. Canceling will make nothing happen"));

            visitor.Visit("Cool Value", ref CoolValue, Options.Description.Format("this doesnt really do anything :P"));
            visitor.Visit("Default String", ref DefaultString,
                Options.Description.Format("this doesnt really do anything :P") +
                Options.DefaultString.Format("type something epic here :D"));
        }
    }
}
