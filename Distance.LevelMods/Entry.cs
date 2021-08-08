using System.IO;
using Events.Level;
using Reactor.API.Attributes;
using Reactor.API.Interfaces.Systems;
using Reactor.API.Logging;
using Reactor.API.Storage;
using UnityEngine;

namespace Distance.LevelMods {
    [ModEntryPoint("com.github.johncorby/Distance.LevelMods")]
    public class Entry : MonoBehaviour {
        public static readonly Log LOG = LogManager.GetForCurrentAssembly();

        public static string ModsFolder = null!;

        public void Initialize(IManager manager) {
            var fileSystem = new FileSystem();
            ModsFolder = Path.Combine(fileSystem.RootDirectory, "Mods");

            PostLoad.Subscribe(OnLevelPostLoad);
        }

        private static void OnLevelPostLoad(PostLoad.Data data) {
            LOG.Debug($"level {data.level_.Name_} loaded");
        }
    }
}
