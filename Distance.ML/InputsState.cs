using System.Linq;
using System.Text;
using UnityEngine;
using static Distance.ML.Entry;

namespace Distance.ML {
    /// stuff for handling car input
    public class InputsState : MonoBehaviour {
        public uint Actions;

        // private void Awake() {
        // throw new NotImplementedException();
        // }

        /// update state stuff
        public void UpdateState() {
            var states = Utils.PlayerDataLocal!.InputStates_;
            // override
            if (states.states_.Any(state => state.isPressed_)) return;

            static void Press(InputState state) {
                state.isPressed_ = true;
                state.isTriggered_ = true;
                state.isReleased_ = false;
                state.value_ = 1;
            }
            Press(states.GetState(InputAction.Jump));
        }
    }
}
