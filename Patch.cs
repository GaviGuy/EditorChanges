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
            enableChangeKeybinds;

        public static ConfigEntry<int> pitchUpKey,
            pitchDownKey,
            twistRightKey,
            twistLeftKey,
            turnRightKey,
            turnLeftKey;

        private void Awake() {
            logger = Logger;
            
            enableRemoveLaneLimit = Config.Bind("Toggles", "enableRemoveLaneLimit", true, "Whether to remove the limit of lane movement in the editor");
            enableFixTimeSigPlacement = Config.Bind("Toggles", "enableFixTimeSigPlacement", true, "Whether to adjust placement of time signatures when the first time sig is offset from the first bpm marker");
            enableChangeKeybinds = Config.Bind("Toggles", "enableChangePathKeybind", true, "Whether hardcoded flight path controls should be overridden (change keys in config if enabling)");

            pitchDownKey = Config.Bind("Keybinds", "PitchDownKey", (int) 'w', "Unicode value of the editor key to pitch down");
            pitchUpKey = Config.Bind("Keybinds", "PitchUpKey", (int) 's', "Unicode value of the editor key to pitch up");
            twistLeftKey = Config.Bind("Keybinds", "twistLeftKey", (int) 'a', "Unicode value of the editor key to twist left");
            twistRightKey = Config.Bind("Keybinds", "twistRightKey", (int) 'd', "Unicode value of the editor key to twist right");
            turnLeftKey = Config.Bind("Keybinds", "turnLeftKey", (int) 'e', "Unicode value of the editor key to turn left");
            turnRightKey = Config.Bind("Keybinds", "turnRightKey", (int) 'q', "Unicode value of the editor key to turn right");


            //Logger = base.Logger;
            
            if(enableFixTimeSigPlacement.Value)
                Harmony.CreateAndPatchAll(typeof(FixTimeSigPlacement));
            if(enableRemoveLaneLimit.Value)
                Harmony.CreateAndPatchAll(typeof(RemoveLaneLimit));
            if(enableChangeKeybinds.Value
                && pitchDownKey.Value != (int) 'w'
                || pitchUpKey.Value != (int) 's'
                || twistLeftKey.Value != (int) 'a'
                || twistRightKey.Value != (int) 'd'
                || turnLeftKey.Value != (int) 'e'
                || turnRightKey.Value != (int) 'q') {
                Harmony.CreateAndPatchAll(typeof(ChangeKeybinds));
                Harmony.CreateAndPatchAll(typeof(ChangeKeybindsDisplay));
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
