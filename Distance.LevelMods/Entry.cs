using Events.LevelEditor;
using Reactor.API.Attributes;
using Reactor.API.Interfaces.Systems;
using Reactor.API.Logging;
using Reactor.API.Runtime.Patching;
using UnityEngine;

namespace Distance.LevelMods {
    [ModEntryPoint("com.github.johncorby/Distance.LevelMods")]
    public class Entry : MonoBehaviour {
        public static readonly Log log = LogManager.GetForCurrentAssembly();

        public void Initialize(IManager manager) {
            ControlSchemeChanged.Subscribe(_ => {
                // register hotkeys for custom tools
                G.Sys.LevelEditor_.AddEventToRegisteredHotKeys(InputEvent.Create("ctrl+shift+o"),
                    LoadCustomObjectTool.info.Name_);
            });


            RuntimePatcher.AutoPatch();
        }
    }
}
