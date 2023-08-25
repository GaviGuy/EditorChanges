using BepInEx;
using BepInEx.Configuration;
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
    [BepInPlugin("srxd.editorchanges", "EditorChanges", "1.0.0")]
    public class Patch : BaseUnityPlugin {
        //public static BepInEx.Logging.ManualLogSource Logger;
        private ConfigEntry<bool> enableRemoveLaneLimit,
            enableFixTimeSigPlacement,
            enableChangeKeybinds;

        private void Awake() {
            
            enableRemoveLaneLimit = Config.Bind("General.Toggles", "enableRemoveLaneLimit", true, "Whether to remove the limit of lane movement in the editor");
            enableFixTimeSigPlacement = Config.Bind("General.Toggles", "enableFixTimeSigPlacement", true, "Whether to adjust placement of time signatures when the first time sig is offset from the first bpm marker");
            enableChangeKeybinds = Config.Bind("General.Toggles", "enableChangePathKeybind", false, "Whether hardcoded flight path controls should be overridden (see config below to change the keys)");

            //Logger = base.Logger;
            
            if(enableFixTimeSigPlacement.Value)
                Harmony.CreateAndPatchAll(typeof(FixTimeSigPlacement));
            if(enableRemoveLaneLimit.Value)
                Harmony.CreateAndPatchAll(typeof(RemoveLaneLimit));
            if(enableChangeKeybinds.Value)
                Harmony.CreateAndPatchAll(typeof(ChangeKeybinds));
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
