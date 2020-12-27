using UnityEngine;

namespace Distance.Raycast {
    /// allows you to advance frame by frame
    public class Stepper : MonoBehaviour {
        private bool DoStep;

        private void Update() {
            if (DoStep) {
                Time.timeScale = 0;
                DoStep = false;
            }

            if (Input.GetKeyDown(KeyCode.K)) Time.timeScale = Time.timeScale == 0 ? 1 : 0;

            if (Input.GetKeyDown(KeyCode.L)) {
                Time.timeScale = 1;
                DoStep = true;
            }
        }
    }
}
