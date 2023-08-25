# EditorFixes
A plugin that adds extra functionality to the SRXD editor.

## Current Features
* allows placing and moving of notes past the outer lanes
* fixes a bug that causes time signatures to place at the wrong time
* allows changing key mapping for the flight path editor

## Using the Config File

After running the mod once, a config will be generated at Spin Rhythm\BepInEx\config named srxd.editorchanges.cfg
Use it to enable individual features of this plugin and to set new keys to use in the flight path editor

*note: due to the way SRXD codes controls for the flight editor, a more human-friendly interface for changing the controls isn't possible without a large rework of code--well beyond my expertise.*
*for now, you'll have to use the unicode value of the key*

## Potential Future Features
* allow mirroring in the flight path editor
* allow changing notes to their invisible variants
* allow changing shape of sliders with no endpoint
* allow changing ending of stray beathold ends
* place error flags on stray hold ends, matches ending spins, and scratch snipes
* fix a bug that causes position in clip info editor to be different than in the note editor
* fix a bug that deletes notes when changing a tempomap
