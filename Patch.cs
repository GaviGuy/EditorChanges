using BepInEx;
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
    public class EditorFixPlugin : BaseUnityPlugin {
        public static BepInEx.Logging.ManualLogSource Logger;

        private void Awake() {
            Logger = base.Logger;
            Harmony.CreateAndPatchAll(typeof(FixTimeSigPlacement));
            Harmony.CreateAndPatchAll(typeof(RemoveLaneLimit));
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
