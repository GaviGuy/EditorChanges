using BepInEx;
using BepInEx.Configuration;
using UnityEngine;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Linq;


namespace EditorChanges {

    [HarmonyPatch(typeof(TrackEditorGUI), "HandleTrackTurnEditorInput")]
    public class ChangeKeybinds {
        //passive
        //line 2156 in c#

        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {
            var codes = new List<CodeInstruction>(instructions);
            
            int[] keycodes = {
                (int) Patch.pitchDownKey.Value,
                (int) Patch.pitchUpKey.Value,
                (int) Patch.turnRightKey.Value,
                (int) Patch.turnLeftKey.Value,
                (int) Patch.twistLeftKey.Value,
                (int) Patch.twistRightKey.Value
            };
            // 119, 115, 113, 101, 97, 100
            // W S Q E A D

            int startInd = 0,
                ind;
            for (int i = 0; i < 6; i++) {
                for (ind = startInd; ind < codes.Count; ind++) {
                    if (codes[ind].opcode == OpCodes.Call && codes[ind].Calls(typeof(TrackEditorGUI).GetMethod("GetKeyHeld"))) {
                        //Patch.logger.LogWarning("ind: " + ind);
                        break;
                    }
                }
                startInd = ind + 1;
                for (; ind > startInd - 5; ind--) {
                    if (codes[ind].opcode == OpCodes.Ldc_I4_S) {
                        codes[ind].operand = keycodes[i];
                        break;
                    }
                }
                Patch.logger.LogInfo("Set " + keycodes[i] + " to operation " + i);
            }
            return codes.AsEnumerable();
        }
    }
}