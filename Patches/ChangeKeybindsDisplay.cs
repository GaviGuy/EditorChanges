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

    [HarmonyPatch(typeof(TrackEditorLegendPanel), "AddBoundMappings")]
    public class ChangeKeybindsDisplay {

        // down twistLeft up twistRight / turnRight turnLeft
        static string rep = "" + Char.ToUpper((char)Patch.pitchDownKey.Value) + Char.ToUpper((char)Patch.twistLeftKey.Value)
            + Char.ToUpper((char)Patch.pitchUpKey.Value) + Char.ToUpper((char)Patch.twistRightKey.Value) + " / "
            + Char.ToUpper((char)Patch.turnRightKey.Value) + Char.ToUpper((char)Patch.turnLeftKey.Value);

        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {
            var codes = new List<CodeInstruction>(instructions);
            for (int i = 0; i < codes.Count; i++)
                if(codes[i].opcode == OpCodes.Ldstr && (string) codes[i].operand == "WASD / QE")
                    codes[i].operand = rep;
            return codes.AsEnumerable();
        }
    }
}