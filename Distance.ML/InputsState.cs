using System;
using UnityEngine;
using static Distance.ML.Entry;

namespace Distance.ML {
    /// stuff for handling car input
    public class InputsState : MonoBehaviour {
        public Action actions = Action.None;

        [Flags]
        public enum Action : ushort {
            None = 0,

            Gas = 1 << 0,
            Brake = 1 << 1,
            SteerLeft = 1 << 2,
            SteerRight = 1 << 3,
            Boost = 1 << 4,
            Jump = 1 << 5,
            Wings = 1 << 6,

            JetLeft = 1 << 7,
            JetRight = 1 << 8,
            JetDown = 1 << 9,
            JetUp = 1 << 10,
            Grip = 1 << 11,
        }

        private static InputStates currentStates {
            get => Utils.playerDataLocal!.InputStates_;
            set => Utils.playerDataLocal!.controlProfile_.states_ = value;
        }

        private InputStates otherStates = null!;
        public bool actionsOverriden;

        private void Awake() {
            otherStates = currentStates;
            currentStates = new InputStates();
        }

        private void Update() {
            // swap input between computer controlled and player controlled
            if (Input.GetKey(KeyCode.M) && Input.GetKeyDown(KeyCode.I)) {
                actionsOverriden = !actionsOverriden;

                var temp = currentStates;
                currentStates = otherStates;
                otherStates = temp;

                log.Debug($"action override {(actionsOverriden ? "on" : "off")}");
            }
        }

        /// update state stuff
        public void UpdateState() {
            if (actionsOverriden) return;

            void Set(InputAction action1, Action action2) {
                var value = (actions & action2) != 0;
                currentStates.states_[(int)action1] = new InputState {
                    value_ = value ? 1 : 0,
                    isPressed_ = value,
                    isTriggered_ = value,
                    isReleased_ = !value
                };
            }

            Set(InputAction.Gas, Action.Gas);
            Set(InputAction.Brake, Action.Brake);
            Set(InputAction.SteerLeft, Action.SteerLeft);
            Set(InputAction.WingYawLeft, Action.SteerLeft);
            Set(InputAction.SteerRight, Action.SteerRight);
            Set(InputAction.WingYawRight, Action.SteerRight);
            Set(InputAction.Boost, Action.Boost);
            Set(InputAction.Jump, Action.Jump);
            Set(InputAction.Wings, Action.Wings);

            Set(InputAction.JetRollLeft, Action.JetLeft);
            Set(InputAction.WingRollLeft, Action.JetLeft);
            Set(InputAction.JetRollRight, Action.JetRight);
            Set(InputAction.WingRollRight, Action.JetRight);
            Set(InputAction.JetPitchDown, Action.JetDown);
            Set(InputAction.WingPitchDown, Action.JetDown);
            Set(InputAction.JetPitchUp, Action.JetUp);
            Set(InputAction.WingPitchUp, Action.JetUp);

            Set(InputAction.Grip, Action.Grip);
        }
    }
}
