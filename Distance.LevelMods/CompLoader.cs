using static Distance.LevelMods.Entry;

namespace Distance.LevelMods {
    public class CompLoader : SerialLevelEditorListener {
        public override ComponentID ID_ => (ComponentID)420;

        public override string DisplayName_ => $"Custom {nameof(SerialComponent)} Loader";
        public override string ComponentDescription_ => $"Used to load and register a custom {nameof(SerialComponent)}";

        public override void LevelEditorSpawned() {
            log.Debug("we are spawned in the level editor");
        }
    }
}
