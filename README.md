# KioskHelper

A WIP collection of utility scripts for Unity kiosk applications.

I decided to publish a bunch of stuff I've used over the years to help in building kiosk type games and applications. Kind of kiosk boilerplate. Some of these like the Corner Clicker prefab/script were used in literally every kiosk I ever produced. I'll be adding more as I can - configurable keyboard coming soon.


## Dependencies

KioskHelper requires:

- TextMeshPro
- Unity UI
- Input System
- DOTween - https://assetstore.unity.com/packages/tools/animation/dotween-hotween-v2-27676


## Installing

Window > Package Management > Package Manager > Install package from git URL...

URL: https://github.com/dmennenoh/KioskHelper.git

**Note about DOTween:** If you're installing fresh then import DOTween into Unity from your assets and run the required setup. In the Utility Panel click the Create ASMDEF button. That will generate DOTween.Modules that Kiosk Helper needs to reference. If DOTWeen is already installed in your project go to Tools > Demigiant > DOTween Utility Panel and create the ASMDEF.


## Contact

Suggestions or whatever: dmennenoh95@gmail.com


## Components - Prefabs

### Corner Clicker

**Description:**  Old Reliable. Many times this was used to simply shutdown an app after four taps in the upper right corner. But you can require any number of taps in any corner, and use multiple corners. For instance two taps lower left, followed by two taps upper right could open a settings dialog. 

**Setup:**
1. Drag the CornerClicker prefab into your scene.
2. Set the number of click counts for each corner, in the Inspector.
3. Set the corner square size if you want, but the default (200) is pretty good for both 2K and 4K.
4. Use the Inspector to invoke a function in your code when the `cornerClickerActivated` event is raised.

**Note:** If you want to see the corners for scaling, etc. just select the four children in the prefab and set the Image components alpha to something other than zero.

### Scene Transitioner

**Description:** As of now (Nov 26) this is just a simple diagonal wipe. Eventually, it'll be a more modular system probably using scriptable objects as transition modules. Add a SceneTransitioner prefab to *each* scene you want to transition in/out of.

**Setup:**
1. Drag the prefab into a scene.
2. Drag an image into the Transition Image parameter in the Inspector.
3. Call Show() and Hide() as necessary, on the SceneTransitionController component.
4. Optionally use the Inspector to invoke functions in your code when the `transitionHidden` and `transitionShowing` events are raised. 

### Timer

**Description:** A on screen timer that can display min:sec min:sec.tenths or min.sec.hundreths. It can count up or down and if counting down will raise `timerComplete` when the timer reaches zero. The text field is using the default Liberation Sans font in TMP. Change this in your scene as needed.

**Setup:**
1. Drag the Timer prefab into a Canvas object and position it accordingly.
2. You can use the anchor presets in the Timers Rect Transform to initially position it.
3. If you'll be counting down and want to listen for `timerComplete`, use the Inspector to invoke a function in your code.

**Public Methods:**

- `SetTimer(numSeconds = 0)`    Call this first, in order to set the timer.
- `StartTimer()`                Starts the timer counting up
- `StartCountdown()`            Starts the timer counting down to zero
- `PauseTimer()`                Pauses/Stops the timer
- `ResumeTimer()`               Resumes counting
- `ResetTimer()`               Resets the timer. If counting down the timer is set to the initial starting time, otherwise it's set to zero.


## Components - Scripts

### Corner Clicker 

**Description:** This is the script attached to the Corner Clicker prefab. Unless you want to, you shouldn't need to use this on it's own.

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

## Scene Transition Controller

**Description:** This is the controller script for the SceneTransitioner prefab. You shouldn't need to use this on its own.

## Standard Controls

**Description:** Super simple keyboard listener that allows a BA (Brand Ambassador) or other person manning the kiosk, to use the keyboard for simple control. Press F1 to toggle the mouse cursor. Press Esc to exit the application. 

**Setup:**
1. Simply drag onto any gameObject. 
2. Set hide cursor initially to false if you want to initially see the cursor. Hidden by default.

## Timer

**Description:** This is the timer script attached to the Timer prefab.
