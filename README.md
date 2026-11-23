# KioskHelper

A WIP collection of utility scripts for building Unity kiosk applications. A kind of kiosk boilerplate. Some of what I'll be adding is pretty dated, so I'm rewriting it as I go. Latest added was the Dialog Box. Next in will be a custom on-screen keyboard with editor.

**Note:** If you happen to make updates, fixes, etc. Please open a PR so I can have a look and probably incorporate what you've done.

## What's Included

KioskHelper currently includes the following prefabs, scripts, and utilities:

### Prefabs
- **Corner Clicker** - Configurable multi-corner click detection for hidden settings menus, app shutdown, and other kiosk controls.
- **Dialog Box** - Customizable modal dialog box.
- **Scene Transitioner** - Reusable horizontal and vertical wipe transitions between scenes.
- **Timer** - Configurable count-up and countdown timer with completion events.

### Scripts
- **Idle Detector** - Monitors user activity and triggers warning, idle, and activity events.
- **Persist Object** - Keeps GameObjects alive between scene changes while preventing duplicates.
- **Scene Loader** - Asynchronously loads scenes with optional manual activation.
- **Standard Controls** - Basic keyboard controls for toggling the mouse cursor and exiting the application.

### Utilities
- **Dialog Style** - Scriptable object that allows creating different dialog box styles.
- **Layout Group Visualizer** - Editor gizmo utility for visualizing Unity layout groups.
- **Profanity Filter** - Multi-stage input filtering with character substitution and obfuscation detection.
- **Profanity Dictionary** - HashSet-based collection of nearly 300 words used by the profanity filter.
- **Test Profanity Filter** - A bunch of swears and non-swears to test the filter.

See below for detailed descriptions, setup instructions, and usage information for each component.

## Dependencies

KioskHelper requires:

- TextMeshPro
- Unity UI
- Input System
- DOTween - https://assetstore.unity.com/packages/tools/animation/dotween-hotween-v2-27676


## Installing

Install through the Unity Package Manager

Window > Package Management > Package Manager > Install package from git URL...

URL: https://github.com/dmennenoh/KioskHelper.git

**Note about DOTween:** If you're installing fresh then import DOTween into Unity from your assets and run the required setup. In the Utility Panel click the Create ASMDEF button. That will generate DOTween.Modules that Kiosk Helper needs to reference. If DOTWeen is already installed in your project go to Tools > Demigiant > DOTween Utility Panel and create the ASMDEF.


## Contact

Suggestions or whatever: dmennenoh95@gmail.com


## Components - Prefabs

**NOTE:** To make Unity place a prefab at the coordinates saved inside its root Transform when you drag it into the Hierarchy, enable "Use prefab asset position when dropping on the Hierarchy window" in your Scene View preferences. Enabling this setting restores Unity to the way it used to handle dragging in prefabs and not sure why this is off by default. So Edit > Preferences > Scene View > Enable 'Use prefab asset position when dropping on the Hierarchy window'
Now, when you drag a prefab like DialogBox into a canvas it will appear at screen center. You can, of course, just Alt-click the center anchor position in Anchor Settings, but enabling the preference is easier.

### Corner Clicker

**Description:** Old Reliable. Many times this was used to simply shutdown an app after four taps in the upper right corner. But you can require any number of taps in any corner, and use multiple corners. For instance two taps lower left, followed by two taps upper right could open a settings dialog. There is no set corner order - if UL and LR are both set to 2, it doesn't matter which corner you start in.

**Setup:**
1. Drag the CornerClicker prefab into your scene.
2. Set the number of click counts for each corner, in the Inspector.
3. Set the corner square size if you want, but the default (200) is pretty good for both 2K and 4K.
4. Use the Inspector to invoke a function in your code when the `cornerClickerActivated` event is raised.

**Note:** If you want to see the corners for scaling, etc. just select the four children in the prefab and set the Image component's alpha to something other than zero.

### Dialog Box

**Description:** A simple, but flexible modal dialog box that's easy to customize using scriptable objects. Includes some common icons (error, info, question, success, warning) that you can use, or add your own.

**Setup:**
1. Drag the DialogBox prefab into a Canvas. If it doesn't appear at center-screen have a look at the note at the start of this section. 
2. Set the width and height of the rectTransform to match your canvases reference resolution settings.
3. Scale the DialogRoot if necessary. It should be good if you're dropping into a 2K canvas, but if 4K you may want to change scale to 1.75 or so.
4. Create a new instance of a DialogStyle scriptable object: Create new asset > Kiosk Helper > Dialog Style. 
5. Configure the settings for the new style/data asset. 
6. Pass an instance of the new asset into the DialogControllers Show() method.

**Public Methods:**

- `Show(styleInstance, title, text, primaryButtonText, secondaryButtonText, callBackMethod)`    

If secondaryButtonText is omitted, or "" the secondary button will not be shown. If callBackMethod is omitted the dialog will just close once the button is pressed.

Regarding passing an instance of the style object. In your controller, or whatever script you're opening the dialog from you will have a reference to `DialogController` on the Dialog Box, in order to call `Show()` on it. Just add another reference for the DialogStye and hook it up in the inspector. Something like: `[SerializeField] private DialogStyle myStyle` Then, when you want to show the dialog you can pass `myStyle`. If you've got multiple styles either use an array: `[SerializeField] private DialogStyle[] myStyles` or list them individually, it's up to you.

**Example:** 
```c#
[SerializeField] private DialogController dc;
[SerializeField] private DialogStyle warningStyle;
[SerializeField] private DialogStyle infoStyle;

dc.Show(warningStyle, "NOTICE!", "Changes have not been saved", "OK", "Cancel", SaveDialogResult);
dc.Show(infoStyle, "Welcome", "Thanks for joining!", "OK");

private void SaveDialogResult(DialogResult result)
{
    Debug.Log($"Result: {result}"); //will print Primary, Secondary, Closed, or BackgroundClicked

    if(result == DialogResult.Primary)
    {
        //user clicked OK... don't save
    }
}
```

### Scene Transitioner

**Description:** A horizontal or vertical wipe transition. There are four different sets of masks you can use to modify the transition. These are located in the KioskHelper\Textures folder. Just replace the default maskL_30 and maskR_30 images within the maskL and maskR objects in the prefab. Be sure to maskL images in the maskL object and maskR images for the maskR object.

**Setup:**
1. Drag the prefab into a scene.
2. For a vertical transition uncheck Horizontal Transition in the Inspector.
2. Drag an image into the Transition Image parameter in the Inspector.
3. Optionally change the mask images as described above.
4. Call Show() and Hide() as necessary, on the SceneTransitionController component.
5. Optionally use the Inspector to invoke functions in your code when the `transitionHidden` and `transitionShowing` events are raised. 

**Note:** Add a SceneTransitioner prefab to *each* scene you want to transition in/out of.

### Timer

**Description:** A on screen timer that can display min:sec min:sec.tenths or min.sec.hundreths. It can count up or down and if counting down will raise `timerComplete` when the timer reaches zero. The text field is using the default Liberation Sans font in TMP. Change this in your scene as needed.

**Setup:**
1. Drag the Timer prefab into a Canvas object and position it accordingly.
2. See the note at the start of this section if the prefab appears off-screen initially. 
3. If you'll be counting down and want to listen for `timerComplete`, use the Inspector to invoke a function in your code.

**Public Methods:**

- `SetTimer(numSeconds = 0)`    Call this first, in order to set the timer.
- `StartTimer()`                Starts the timer counting up
- `StartCountdown()`            Starts the timer counting down to zero
- `PauseTimer()`                Pauses/Stops the timer
- `ResumeTimer()`               Resumes counting
- `ResetTimer()`               Resets the timer. If counting down the timer is set to the initial starting time, otherwise it's set to zero.


## Components - Scripts

### Idle Detector

**Description:** Use Idle Detector to monitor user activity - ie so you know when someone walks away. If no interaction has happened for a set time period, an `onIdle` event will be raised. You can also issue a `onWarning` n seconds before `onIdle` is fired, in order to present the user with something like a 'Are you still there?' dialog box. Finally, once `onIdle` has fired an `onActivity` event will be fired once interaction resumes. You can use this to stop a screen saver, etc.

**Setup:**
1. Add Idle Detector to a GameObject.
2. In the Inspector set the Idle time and Warning time. Warning time is the number of seconds before idle time.
3. Invoke events in your code that will be called when `onWarning`, `onIdle` or `onActivity` are raised.

**Public Methods**
- `Reset()` - Resets the activity time and flags.

### Persist Object

**Description:** Uses `DontDestroyOnLoad()` in order to keep an object, and any children, alive between scene changes. Uses the standard singleton pattern in order to avoid making duplicates if the orignal scene is returned to.

**Setup:**
1. Add PersistObject to the GameObject you want to keep alive

### Scene Loader

**Description:**  Loads Unity scenes asynchronously and optionally waits for an external signal before activating the new scene.

**Setup:**
1. Add SceneLoader to a GameObject.
2. Call `LoadNewScene()` - with the target scene name.
3. Use `ActivateScene()` - when you're ready to activate the scene.

**Public Methods:**
- `LoadNewScene()` - Begins asynchronous scene loading.
- `ActivateScene()` - Allows the loaded scene to activate.

**Public Properties:**
- `IsLoading` - Indicates whether a scene is currently loading.
- `Progress` - Normalized loading progress from 0 to 1.

### Standard Controls

**Description:** Super simple keyboard listener that allows a BA (Brand Ambassador) or other person manning the kiosk, to use the keyboard for simple control. Press F1 to toggle the mouse cursor. Press Esc to exit the application. 

**Setup:**
1. Simply drag onto any GameObject. 
2. Set hide cursor initially to false if you want to initially see the cursor. Hidden by default.


## Components - Scripts / Utilities

### Dialog Style

**Description:** This is a scriptable object used for styling the dialog box prefab. Create an instance with Create New Asset > Kiosk Helper > Dialog Style 
Give it an appropriate name and change the configuration values as needed. Pass these to the dialog box's Show() method to have different dialog styles. Within the KioskHelper \ Textures folder are various 50x50 dialog icons you can  use to get started.

### Layout Group Visualizer

**Description:** A gizmos utility script that allows you to visualize padding, spacing, and child boundaries of whatever layout group (horizontal, vertical, grid) that you attach it to. Executes in the Editor only.

### Profanity Dictionary

**Description:** A fairly comprehensive list of nearly 300 words. This is used by the Profanity Filter class. Contains all the three letter swears we could come up with, in order to also filter user initials.

### Profanity Filter

**Description:** Detects profanity and attempts to catch common methods of bypassing word filters, including leetspeak, inserted punctuation, whitespace, and wildcard substitutions. This is Another one that was used on so many projects. Pretty much anytime you need to allow user input, it's not a bad idea to pass it through a profanity filter. 

**Setup:**
1. It's a static class so just add the `using KioskHelper;` to your usings.
2. Call `ProfanityFilter.ContainsProfanity(stringToCheck)`
3. Returns a bool - true if profanity was found.

**Note:** If you find any false positives or negatives please let me know - dmennenoh95@gmail.com

### Test Profanity Filter

**Description:** Simple script tests the filter by running a bunch of known words and phrases through it. 

**Setup:**
1. Just drag onto a gameObject and press play. Check Console window for the output.


## Components - Scripts / Prefabs

1. `Corner Clicker` - This is the script attached to the Corner Clicker prefab.
2. `Dialog Controller` - This is the script attached to the DialogBox prefab - contains the DialogResult enum.
3. `Scene Transition Controller` - This is the controller script for the SceneTransitioner prefab.
4. `Timer` - This is the timer script attached to the Timer prefab.
