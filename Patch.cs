using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using UnityEngine;
using HarmonyLib;
using System;
using System.Threading;
using System.Reflection;
using System.Reflection.Emit;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EditorChanges {
    [BepInPlugin("srxd.editorchanges", "EditorChanges", "1.4.0")]
    public class Patch : BaseUnityPlugin {

        public static ManualLogSource logger;

        private static ConfigEntry<bool> enableRemoveLaneLimit,
            enableFixTimeSigPlacement,
            enableChangeKeybinds,
            enableCustomSubdivisions,
            enableInvisibility,
            enablePreciseTime;

        public static ConfigEntry<int> pitchUpKey,
            pitchDownKey,
            twistRightKey,
            twistLeftKey,
            turnRightKey,
            turnLeftKey;

        private void Awake() {
            logger = Logger;

            enableRemoveLaneLimit = Config.Bind("Toggles", "enableRemoveLaneLimit", true, "Whether to enable this module");
            enableFixTimeSigPlacement = Config.Bind("Toggles", "enableFixTimeSigPlacement", true, "Whether to enable this module");
            enableChangeKeybinds = Config.Bind("Toggles", "enableChangePathKeybind", true, "Whether to enable this module");
            enableCustomSubdivisions = Config.Bind("Toggles", "enableCustomSubdivisions", true, "Whether to enable this module");
            enableInvisibility = Config.Bind("Toggles", "enableInvisibility", true, "Whether to enable this module");
            enablePreciseTime = Config.Bind("Toggles", "enablePreciseTime", true, "Whether to enable this module");

            pitchDownKey    = Config.Bind("Keybinds", "PitchDownKey",   (int) 'w', "Unicode value of the key for DOWN");
            pitchUpKey      = Config.Bind("Keybinds", "PitchUpKey",     (int) 's', "Unicode value of the key for UP");
            twistLeftKey    = Config.Bind("Keybinds", "twistLeftKey",   (int) 'a', "Unicode value of the key for CLOCKWISE");
            twistRightKey   = Config.Bind("Keybinds", "twistRightKey",  (int) 'd', "Unicode value of the key for COUNTER-CLOCKWISE");
            turnLeftKey     = Config.Bind("Keybinds", "turnLeftKey",    (int) 'e', "Unicode value of the key for LEFT");
            turnRightKey    = Config.Bind("Keybinds", "turnRightKey",   (int) 'q', "Unicode value of the key for RIGHT");
            
            int enabledCount = 0;

            if(enableFixTimeSigPlacement.Value) {
                Harmony.CreateAndPatchAll(typeof(FixTimeSigPlacement));
//                Logger.LogInfo("Enabled FixTimeSigPlacement");
                enabledCount++;
            }

            if(enableRemoveLaneLimit.Value) {
                Harmony.CreateAndPatchAll(typeof(RemoveLaneLimit));
//                Logger.LogInfo("Enabled RemoveLaneLimit");
                enabledCount++;
            }

            if(enableChangeKeybinds.Value
                    && pitchDownKey.Value != (int) 'w'
                    || pitchUpKey.Value != (int) 's'
                    || twistLeftKey.Value != (int) 'a'
                    || twistRightKey.Value != (int) 'd'
                    || turnLeftKey.Value != (int) 'e'
                    || turnRightKey.Value != (int) 'q') {
                Harmony.CreateAndPatchAll(typeof(ChangeKeybinds));
                Harmony.CreateAndPatchAll(typeof(ChangeKeybindsDisplay));
//                Logger.LogInfo("Enabled ChangeKeybinds");
                enabledCount++;
            }

//            if(enableCustomSubdivisions.Value) {
//                Harmony.CreateAndPatchAll(typeof(CustomSubdivisions));
//                Logger.LogInfo("Enabled CustomSubdivisions");
//            }

            if(enableInvisibility.Value) {
                Harmony.CreateAndPatchAll(typeof(InvisToggle));
                Harmony.CreateAndPatchAll(typeof(InvisColorSwap));
//               Logger.LogInfo("Enabled Invisibility");
                enabledCount++;
            }

            if(enablePreciseTime.Value) {
                Harmony.CreateAndPatchAll(typeof(DisplayPreciseTime));
//               Logger.LogInfo("Enabled PreciseTime");
                enabledCount++;
            }
            
            Logger.LogInfo("Enabled " + enabledCount + " modules");
        }
    }
}
