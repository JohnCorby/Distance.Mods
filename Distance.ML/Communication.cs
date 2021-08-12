using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using static Distance.ML.Entry;

namespace Distance.ML {
    /// reading/writing data via ipc (so loopback and port)
    public class Communication : MonoBehaviour {
        private static readonly IPEndPoint addr = new(IPAddress.Loopback, 6969);
        private Socket sock = null!;

        private State state = null!;

        private void Awake() {
            if (!G.Sys.GameManager_.SoloAndNotOnline_) {
                log.Error("must be in solo game");
                Destroy(this);
                return;
            }

            log.Info("connecting...");
            sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try {
                sock.Connect(addr);
                log.Info("connected!");
            } catch (SocketException e) {
                log.Error($"error connecting: {e}");
                Destroy(this);
                return;
            }

            state = gameObject.AddComponent<State>();
        }

        private void OnDestroy() {
            sock.Close();
            Destroy(state);
            log.Debug("communication destroyed");
        }

        private bool processStep, stepQueued;

        /// process incoming directions
        private void Update() {
            if (processStep && !stepQueued) {
                var isStep = BitConverter.ToBoolean(Recv(1), 0);
                if (isStep) DoStep();
                else DoReset();
            } else if (processStep && stepQueued) {
                stepQueued = false;
                DoStep();
            } else if (!processStep && !stepQueued && sock.Available > 0) {
                var isStep = BitConverter.ToBoolean(Recv(1), 0);
                if (isStep) stepQueued = true;
                else DoReset();
            }
        }

        private static void DoReset() {
            log.Debug("reset");
            G.Sys.GameManager_.RestartLevel();
        }

        private void DoStep() {
            log.Debug("step");

            // todo get action
            // State.InputsState.Actions = (InputsState.Action)BitConverter.ToUInt16(Recv(2), 0);
            state.inputsState.actions = InputsState.Action.SteerLeft;
            state.UpdateState();

            // send results
            Send(state.pointsState.bytes);
            Send(BitConverter.GetBytes(state.reward));
            Send(BitConverter.GetBytes(State.done));

            state.ResetState();
        }

        private void LateUpdate() {
            var data = Utils.playerDataLocal!;
            processStep = data.CarInputEnabled_ && data.IsCarRelevant_;
        }

        private void Send(byte[] data) {
            try {
                sock.Send(data);
            } catch (SocketException e) {
                log.Error($"send error: {e}");
                Destroy(this);
            }
        }

        private byte[] Recv(int size) {
            try {
                var data = new byte[size];
                sock.Receive(data);
                return data;
            } catch (SocketException e) {
                log.Error($"recv error: {e}");
                Destroy(this);
                return null!;
            }
        }
    }
}
