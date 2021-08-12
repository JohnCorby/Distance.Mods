using Reactor.API.Attributes;
using Reactor.API.Interfaces.Systems;
using Reactor.API.Logging;
using Reactor.API.Runtime.Patching;
using UnityEngine;

namespace Distance.ML {
    [ModEntryPoint("com.github.johncorby/Distance.ML")]
    public class Entry : MonoBehaviour {
        public static readonly Log log = LogManager.GetForCurrentAssembly();
        public void Initialize(IManager manager) => RuntimePatcher.AutoPatch();
    }
}
