## What is GW2BuildLibrary?

GW2BuildLibrary is a simple application to help store and manage build templates from Guild Wars 2.

## Installation

1. Download latest .zip archive from [here](https://github.com/LawrenceGow/GW2BuildLibrary/releases).
2. Place the downloaded archive where you wish to install.
3. Unzip the archive.
5. Done!

## General Usage

Run "GW2BuildLibrary.exe", once open simply copy the build template from Guild Wars 2 and tap on an empty slot to save it. Saved builds can be named by clicking on the quill icon.

To get a build back out of GW2BuildLibrary simply click on its slot, this will place the build in the clipboard which can then be pasted onto a build slot in-game.

Builds can be deleted by clicking the 'X' button on their respective slot.

Builds are saved automatically upon the application closing.

### Command-Line Arguments

#### Help

`-h` or `-?` or `--help`
Displays a message with all possible command line arguments with the option to open this file.

#### Overlay Mode

`-o` or `--overlay`
Opens GW2BuildLibrary with no window furniture. GW2BuildLibrary will be placed on top of all other windows.

#### Full Screen Mode

`-f` or `--full-screen`
GW2BuildLibrary will open full screen, regardless of the previous state.

#### Quick Mode

`-q` or `--quick`
GW2BuildLibrary will close after copying a build into the clipboard or after storing a build.

#### Don't save window state

`--no-save-window-state`
GW2BuildLibrary will not save the window state.

#### Profession Filter

`--profession=<profession_name>`
Opens GW2BuildLibrary filtered to the specified profession. Name must be typed in-full with the first letter capitalised.

#### Export

`--export="D:\Program Files (x86)\Guild Wars 2\Build Library\builds.xml"`
Exports the builds to the file specified in the XML format then exits.

#### Import

`--import="D:\Program Files (x86)\Guild Wars 2\Build Library\builds.xml"`
Imports builds from the file specified.