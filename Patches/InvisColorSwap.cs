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
                if (codes[i].opcode == OpCodes.Conv_U1) {
                    ind = i - 4;
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
            
            codes.RemoveRange(ind, 4);

            CodeInstruction[] adds = {
                // it's quite shrimple actually. fuck that old noise, just bitwise XOR with 1 lmao
                new CodeInstruction(OpCodes.Ldc_I4_1), // push 1
                new CodeInstruction(OpCodes.Xor) // xor

            };

            codes.InsertRange(ind, adds);
            

            Logger.LogInfo("InvisColorSwap: Transpilation successful!");

            return codes.AsEnumerable();
        }
    }
}
