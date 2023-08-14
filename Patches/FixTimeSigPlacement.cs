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
                //Logger.LogInfo("Original outputTick: " + __1);
                var firstMarker = __instance.timeSignatureMarkers[0];
                outputTick += firstMarker.startingTick;
                //Logger.LogInfo("New outoutTick: " + outputTick);
            }
            //else Logger.LogInfo("No new timeSigMarker added.");
            return __result;
        }
    }
}