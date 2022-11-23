# Debug Panel

<p align="center">
    <a href="https://github.com/brunomikoski/Debug-Panel/blob/master/LICENSE.md">
		<img alt="GitHub license" src ="https://img.shields.io/github/license/Thundernerd/Unity3D-PackageManagerModules" />
	</a>

</p> 
<p align="center">
    <a href="https://openupm.com/packages/com.brunomikoski.debugpanel/">
        <img src="https://img.shields.io/npm/v/com.brunomikoski.debugpanel?label=openupm&amp;registry_uri=https://package.openupm.com" />
    </a>

  <a href="https://github.com/brunomikoski/Debug-Panel/issues">
     <img alt="GitHub issues" src ="https://img.shields.io/github/issues/brunomikoski/Debug-Panel" />
  </a>

  <a href="https://github.com/brunomikoski/Debug-Panel/pulls">
   <img alt="GitHub pull requests" src ="https://img.shields.io/github/issues-pr/brunomikoski/Debug-Panel" />
  </a>

  <img alt="GitHub last commit" src ="https://img.shields.io/github/last-commit/brunomikoski/Debug-Panel" />
</p>

<p align="center">
    	<a href="https://github.com/brunomikoski">
        	<img alt="GitHub followers" src="https://img.shields.io/github/followers/brunomikoski?style=social">
	</a>	
	<a href="https://twitter.com/brunomikoski">
		<img alt="Twitter Follow" src="https://img.shields.io/twitter/follow/brunomikoski?style=social">
	</a>
</p>


https://user-images.githubusercontent.com/600419/203599890-43fefa18-0189-4cba-8102-068b276abd93.mp4



Debug Panel is a tool to expose methods, fields to be activated/tweaked by the Panel, this is useful for the development process where you can expose hacks and tools to be easily accessible.

After seeing the awesome [DebugSheet package](https://github.com/Haruma-K/UnityDebugSheet), I refactored my system to use his visuals that are far better but keep the usability code based, and avoid prefab creation/resources usage



## Features
- Not Boatload with a lot of unrelated features.
- Code based
- Easily expose methods to be accessed by the Debug Panel by simple adding a `[DebuggableAction]` attribute
- Works with any `MonoBehaviours`, `Scriptable Objects` or any `object`
- Support shortcuts for method.
- Favorites
- UI for generic fields


## How it works?
- When you open the Debug Panel, will try to load any active `MonoBehaviours` or pre-registered `object` that contains the attribute `[DebuggableClass]` them will look for any exposed
  object by the attributes `[DebuggableAction]` `[DebuggableField]` and expose all of this inside the Debug Panel.
- You can also add any dynamic action by sending it directly to the `debugPanel.AddAction("Folder/Method Name", ()=>{Debug.Log("Dynamic Method");`

## How to use?
- Simple drag the `Debug Panel` prefab inside your boot scene
- The DebugPane doesn't come with any Singleton or any static access solution by default, this is intentionally since some developers prefer to work with other solutions like ServiceLocator/Dy and so on. But should be fairly simple to implement the ones you need, like a singleton by just extending the `DebugPanel` class.

### Exposing Methods
To expose a method from a class, you just need to add the `[DebuggableClass("PATH_TO_THIS_CLASS")]` to the class and also add the `[DebuggableAction]` to the method.

```c#
[DebuggableClass(Path = "Examples")]
public class ExampleExposingMethod : MonoBehaviour
{
    [DebuggableAction(Path = "Try this method")]
    private void ExampleMethod()
    {
        Debug.Log("This has been called by the debug panel!");
    }
}
```



### Tweaking Fields
To expose one field to be tweaked by the Panel you have to add `[DebuggableClass]` and the `[DebuggableField]` to the field, when submitting the changes on the keyboard will try to parse the value and apply back to the field
```c#
[DebuggableClass(Path = "Examples")]
public sealed class ExampleExposedField : MonoBehaviour
{
    [SerializeField, DebuggableField]
    private Vector3 playerVelocity = Vector3.one;
}
```


#### Supported fields:
- float
- string
- int
- double
- Vector2
- Vector3
- Vector4
- Quaternion
- Enums
- ScriptableObjectCollectionItem
- bool

### Debug Text Area
Sometimes you want to expose a big chunk of text on the panel as well, so you can expose any string like this and adding the `[Multiline]` attribute to it:
```c#
[DebuggableClass(Path = "Examples")]
public sealed class ExampleExposingTextArea : MonoBehaviour
{
    [DebuggableField, Multiline]
    private string debugString;

    private void Awake()
    {
        debugString = "Big text area\n";
        debugString += "<b>Value:</b> crazy value\n";
        debugString += "<b>Another Value:</b> 78888\n";
    }
}
```

You can also use the callbacks to do something when a specific field value changed, for instance:
```c#
[DebuggableClass(Path = "Examples")]
public sealed class ExampleExposingTextArea : MonoBehaviour
{
    [DebuggableField(OnAfterSetValueMethodName = nameof(OnInputTypeChanbged))]
    private enum InputType inputType;


    private void OnInputTypeChanbged()
    {
        Debug.Log($"Input Type Changed: {inputType}");
    }
}
```


## Shortcuts
You can define hotkeys for specific actions, following [Unity hotkey standard](https://docs.unity3d.com/ScriptReference/MenuItem.html), so this method would be triggered by <kbd>Ctrl/Command</kbd>+<kbd>Shift</kbd>+<kbd>A</kbd>

```c#
    [DebuggableAction(Path = "One More Test", "%#a")]
    private void OneMoreTest()
    {
        Debug.Log("Called from hotkey");
    }
```

```c#
% (ctrl on Windows, cmd on macOS), # (shift), & (alt).
```


# FAQ
### How does the Favorite Feature work?
If you hold any Debuggable for more than 2 seconds, you should se a star ⭐, this will make sure next time you see this item, will be at the begining.


https://user-images.githubusercontent.com/600419/203599816-cabe5250-dea1-4188-9441-1d55ddc7222b.mp4



### Its showing errors when using with the new Unity Input Manager.
Make sure you have Both selected on your player settings, since the DebugPanel still uses the legacy input system.

### Updating shortcuts for opening / closing debug panel
By default the keyboard shortcut for toggle the DebugPanel is (Alt+0) or 3 touches on the screen for more than 2 seconds.
You can change those settings on the DebugPanel/Trigger Settings

![trigger-settings](https://user-images.githubusercontent.com/600419/203600070-e143374d-3e36-4dcf-b3fe-cfea66d1482d.png)

### Active Load Debuggables
In order to expose Debuggable Methods that contains shortcut to be available even before the DebugPanel is enabled, you can active this setting on the DebugPanel prefab.
This will try to load all available debuggables as soon as a new scene is loaded.
This is expensive, and automatically disabled for mobile. 

### Favorites

## System Requirements
Unity 2018.4.0 or later versions


## How to install

<details>
<summary>Add from OpenUPM <em>| via scoped registry, recommended</em></summary>

This package is available on OpenUPM: https://openupm.com/packages/com.brunomikoski.debugpanel

To add it the package to your project:

- open `Edit/Project Settings/Package Manager`
- add a new Scoped Registry:
  ```
  Name: OpenUPM
  URL:  https://package.openupm.com/
  Scope(s): com.brunomikoski
  ```
- click <kbd>Save</kbd>
- open Package Manager
- click <kbd>+</kbd>
- select <kbd>Add from Git URL</kbd>
- paste `com.brunomikoski.debugpanel`
- click <kbd>Add</kbd>
</details>

<details>
<summary>Add from GitHub | <em>not recommended, no updates :( </em></summary>

You can also add it directly from GitHub on Unity 2019.4+. Note that you won't be able to receive updates through Package Manager this way, you'll have to update manually.

- open Package Manager
- click <kbd>+</kbd>
- select <kbd>Add from Git URL</kbd>
- paste `https://github.com/brunomikoski/Debug-Panel.git`
- click <kbd>Add</kbd>
</details>


