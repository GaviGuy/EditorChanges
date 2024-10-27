using BepInEx;
using UnityEngine;
using HarmonyLib;
using System;


namespace EditorChanges {

    [HarmonyPatch(typeof(ClipInfo), "GetTickTimeSignatureMarkerWouldInsert")]
    public class FixTimeSigPlacement {
        //passive
        static int Postfix(int __result, ClipInfo __instance, out int outputTick, int __1) {
            outputTick = __1;
            if (__result >= 0) {
                var markers = __instance.timeSignatureMarkers;
                for(int i = markers.Length - 1; i >= 0; i--) {
                    if (outputTick > markers[i].startingTick) {
                        outputTick += (int) (Math.Round((double) markers[0].startingTick / markers[i].BeatsPerBar) * markers[i].BeatsPerBar);
                        break;
                    }
                }
            }
            return __result;
        }
    }
}