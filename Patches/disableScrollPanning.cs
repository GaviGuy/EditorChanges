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

    [HarmonyPatch(typeof(TrackEditorGUI), "CurrentInput.get")]
    public class disableScrollPanning {

        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {
            var codes = new List<CodeInstruction>(instructions);

            // 119, 115, 113, 101, 97, 100
            // W S Q E A D

            for (int i = 0; i < codes.Count; i++) {
                if (codes[i].opcode == OpCodes.Call && codes[ind].Calls(typeof(UnityEngine.Input).GetMethod("get_mouseScrollDelta"))) {
                    codes[i].opcode = OpCodes.Nop;
                    codes[i + 1].opcode = OpCodes.Ldc_I4_0;
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
            Patch.logger.LogInfo("Set " + (char)keycodes[i] + " to " + operations[i]);
            return codes.AsEnumerable();
        }
    }
}