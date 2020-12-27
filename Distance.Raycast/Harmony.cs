using System;
using HarmonyLib;
using UnityEngine;

namespace Distance.Raycast {
    // namespace LocalPlayerControlledCar_ {
    //     [HarmonyPatch(typeof(LocalPlayerControlledCar), nameof(Awake))]
    //     public static class Awake {
    //         private static void Postfix(Component __instance) {
    //             var go = new GameObject();
    //             go.transform.SetParent(__instance.transform, false);
    //             go.transform.localPosition = Vector3.up;
    //             go.AddComponent<Raycaster>();
    //         }
    //     }
    // }

    namespace CarCamera_ {
        [HarmonyPatch(typeof(CarCamera), nameof(Awake))]
        public static class Awake {
            private static void Postfix(Component __instance) {
                __instance.gameObject.AddComponent<DepthCamera>();
            }
        }
    }
}
