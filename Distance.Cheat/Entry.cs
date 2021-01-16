using System;
using System.Collections;
using Events.Level;
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

        /// not the best place to put this, but it works
        public static RaceEndLogic? End;

        public void Initialize(IManager manager) =>
            RuntimePatcher.AutoPatch();

        private void Awake() =>
            PostLoad.Subscribe(OnLevelPostLoad);

        private void OnDestroy() =>
            PostLoad.Unsubscribe(OnLevelPostLoad);

        private static void OnLevelPostLoad(PostLoad.Data data) =>
            End = FindObjectOfType<RaceEndLogic>();

        /// go straight to main menu
        private IEnumerator Start() {
            yield return new WaitForEndOfFrame();
            SceneManager.LoadScene("MainMenu");
        }
    }
}
