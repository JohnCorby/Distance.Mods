using Reactor.API.Attributes;
using Reactor.API.Interfaces.Systems;
using Reactor.API.Logging;
using Reactor.API.Runtime.Patching;
using UnityEngine;

namespace Distance.Raycast {
    [ModEntryPoint("com.github.johncorby/Raycast")]
    public class Entry : MonoBehaviour {
        public static readonly Log LOG = LogManager.GetForCurrentAssembly();

        public static Communication Communication;
        public static Stepper Stepper;

        public void Initialize(IManager manager) {
            RuntimePatcher.AutoPatch();
        }

        private void Awake() {
            Communication = gameObject.AddComponent<Communication>();
            Stepper = gameObject.AddComponent<Stepper>();
        }
    }
}
