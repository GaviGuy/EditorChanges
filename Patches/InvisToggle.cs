using BepInEx;
using BepInEx.Logging;
using UnityEngine;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Linq;

namespace EditorChanges {

    [HarmonyPriority(Priority.High)]
    [HarmonyPatch(typeof(TrackEditorGUI), "HandleNoteEditorInput")]
    public class InvisToggle {
        static ManualLogSource Logger = Patch.logger;

        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator iL) {
            var codes = new List<CodeInstruction>(instructions);
            Logger.LogInfo("InvisToggle: entered transpiler");

            //from mew's speenphone:
            {
                /*
                [HarmonyPatch(typeof(Game), nameof(Game.Update)), HarmonyPostfix]
                private static void Game_Update_Postfix()
                {
                    if (Input.GetKeyDown(KeyCode.F12))
                    {
                        enableCustomSounds = !enableCustomSounds;
                        NotificationSystemGUI.AddMessage("Custom sounds are " + (enableCustomSounds ? "ON" : "OFF"));
                        SetClips();
                    }
                }
                */
            }

            //1. find a way to add a new InputMapping.SpinCommands for invisToggle
            //2. steal most of the actual logic from ChangeSelectedNoteColor


            //find the start & end of the section for changeSelectedNoteColor
            int startInd = -1, endInd = -1;
            for (int i = 0; i < codes.Count; i++) {
                if (codes[i].opcode == OpCodes.Ldc_I4
                        && (int)codes[i].operand == 1040) {
                    startInd = i;
                    break;
                }
            }
            for (int i = startInd + 1; i < codes.Count; i++) {
                if (codes[i].opcode == OpCodes.Ldc_I4
                        && (int)codes[i].operand > 1000) {
                    endInd = i;
                    break;
                }
            }
            if (startInd < 0 || endInd < 0) {
                //cry about it (need to fix my logger)
                Logger.LogInfo("InvisToggle: failed to locate changeSelectedNoteColor block");
                return instructions;
            }

            Logger.LogInfo("InvisToggle: start: " + startInd + ", end: " + endInd);
            //make a copy of the block for color swapping
            var newCodes = new List<CodeInstruction>();
            for (int i = startInd; i < endInd; i++) {
                newCodes.Add(new CodeInstruction(codes[i]));
            }

            //find the signature of the part that modifies colorIndex
            int guhInd = -1;
            for (int i = 0; i < newCodes.Count - 1; i++) {
                if (newCodes[i].opcode == OpCodes.Ldc_I4_2
                        && newCodes[i + 1].opcode == OpCodes.Call) {
                    guhInd = i;
                    break;
                }
            }

            if (guhInd < 0) {
                Logger.LogInfo("InvisToggle: failed to locate colorIndex changer (guhInd = " + guhInd + ")");
                Logger.LogInfo("InvisToggle: (newCodes[" + newCodes.Count + "]");
                return instructions;
            }

            //kill cut
            int count = 0;
            for (int i = 0; i < codes.Count; i++) {
                if (codes[i].opcode == OpCodes.Ldc_I4
                            && (int)codes[i].operand == 1107) {
                    codes[i].operand = 9996;
                    count++;
                }
            }

            Logger.LogInfo("InvisToggle: removed " + count + " instances of cut");


            //modify so it swaps vis instead of color
            //  dup push2 cgt push4 mul push2 sub add
            //  ie. if 2 is greater than color, add 2. Else, subtract 2
            /*
            CodeInstruction[] changes = {
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Ldc_I4_2),
                new CodeInstruction(OpCodes.Cgt),
                new CodeInstruction(OpCodes.Ldc_I4_4),
                new CodeInstruction(OpCodes.Mul),
                new CodeInstruction(OpCodes.Ldc_I4_2),
                new CodeInstruction(OpCodes.Sub),
                new CodeInstruction(OpCodes.Add)
            };

            newCodes.RemoveRange(guhInd - 2, 4);
            newCodes.InsertRange(guhInd - 2, changes);
            */

            //modify the call to look for editorCut's enum instead of changeSelectedNoteColor's enum
            for (int i = 0; i < newCodes.Count; i++) {
                if (newCodes[i].opcode == OpCodes.Ldc_I4
                            && (int)newCodes[i].operand > 1000) {
                    newCodes[i].operand = 1107;
                    Logger.LogInfo("InvisToggle: successfully subbed in new enum");
                    break;
                }
            }

            //modify labels and branches

            //fuck it. let's do this the old way.
            //1. remove all labels
            //2. add each operand and destination one at a time

            for (int i = 0; i < newCodes.Count; i++) {
                newCodes[i].labels.Clear();
            }

            Label newLabel = iL.DefineLabel();
            Label[] newLabels = {
                iL.DefineLabel(),
                iL.DefineLabel(),
                iL.DefineLabel(),
                iL.DefineLabel(),
                iL.DefineLabel(),
                iL.DefineLabel(),
                iL.DefineLabel(),
                iL.DefineLabel()
            };
            int[] labelNums = { 0, 1, 2, 3, 1, 1, 1, 4, 5, 6, 7 };

            //set branch operands
            int step = 0;
            for (int i = 0; i < newCodes.Count; i++) {
                if (newCodes[i].Branches(out _)
                       || newCodes[i].opcode == OpCodes.Leave_S) {
                    newCodes[i].operand = newLabels[labelNums[step]];
                    step++;
                }
            }
            Logger.LogInfo("InvisToggle: successfully replaced operands");

            //set labels
            //this is disgusting, I know, but I'm out of ideas
            CodeInstruction append = new CodeInstruction(OpCodes.Nop);

            append.labels.Add(newLabels[0]);
            newCodes.Add(append);
            Logger.LogInfo("InvisToggle: label 0");

            //IL_0703
            for (int i = 1; ; i++) {
                if (newCodes[i + 1].opcode == OpCodes.Call
                        && newCodes[i + 1].Calls(typeof(SelectionRanges.SelectionEnumerator).GetMethod("MoveNext"))) {
                    newCodes[i].labels.Add(newLabels[1]);
                    Logger.LogInfo("InvisToggle: label 1");
                    break;
                }
            }
            //IL_06B7
            for (int i = 1; ; i++) {
                if (newCodes[i - 1].Branches(out _)
                        && newCodes[i].opcode == OpCodes.Ldsfld
                        && newCodes[i + 1].IsLdloc()
                        && newCodes[i + 2].opcode == OpCodes.Callvirt) {
                    newCodes[i].labels.Add(newLabels[2]);
                    Logger.LogInfo("InvisToggle: label 2");
                    break;
                }
            }
            //IL_06C6
            for (int i = 1; ; i++) {
                if (newCodes[i + 1].opcode == OpCodes.Call
                        && newCodes[i + 1].Calls(typeof(Note).GetMethod("get_IsSectionContinuation")))  {
                    newCodes[i].labels.Add(newLabels[3]);
                    Logger.LogInfo("InvisToggle: label 3");
                    break;
                }
            }
            //IL_0691
            for (int i = 1; ; i++) {
                if (newCodes[i + 1].opcode == OpCodes.Call
                        && newCodes[i + 1].Calls(typeof(SelectionRanges.SelectionEnumerator).GetMethod("get_Current"))) {
                    newCodes[i].labels.Add(newLabels[4]);
                    Logger.LogInfo("InvisToggle: label 4");
                    break;
                }
            }
            //IL_074F
            for (int i = 1; ; i++) {
                if (newCodes[i - 1].opcode == OpCodes.Callvirt
                        && newCodes[i - 1].Calls(typeof(PlayableTrackData).GetMethod("SetNote"))) {
                    newCodes[i].labels.Add(newLabels[5]);
                    Logger.LogInfo("InvisToggle: label 5");
                    break;
                }
            }
            //IL_071A
            for (int i = 1; ; i++) {
                if (newCodes[i + 1].opcode == OpCodes.Call
                        && !(newCodes[i + 1].Calls(typeof(SelectionRanges.SelectionEnumerator).GetMethod("get_Current")))
                        && newCodes[i + 5].opcode == OpCodes.Callvirt
                        && newCodes[i + 5].Calls(typeof(PlayableNoteData).GetMethod("GetNote"))) {
                    newCodes[i].labels.Add(newLabels[6]);
                    Logger.LogInfo("InvisToggle: label 6");
                    break;
                }
            }
            //IL_0768
            for (int i = 1; ; i++) {
                if (newCodes[i - 1].opcode == OpCodes.Endfinally) {
                    newCodes[i].labels.Add(newLabels[7]);
                    Logger.LogInfo("InvisToggle: label 7");
                    break;
                }
            }
            Logger.LogInfo("InvisToggle: successfully located and placed labels");

            //insert this new block somewhere
            for (int i = 0; i < codes.Count; i++) {
                if (codes[i].opcode == OpCodes.Ldc_I4
                        && (int)codes[i].operand == 1084) {
                    endInd = i - 1;
                    break;
                }
            }
            codes.InsertRange(endInd, newCodes);

            //let's cheat and hardcode an enum for SpinCommands. something in the 1000s
            //1704

            //now I have the a ability to add it into HandleNoteEditorInput

            //but I still need a way to bind it to a key (preferably configable)
            //or at the very least, send a signal for the 'hard'coded value


            Logger.LogInfo("InvisToggle: Transpilation successful!");

            return codes.AsEnumerable();
        }
    }
}
