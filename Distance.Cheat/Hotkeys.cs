using System.Text;
using UnityEngine;
using static Distance.Cheat.Entry;

namespace Distance.Cheat {
    public class Hotkeys : MonoBehaviour {
        private Cheats Cheats = null!;

        private void Awake() =>
            Cheats = GetComponent<Cheats>();

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
                if (Cheats.RaceEndLogic != null) {
                    Cheats.PlayerDataLocal.transform.position = Cheats.RaceEndLogic.transform.position;
                }
            }
        }
    }
}
