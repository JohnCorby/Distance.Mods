using System.Text;
using UnityEngine;
using static Distance.Cheat.Entry;

namespace Distance.Cheat {
    public class Hotkeys : MonoBehaviour {
        static Hotkeys() {
            Events.Car.Death.SubscribeAll((_, _) => {
                LOG.Info("CHEATS OFF");

                JetsGadget.thrusterBoostFullPowerLimit_ = 1f;
                JetsGadget.thrusterBoostDepletedLimit_ = 0.4f;
            });
        }

        private void Update() {
            if (!Input.GetKey(KeyCode.C)) return;

            if (Input.GetKeyDown(KeyCode.E)) { // enable cheats
                LOG.Info("CHEATS ON");

                if (Car) {
                    Car.invincible_ = true;
                    Car.minInstaRespawnTriggerTime_ = 0;
                    Car.minInstaRespawnTime_ = 0;

                    var boost = Car.GetComponent<BoostGadget>();
                    boost.accelerationMul_ = 1f * 3;
                    boost.heatUpRate_ = 0;

                    JetsGadget.thrusterBoostFullPowerLimit_ = 1f * 3;
                    JetsGadget.thrusterBoostDepletedLimit_ = 0.4f * 3;

                    foreach (var gadget in Car.GetComponents<Gadget>())
                        gadget.SetAbilityEnabled(true, false);

                }
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

            if (Input.GetKeyDown(KeyCode.End)) { // tp to end
                if (Car && End)
                    Car.transform.position = End.transform.position;
            }
        }
    }
}
