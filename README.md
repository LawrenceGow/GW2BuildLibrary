## What is GW2BuildLibrary?

GW2BuildLibrary is a simple application to help store and manage build templates from Guild Wars 2.

## Installation

1. Download latest .zip archive from [here](https://github.com/LawrenceGow/GW2BuildLibrary/releases).
2. Place the downloaded archive where you wish to install.
3. Unzip the archive.
4. Ensure both GW2BuildLibrary.exe and the README file are in the same folder.
5. Done!

## General Usage

Run "GW2BuildLibrary.exe", once open simply copy the build template from Guild Wars 2 and tap on an empty slot to save it. Saved builds can be named by clicking on the quill icon.

To get a build back out of GW2BuildLibrary simply click on its slot, this will place the build in the clipboard which can then be pasted onto a build slot in-game.

Builds can be deleted by clicking the 'X' button on their respective slot.

Builds are saved automatically upon the application closing.

### Command-Line Arguments
- o|overlay : `-o` or `--overlay`
  - Opens GW2BuildLibrary with no window furniture.
  - GW2BuildLibrary will be placed on top of all other windows.
- ~~f|full-screen: `-f` or `--full-screen`~~
  - GW2BuildLibrary will open full screen, regardless of the previous state.
- q|quick: `-q` or `--quick`
  - GW2BuildLibrary will close after copying a build into the clipboard or after storing a build.
- no-save-window-state: `--no-save-window-state`
  - GW2BuildLibrary will not save the window state.
- profession=VALUE: `--profession="Warrior"`
  - Opens GW2BuildLibrary filtered to the specified profession.
  - Name must be typed in-full with the first letter capitalised.
- export=FILENAME: `--export="D:\Program Files (x86)\Guild Wars 2\Build Library\builds.xml"`
  - Exports the builds to the file specified in the XML format then exits.
- ~~import=FILENAME: `--import="D:\Program Files (x86)\Guild Wars 2\Build Library\builds.xml"`~~
  - Imports builds from the file specified.
- h|?|help: `-h` or `-?` or `--help`
  - Displays a message with all possible command line arguments with the option to open this file.

#### Running With Arguments sans Command-Line

If you want to run GW2BuildLibrary with some arguments set there are two methods:

##### Create a shortcut

Right-click on GW2BuildLibrary.exe and select 'Create shortcut' from the menu. Once created right-click the shortcut and select 'Properties' from the menu.

With the properties window open edit the 'Target' field and place the arguments you wish GW2BuildLibrary to run with at the end, it should look something like this:

`"D:\Program Files (x86)\Guild Wars 2\Build Library\GW2BuildLibrary.exe" --overlay`

After setting the 'Target' click 'Apply' and 'OK'. Now double clicking the shortcut should run GW2BuildLibrary with the arguments specified.

##### Create a bat file

The following is an example of a simple .bat file that can be used to run GW2BuildLibrary with some arguments.

```bash
start GW2BuildLibrary.exe --overlay
exit
```

## XML Save File Format

