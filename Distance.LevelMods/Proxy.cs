namespace Distance.LevelMods {
    /// placeholder for custom objects that will be loaded in
    public class Proxy : SerialLevelEditorListener {
        public override ComponentID ID_ => (ComponentID)1337;

        public override string DisplayName_ => "LevelMods Proxy";
        public override string ComponentDescription_ => "Replaced with custom LevelMods object on load";

        public override void Visit(IVisitor visitor, ISerializable prefabComp, int version) {
            var label = NGUILabelInspector.Label.CreateDefault("this is an epic label!!!");
            visitor.VisitLabel("Label", ref label, true);
        }
    }
}
