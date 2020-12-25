using System.Collections;
using JetBrains.Annotations;
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

        internal static bool IsFreshLevel;

        public void Initialize(IManager manager) {
            RuntimePatcher.AutoPatch();

            Events.Level.PostLoad.Subscribe(_ => IsFreshLevel = true);
        }

        private IEnumerator Start() {
            // go straight to main menu
            yield return new WaitForEndOfFrame();
            SceneManager.LoadScene("MainMenu");
        }

        [CanBeNull]
        internal static RaceEndLogic RaceEndLogic;

        /// when player car spawns for the first time in a level
        internal static void OnCarSpawn(LocalPlayerControlledCar car) {
            // update cache
            RaceEndLogic = FindObjectOfType<RaceEndLogic>();

            Utils.PrintObjects();

            // uh ohhhhh
            // foreach (var gameObject in FindObjectsOfType<GameObject>()) {
            //     foreach (var collider in gameObject.GetComponents<Collider>()) {
            //         collider.enabled = collider.isTrigger;
            //     }
            //
            //     if (gameObject.HasComponentInChildren<Camera>()) continue;
            //     gameObject.transform.position = Vector3.zero;
            //     gameObject.transform.rotation.SetLookRotation(Vector3.forward, Vector3.up);
            // }
        }
    }
}
