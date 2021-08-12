using System.Text;
using UnityEngine;
using static Distance.Cheat.Entry;

namespace Distance.Cheat {
    public class Hotkeys : MonoBehaviour {
        private Cheats cheats = null!;
        public RaceEndLogic? raceEndLogic;

        private void Awake() {
            cheats = GetComponent<Cheats>();
            raceEndLogic = FindObjectOfType<RaceEndLogic>();
        }


        private void Update() {
            if (!Input.GetKey(KeyCode.F)) return;

            if (Input.GetKeyDown(KeyCode.E)) { // toggle cheats
                cheats.ToggleCheats();
            }

            if (Input.GetKeyDown(KeyCode.D)) { // dump objects
                var s = new StringBuilder();
                s.AppendLine("############# BEGIN DumpObjects #############");

                foreach (var gameObject in FindObjectsOfType<GameObject>()) {
                    s.AppendLine(gameObject.name);
                    foreach (var component in gameObject.GetComponents<Component>())
                        s.AppendLine("\t" + component.GetType());
                }

                s.AppendLine("############# END DumpObjects #############");
                log.Info(s);
            }

            if (cheats.cheatsEnabled && Input.GetKeyDown(KeyCode.End)) { // tp to end
                if (raceEndLogic != null) {
                    log.Info("teleporting to end");
                    cheats.playerDataLocal.LocalCar_.transform.position = raceEndLogic.transform.position;
                } else {
                    log.Info("cant tp to end because it is null (ie it wasnt found)");
                }
            }
        }
    }
}
