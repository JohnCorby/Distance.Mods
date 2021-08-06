using System.Text;
using UnityEngine;
using static Distance.ML.Entry;

namespace Distance.ML {
    /// stuff for handling car input
    public class InputsState : MonoBehaviour {
        public object Actions = null!;

        // private void Awake() {
        // throw new NotImplementedException();
        // }

        /// update state stuff
        public void UpdateState() {
            var states = Utils.PlayerDataLocal!.InputStates_;
            var s = new StringBuilder();
            for (var i = 0; i < states.states_.Length; i++) {
                s.AppendLine($"{(InputAction)i}: {states.states_[i]}");
            }
            LOG.Debug(s);
        }
    }
}
