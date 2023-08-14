using BepInEx;
using UnityEngine;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Linq;

namespace EditorChanges {

    [HarmonyPatch(typeof(TrackEditorGUI), "MoveNotesInLateralDirection")]
    public class RemoveLaneLimit {
        //passive
        //TODO: allow toggle via a key
        //TODO: patch TrackEditorInfoPanel to show whether toggled (4, 12, 128?)
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {
            var codes = new List<CodeInstruction>(instructions);

            int nearInd = -1;
            for (int i = 0; i < codes.Count; i++) {
                if (codes[i].opcode == OpCodes.Div && codes[i + 1].opcode == OpCodes.Sub) {
                    nearInd = i;
                    break;
                }
            }
            if (nearInd < 0) {
                //Logger.LogError("failed to patch Lane Limit 1");
                return instructions;
            }
            //codes.RemoveRange(sizeInd - 2, 7);

            int negInd = -1;
            for (int i = nearInd; i < codes.Count; i++) {
                if (codes[i].opcode == OpCodes.Neg) {
                    negInd = i;
                    break;
                }
            }
            if (negInd < 0) {
                //Logger.LogError("failed to patch Lane Limit 2");
                return instructions;
            }
            codes[negInd - 1].opcode = OpCodes.Ldc_I4_S;
            codes[negInd - 1].operand = -128;
            codes[negInd + 1].opcode = OpCodes.Ldc_I4_S;
            codes[negInd + 1].operand = 127;
            codes.RemoveAt(negInd);

            //Logger.LogInfo("Transpilation successful!");

            return codes.AsEnumerable();
        }
    }
}
