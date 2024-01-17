# EditorChanges
A plugin that adds extra functionality to the SRXD editor.

## Current Features
* allows placing and moving of notes past the outer lanes
* allows color-swapping invisible notes
* allows visibility-swapping notes (via the Cut keybind)
* fixes a bug that causes time signatures to place at the wrong time
* allows changing key mapping for the flight path editor

## Potential Future Features
* allow mirroring in the flight path editor
* putting new editor functions onto their own keys (instead of stealing keys)
* allow changing shape of sliders with no endpoint
* allow changing ending of stray beathold ends
* place error flags on stray hold ends, matches ending spins, and scratch snipes
* fix a bug that causes position in clip info editor to be different than in the note editor
* fix a bug that deletes notes when changing a tempomap

## Installing and Configuring 
First, install BepInEx: https://github.com/BepInEx/BepInEx/releases/tag/v5.4.22  
Next, navigate to your SRXD installation (`...\Steam\steamapps\common\Spin Rhythm`) and delete/rename `UnityPlayer.dll`, then rename `UnityPlayer_mono.dll` to `UnityPlayer.dll`.  
Now, running the game should generate folders and files inside `BepInEx`. Put this mod's dll file into `BepInEx\plugins\EditorChanges`.  
Most features should work automatically. Run the game again to generate a config file at `BepInEx\config`. Use it to disable individual modules and change twisty track keybinds.  
