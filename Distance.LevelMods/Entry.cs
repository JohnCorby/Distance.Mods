﻿using Events.Level;
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
            ControlSchemeChanged.Subscribe(_ =>
                // register hotkeys for custom tools
                G.Sys.LevelEditor_.AddEventToRegisteredHotKeys(InputEvent.Create("ctrl+shift+o"),
                    LoadCustomObjectTool.info.Name_));

            PostLoad.Subscribe(data => {
                // remove custom object manager when a level loads
                var layer = data.level_.GetLayer(nameof(CustomObjectManager));
                if (layer != null) data.level_.TryDeleteLayer(layer, true);
            });

            RuntimePatcher.AutoPatch();
        }
    }
}
