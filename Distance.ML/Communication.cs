using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using static Distance.ML.Entry;

namespace Distance.ML {
    /// reading/writing data via ipc (so loopback and port)
    public class Communication : MonoBehaviour {
        private static readonly IPEndPoint ADDR = new(IPAddress.Loopback, 6969);
        private Socket Sock = null!;

        private State State = null!;

        private void Awake() {
            if (!G.Sys.GameManager_.SoloAndNotOnline_) {
                LOG.Error("must be in solo game");
                Destroy(this);
                return;
            }

            LOG.Info("connecting...");
            Sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try {
                Sock.Connect(ADDR);
                LOG.Info("connected!");
            } catch (SocketException e) {
                LOG.Error($"error connecting: {e}");
                Destroy(this);
                return;
            }

            State = gameObject.AddComponent<State>();
        }

        private void OnDestroy() {
            Sock.Close();
            Destroy(State);
            LOG.Debug("communication destroyed");
        }

        private bool ProcessStep, StepQueued;

        /// process incoming directions
        private void Update() {
            if (ProcessStep && !StepQueued) {
                var isStep = BitConverter.ToBoolean(Recv(1), 0);
                if (isStep) DoStep();
                else DoReset();
            } else if (ProcessStep && StepQueued) {
                StepQueued = false;
                DoStep();
            } else if (!ProcessStep && !StepQueued && Sock.Available > 0) {
                var isStep = BitConverter.ToBoolean(Recv(1), 0);
                if (isStep) StepQueued = true;
                else DoReset();
            }
        }

        private static void DoReset() {
            LOG.Debug("reset");
            G.Sys.GameManager_.RestartLevel();
        }

        private void DoStep() {
            LOG.Debug("step");

            // todo get action
            State.UpdateState();

            // send results
            Send(State.PointsBytes);
            Send(BitConverter.GetBytes(State.Reward));
            Send(BitConverter.GetBytes(State.Done));

            State.ResetState();
        }

        private void LateUpdate() {
            var data = Utils.PlayerDataLocal!;
            ProcessStep = data.CarInputEnabled_ && data.IsCarRelevant_;
        }

        private void Send(byte[] data) {
            try {
                Sock.Send(data);
            } catch (SocketException e) {
                LOG.Error($"send error: {e}");
                Destroy(this);
            }
        }

        private byte[] Recv(int size) {
            try {
                var data = new byte[size];
                Sock.Receive(data);
                return data;
            } catch (SocketException e) {
                LOG.Error($"recv error: {e}");
                Destroy(this);
                return null!;
            }
        }
    }
}
