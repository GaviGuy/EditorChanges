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
            Logger.LogInfo("InvisColorSwap: entered transpiler");
            var codes = new List<CodeInstruction>(instructions);

            /*
             * V_57 is the address in the hash set
             * V_58 is noteIndex5
             * V_52 is note8 ?
             * V_59 is note8 ??
             * 
             */

            // adds  1 to the note's  color
            // calls repeat(2) on it
            //convert  to int8, then int32
            // stores it back in colorIndex

            //how to fix
                //step 1: check if colorIndex > 1
                //step 2: if yes, store 2. otherwise, 0
                //step 3: after repeat(), add stored number

                //push 1
                //cgt
                //push 2
                //mul
                //  this is the number we will later add

            //signature to check for
                //ldc.i4.2
                //call
                    //MathUtils::Repeat

            int ind = -1;
            for (int i = 1; i < codes.Count; i++) {
                if (codes[i].opcode == OpCodes.Ldc_I4_2 
                        && codes[i - 1].opcode == OpCodes.Add
                        && codes[i + 1].opcode == OpCodes.Call) {
                    ind = i;
                    break;
                }
            }
            if (ind < 0) {
                //Logger.LogError("failed to patch InvisColorSwap");
                return instructions;
            }

            //after add: dup ldc.i4.1 cgt ldc.i4.2 mul (STORE)
            //after call: (RETRIEVE) add

            //after add: dup
            //after call: dup
            //everything must resolve before stfld

            //new plan: kill repeat(). if it's exactly 2, make it 0
            //remove ldc.i4.2
            //remove call
            //after add: dup, push2, ceq, push2, mul, sub (ie. if it's 2, subtract 2)
            //           dup, push3, cgt, push2, mul, sub (ie. if it's >3, subtract 2)
            
            codes.RemoveRange(ind, 2);

            CodeInstruction[] adds = {
                // color+1 at top of stack
                new CodeInstruction(OpCodes.Dup), // dup color
                new CodeInstruction(OpCodes.Ldc_I4_2), // push 2
                new CodeInstruction(OpCodes.Ceq), // if color == 2: 1, else: 0
                new CodeInstruction(OpCodes.Ldc_I4_2), // push 2
                new CodeInstruction(OpCodes.Mul), // mul (if color == 2: 2, else: 0)
                new CodeInstruction(OpCodes.Neg), // neg
                new CodeInstruction(OpCodes.Add), // add -(0/2) to color
                // color+1 (-2) at top of stack
                new CodeInstruction(OpCodes.Dup), // dup newColor
                new CodeInstruction(OpCodes.Ldc_I4_3),  // push 3
                new CodeInstruction(OpCodes.Clt), // if 3 < newColor 1, else 0
                new CodeInstruction(OpCodes.Ldc_I4_2),  // push 2
                new CodeInstruction(OpCodes.Mul), // mul (2 if newColor > 3, else 0)
                new CodeInstruction(OpCodes.Neg), // neg
                new CodeInstruction(OpCodes.Add) // add -(0/2) to newColor
            };

            codes.InsertRange(ind, adds);
            

            Logger.LogInfo("InvisColorSwap: Transpilation successful!");

            return codes.AsEnumerable();
        }
    }
}
