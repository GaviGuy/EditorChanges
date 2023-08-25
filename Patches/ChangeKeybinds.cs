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

        static private ConfigEntry<char> pitchUpKey,
            pitchDownKey,
            twistRightKey,
            twistLeftKey,
            turnRightKey,
            turnLeftKey;

        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {
            var codes = new List<CodeInstruction>(instructions);

            pitchDownKey = Config.Bind("General.Keybinds", "PitchDownKey", 'w', "Editor key to pitch down");
            pitchUpKey = Config.Bind("General.Keybinds", "PitchUpKey", 's', "Editor key to pitch up");
            twistLeftKey = Config.Bind("General.Keybinds", "twistLeftKey", 'a', "Editor key to twist left");
            twistRightKey = Config.Bind("General.Keybinds", "twistRightKey", 'd', "Editor key to twist right");
            turnLeftKey = Config.Bind("General.Keybinds", "turnLeftKey", 'e', "Editor key to turn left");
            turnRightKey = Config.Bind("General.Keybinds", "turnRightKey", 'q', "Editor key to turn right");

            int[] keycodes = {
                (int) pitchDownKey.Value,
                (int) pitchUpKey.Value,
                (int) turnRightKey.Value,
                (int) turnLeftKey.Value,
                (int) twistLeftKey.Value,
                (int) twistRightKey.Value
            };
            // 119, 115, 113, 101, 97, 100
            // W S Q E A D




            int startInd = 0,
                ind = -1;
            for (int i = 0; i < 6; i++) {
                for (int j = startInd; j < codes.Count; j++) {
                    if (codes[i].opcode == OpCodes.Call && codes[i].Calls("TrackEditorGUI::GetKeyHeld")) {
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
            }

            return codes.AsEnumerable();

            //the process is very simple
            //  look for calls to TrackEditorGUI::GetKeyHeld
            //  and replace the previous ldc.i4.s with the right value
            //  it seems to simply use unicode


        }
    }
}