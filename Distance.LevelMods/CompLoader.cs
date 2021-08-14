using System;
using System.IO;
using System.Windows.Forms;
using LevelEditorActions;
using LevelEditorTools;
using UnityEngine;
using static Distance.LevelMods.Entry;

namespace Distance.LevelMods {
    public class CompLoader : SerialLevelEditorListener {
        public override ComponentID ID_ => (ComponentID)420;

        public override string DisplayName_ => $"Custom {nameof(SerialComponent)} Loader";
        public override string ComponentDescription_ => $"Used to load and register a custom {nameof(SerialComponent)}";
        public override bool AllowInspect_ => false;
        public override bool AllowCopyPaste_ => false;

        public override void LevelEditorSpawned() {
            LoadCustomObject();
            // lol
            this.DoFramesLater(10, () => G.Sys.LevelEditor_.DeleteGameObject(gameObject));
        }

        private static void LoadCustomObject() {
            using var dialog = new OpenFileDialog {
                Title = "Choose AssetBundle or DLL to load"
            };
            if (dialog.ShowDialog() != DialogResult.OK) return;
            var data = File.ReadAllBytes(dialog.FileName);

            SerialComponent comp;
            try {
                comp = DataLoader.Load(data);
            } catch (DataLoadException e) {
                LevelEditorTool.PrintMessage(e.Message);
                G.Sys.MenuPanelManager_.Clear();
                G.Sys.MenuPanelManager_.ShowError(e.Message, e.GetType().ToString());
                return;
            }

            // todo register

            var info = new LevelPrefabFileInfo(comp.DisplayName_, comp.gameObject, null);
            G.Sys.LevelEditor_.StartToolJob(new CreateObjectByCursorTool(info));
        }
    }
}
