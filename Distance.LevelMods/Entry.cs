using Events.Level;
using Reactor.API.Attributes;
using Reactor.API.Interfaces.Systems;
using Reactor.API.Logging;
using Reactor.API.Runtime.Patching;
using Serializers;
using UnityEngine;

namespace Distance.LevelMods {
    [ModEntryPoint("com.github.johncorby/Distance.LevelMods")]
    public class Entry : MonoBehaviour {
        public static readonly Log LOG = LogManager.GetForCurrentAssembly();

        public void Initialize(IManager manager) {
            PostLoad.Subscribe(OnLevelPostLoad);

            BinaryDeserializer.idToSerializableTypeMap_.Add(Proxy.ID, typeof(Proxy));

            RuntimePatcher.AutoPatch();
        }

        private static void OnLevelPostLoad(PostLoad.Data data) {
            LOG.Debug($"level {data.level_.Name_} loaded");
        }
    }
}
