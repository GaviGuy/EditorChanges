using BepInEx;
using BepInEx.Logging;
using UnityEngine;
using HarmonyLib;
using SSD.Utility;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Linq;

namespace EditorChanges {

    [HarmonyPatch(typeof(TrackEditorGUI), "HandleNoteEditorInput")]
    public class InvisColorSwap {
        static ManualLogSource Logger = Patch.logger;
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {
            //Logger.LogInfo("InvisColorSwap: entered transpiler");
            var codes = new List<CodeInstruction>(instructions);

            int ind = -1;
            for (int i = 1; i < codes.Count; i++) {
                if (codes[i].opcode == OpCodes.Conv_U1) {
                    ind = i - 4;
                    break;
                }
            }
            if (ind < 0) {
                Logger.LogError("InvisColorSwap: failed to locate signature");
                return instructions;
            }

            codes.RemoveRange(ind, 4);

            CodeInstruction[] adds = {
                // it's quite shrimple actually. fuck that old noise, just bitwise XOR with 1 lmao
                new CodeInstruction(OpCodes.Ldc_I4_1), // push 1
                new CodeInstruction(OpCodes.Xor) // xor
            };

            codes.InsertRange(ind, adds);
            

            //Logger.LogInfo("InvisColorSwap: Transpilation successful!");

            return codes.AsEnumerable();
        }
    }
}
