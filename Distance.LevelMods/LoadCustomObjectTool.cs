using System.IO;
using System.Windows.Forms;
using LevelEditorTools;

namespace Distance.LevelMods {
    public class LoadCustomObjectTool : InstantTool {
        public static readonly ToolInfo info =
            new("Load Custom Object", "Load a custom object from an assetbundle or dll.",
                ToolCategory.Others, ToolButtonState.Button, false);
        public override ToolInfo Info_ => info;

        public override bool Run() {
            using var dialog = new OpenFileDialog {
                Title = "Choose AssetBundle or DLL to load"
            };
            if (dialog.ShowDialog() != DialogResult.OK) return false;
            var data = File.ReadAllBytes(dialog.FileName);

            SerialComponent comp;
            try {
                comp = DataLoader.Load(data);
            } catch (DataLoadException e) {
                G.Sys.MenuPanelManager_.Clear();
                G.Sys.MenuPanelManager_.ShowError(e.Message, e.GetType().Name);
                return false;
            }

            CustomObjectManager.Register(comp, data);

            var info = new LevelPrefabFileInfo(comp.gameObject.name, comp.gameObject, null);
            G.Sys.LevelEditor_.StartToolNextFrame(new CreateObjectByCursorTool(info));
            return true;
        }
    }
}
