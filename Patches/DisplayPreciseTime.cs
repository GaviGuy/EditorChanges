using System.Collections.Generic;
using System.Reflection.Emit;
using BepInEx.Logging;
using HarmonyLib;

namespace EditorChanges
{
    public static class DisplayPreciseTime
    {
        [HarmonyPatch(typeof(TrackEditorGUI), "UpdateTrackInfoDetailsPanel")]
        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> ChangeStringTimeFormat(IEnumerable<CodeInstruction> instructions)
        {
            var code = new List<CodeInstruction>(instructions);

            for (int i = 0; i < code.Count; i++)
            {
                if (code[i].opcode == OpCodes.Ldstr && (string) code[i].operand == "0.00")
                {
                    code[i].operand = "0.0000";
                    break;
                }
            }
            
            Patch.logger.LogInfo("Transpiled time!");

            return code;
        }
    }
}