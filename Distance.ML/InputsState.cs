using System;
using UnityEngine;
using static Distance.ML.Entry;

namespace Distance.ML {
    /// stuff for handling car input
    public class InputsState : MonoBehaviour {
        public Action Actions = Action.NONE;

        [Flags]
        public enum Action : ushort {
            NONE = 0,

            GAS = 1 << 0,
            BRAKE = 1 << 1,
            STEER_LEFT = 1 << 2,
            STEER_RIGHT = 1 << 3,
            BOOST = 1 << 4,
            JUMP = 1 << 5,
            WINGS = 1 << 6,

            JET_LEFT = 1 << 7,
            JET_RIGHT = 1 << 8,
            JET_DOWN = 1 << 9,
            JET_UP = 1 << 10,
            GRIP = 1 << 11,
        }

        private static InputStates CurrentStates {
            get => Utils.PlayerDataLocal!.InputStates_;
            set => Utils.PlayerDataLocal!.controlProfile_.states_ = value;
        }

        private InputStates OtherStates = null!;
        public bool ActionsOverriden;

        private void Awake() {
            OtherStates = CurrentStates;
            CurrentStates = new InputStates();
        }

        private void Update() {
            // swap input between computer controlled and player controlled
            if (Input.GetKey(KeyCode.M) && Input.GetKeyDown(KeyCode.I)) {
                ActionsOverriden = !ActionsOverriden;

                var temp = CurrentStates;
                CurrentStates = OtherStates;
                OtherStates = temp;

                LOG.Debug($"action override {(ActionsOverriden ? "on" : "off")}");
            }
        }

        /// update state stuff
        public void UpdateState() {
            if (ActionsOverriden) return;

            void Set(InputAction action1, Action action2) {
                var value = (Actions & action2) != 0;
                CurrentStates.states_[(int)action1] = new InputState {
                    value_ = value ? 1 : 0,
                    isPressed_ = value,
                    isTriggered_ = value,
                    isReleased_ = !value
                };
            }

            Set(InputAction.Gas, Action.GAS);
            Set(InputAction.Brake, Action.BRAKE);
            Set(InputAction.SteerLeft, Action.STEER_LEFT);
            Set(InputAction.WingYawLeft, Action.STEER_LEFT);
            Set(InputAction.SteerRight, Action.STEER_RIGHT);
            Set(InputAction.WingYawRight, Action.STEER_RIGHT);
            Set(InputAction.Boost, Action.BOOST);
            Set(InputAction.Jump, Action.JUMP);
            Set(InputAction.Wings, Action.WINGS);

            Set(InputAction.JetRollLeft, Action.JET_LEFT);
            Set(InputAction.WingRollLeft, Action.JET_LEFT);
            Set(InputAction.JetRollRight, Action.JET_RIGHT);
            Set(InputAction.WingRollRight, Action.JET_RIGHT);
            Set(InputAction.JetPitchDown, Action.JET_DOWN);
            Set(InputAction.WingPitchDown, Action.JET_DOWN);
            Set(InputAction.JetPitchUp, Action.JET_UP);
            Set(InputAction.WingPitchUp, Action.JET_UP);

            Set(InputAction.Grip, Action.GRIP);
        }
    }
}
