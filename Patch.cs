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
    [BepInPlugin("srxd.editorchanges", "EditorChanges", "1.1.0")]
    public class Patch : BaseUnityPlugin {

        public static ManualLogSource logger;

        private static ConfigEntry<bool> enableRemoveLaneLimit,
            enableFixTimeSigPlacement,
            enableChangeKeybinds,
            enableCustomSubdivisions,
            enableInvisibility;

        public static ConfigEntry<int> pitchUpKey,
            pitchDownKey,
            twistRightKey,
            twistLeftKey,
            turnRightKey,
            turnLeftKey;

        public static ConfigEntry<int[]> subdivisionList;

        private static int[] defaultSubdivisions = {2, 4, 5, 6, 8, 10, 12, 16, 24, 60}; // it won't let me config with arrays :c

        private void Awake() {
            logger = Logger;
            
            /* 
             * change how this works. disable modules if they're not configed instead of having a setting to disable each one
             * Lane limit: -1 to disable, 96 default
             * time sig: seperate module? on/off?
             * changeKeybinds: default to disable
             * customSubdivisions: default to disable
             */

            enableRemoveLaneLimit = Config.Bind("Toggles", "enableRemoveLaneLimit", true, "Whether to enable this module");
            enableFixTimeSigPlacement = Config.Bind("Toggles", "enableFixTimeSigPlacement", true, "Whether to enable this module");
            enableChangeKeybinds = Config.Bind("Toggles", "enableChangePathKeybind", true, "Whether to enable this module");
            enableCustomSubdivisions = Config.Bind("Toggles", "enableCustomSubdivisions", true, "Whether to enable this module");
            enableInvisibility = Config.Bind("Toggles", "enableInvisibility", true, "Whether to enable this module");

            pitchDownKey    = Config.Bind("Keybinds", "PitchDownKey",   (int) 'w', "Unicode value of the key for DOWN");
            pitchUpKey      = Config.Bind("Keybinds", "PitchUpKey",     (int) 's', "Unicode value of the key for UP");
            twistLeftKey    = Config.Bind("Keybinds", "twistLeftKey",   (int) 'a', "Unicode value of the key for CLOCKWISE");
            twistRightKey   = Config.Bind("Keybinds", "twistRightKey",  (int) 'd', "Unicode value of the key for COUNTER-CLOCKWISE");
            turnLeftKey     = Config.Bind("Keybinds", "turnLeftKey",    (int) 'e', "Unicode value of the key for LEFT");
            turnRightKey    = Config.Bind("Keybinds", "turnRightKey",   (int) 'q', "Unicode value of the key for RIGHT");

            //subdivisionList = Config.Bind("Misc", "subdivisionList", defaultSubdivisions, "Possible values for editor subdivisions");
            
            if(enableFixTimeSigPlacement.Value) {
                Harmony.CreateAndPatchAll(typeof(FixTimeSigPlacement));
                Logger.LogInfo("Enabled FixTimeSigPlacement");
            }

            if(enableRemoveLaneLimit.Value) {
                Harmony.CreateAndPatchAll(typeof(RemoveLaneLimit));
                Logger.LogInfo("Enabled RemoveLaneLimit");
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
                Logger.LogInfo("Enabled ChangeKeybinds");
            }

 //           if(enableCustomSubdivisions.Value) {
 //               Harmony.CreateAndPatchAll(typeof(CustomSubdivisions));
 //               Logger.LogInfo("Enabled CustomSubdivisions");
 //           }

            if(enableInvisibility.Value) {
                Harmony.CreateAndPatchAll(typeof(InvisColorSwap));
                Logger.LogInfo("Enabled Invisibility");
            }

        }

        /*

        [HarmonyPatch(typeof(TrackEditorGUI), "HandleTrackTurnEditorInput")]
        public class mirrorTrackTurn {
            //TODO: allow action via a key

        }

        [HarmonyPatch(typeof(), "")]
        public class removeAutoDeletion {
            //TODO: passive
        }

        [HarmonyPatch(typeof(), "")]
        public class keepPlaceInClipInfoEditor {
            //TODO: passive
            //OnEditClipInfoPressed()
            //CycleEditorMode
        }

        public class allowBugSliderShapeChange { }
        public class allowStrayBeatholdChange { }

        [HarmonyPatch(typeof(TrackEditorGUI), "HandleNoteEditorInput()"]
        public class toggleInvisibile {
            //make a new repeated section
            //will also need to patch InputMapping.SpinCommands
            //will also need to explore input keys
        }

        */
    }
}
