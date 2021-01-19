using System.Collections;
using Reactor.API.Attributes;
using Reactor.API.Interfaces.Systems;
using Reactor.API.Logging;
using Reactor.API.Runtime.Patching;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Distance.Cheat {
    [ModEntryPoint("com.github.johncorby/Cheat")]
    public class Entry : MonoBehaviour {
        public static readonly Log LOG = LogManager.GetForCurrentAssembly();

        public void Initialize(IManager manager) =>
            RuntimePatcher.AutoPatch();

        /// go straight to main menu
        private IEnumerator Start() {
            yield return new WaitForEndOfFrame();
            SceneManager.LoadScene("MainMenu");
        }
    }
}
