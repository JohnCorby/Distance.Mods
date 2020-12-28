using System;
using UnityEngine;

namespace Distance.ML {
    /// shoots out a bunch of rays and collects the hit info for them
    /// also renders that info
    [Obsolete("todo remove")]
    public class Raycaster : MonoBehaviour {
        private const float X_SCALE = 1, Y_SCALE = 1;
        private const int NUM_XS = 10, NUM_YS = 10;
        private const float MAX_DISTANCE = 100;

        private readonly RaycastHit[,] Casts = new RaycastHit[NUM_YS, NUM_XS];
        private readonly LineRenderer[,] Lines = new LineRenderer[NUM_YS, NUM_XS];

        private void Awake() {
            for (var j = 0; j < NUM_YS; j++) {
                for (var i = 0; i < NUM_XS; i++) {
                    var go = new GameObject();
                    go.transform.SetParent(transform, false);
                    var line = go.AddComponent<LineRenderer>();
                    line.material = new Material(Shader.Find("Sprites/Default"));
                    line.numPositions = 2;
                    line.startWidth = .1f;
                    line.endWidth = line.startWidth;
                    Lines[j, i] = line;
                }
            }
        }

        private void Update() {
            for (var j = 0; j < NUM_YS; j++) {
                for (var i = 0; i < NUM_XS; i++) {
                    var y = (j - NUM_YS / 2) * Y_SCALE / NUM_YS;
                    var x = (i - NUM_XS / 2) * X_SCALE / NUM_XS;
                    DoCast(j, i, transform.forward + transform.up * y + transform.right * x);
                }
            }
        }

        private void DoCast(int j, int i, Vector3 direction) {
            var didHit = Physics.Raycast(transform.position, direction, out var hit,
                MAX_DISTANCE, PhysicsEx.NoCarsRayCastLayerMask_);

            // if (didHit) {
            //     Entry.LOG.Info($"ray[{j},{i}] hit {hit.collider.gameObject.name} at dist of {hit.distance}");
            // }
            Casts[j, i] = hit;

            var line = Lines[j, i];
            var color = didHit ? new Color(0, 1, 0, .1f) : new Color(1, 0, 0, .1f);
            line.startColor = color;
            line.startColor = color;
            line.SetPositions(new[] {
                transform.position,
                transform.position + direction * MAX_DISTANCE
            });
            // Debug.DrawRay(transform.position, direction * hit.distance, color);
        }
    }
}
