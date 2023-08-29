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
    // it doesn't like CurrentInput.get
    //   but what else do I call it?!
    public class DisableScrollPanning {

        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {
            var codes = new List<CodeInstruction>(instructions);

            for (int i = 0; i < codes.Count; i++) {
                if (codes[i].opcode == OpCodes.Call && codes[i].Calls(typeof(UnityEngine.Input).GetMethod("get_mouseScrollDelta"))) {
                    codes[i].opcode = OpCodes.Ldc_I4_0;
                    codes[i + 1].opcode = OpCodes.Nop;
                    Patch.logger.LogInfo("removed scroll input");
                    break;
                }
            }
            return codes.AsEnumerable();
        }
    }
}