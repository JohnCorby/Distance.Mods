using System.Text;
using UnityEngine;
using static Distance.Cheat.Entry;

namespace Distance.Cheat {
    public class Hotkeys : MonoBehaviour {
        private Cheats Cheats = null!;
        public RaceEndLogic? RaceEndLogic;

        private void Awake() {
            Cheats = GetComponent<Cheats>();
            RaceEndLogic = FindObjectOfType<RaceEndLogic>();
        }


        private void Update() {
            if (!Input.GetKey(KeyCode.F)) return;

            if (Input.GetKeyDown(KeyCode.E)) { // toggle cheats
                Cheats.ToggleCheats();
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
                LOG.Info(s);
            }

            if (Cheats.CheatsEnabled && Input.GetKeyDown(KeyCode.End)) { // tp to end
                if (RaceEndLogic != null) {
                    LOG.Info("teleporting to end");
                    Cheats.PlayerDataLocal.LocalCar_.transform.position = RaceEndLogic.transform.position;
                } else {
                    LOG.Info("cant tp to end because it is null (ie it wasnt found)");
                }
            }
        }
    }
}
