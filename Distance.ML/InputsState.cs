using System.Linq;
using UnityEngine;

namespace Distance.ML {
    /// stuff for handling car input
    public class InputsState : MonoBehaviour {
        public uint Actions;

        private InputStates ComputerStates = null!;
        private InputStates PlayerStates = null!;

        private void Awake() {
            ComputerStates = new InputStates();
            PlayerStates = Utils.PlayerDataLocal!.InputStates_;
            Utils.PlayerDataLocal.controlProfile_.states_ = ComputerStates;
        }

        /// update state stuff
        public void UpdateState() {
            // override if there's player input
            Utils.PlayerDataLocal!.controlProfile_.states_ =
                PlayerStates.states_.Any(state => state.isPressed_)
                    ? PlayerStates
                    : ComputerStates;

            void Press(InputAction action) {
                ComputerStates.states_[(int)action] = new InputState {
                    value_ = 1,
                    isPressed_ = true,
                    isTriggered_ = true,
                    isReleased_ = false
                };
            }

            Press(InputAction.Jump);
        }
    }
}
