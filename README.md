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
     <img alt="GitHub issues" src ="https://img.shields.io/github/issues/brunomikoski/Animation-Sequencer" />
  </a>

  <a href="https://github.com/brunomikoski/Debug-Panel/pulls">
   <img alt="GitHub pull requests" src ="https://img.shields.io/github/issues-pr/brunomikoski/Animation-Sequencer" />
  </a>
  
  <img alt="GitHub last commit" src ="https://img.shields.io/github/last-commit/brunomikoski/Animation-Sequencer" />
</p>

<p align="center">
    	<a href="https://github.com/brunomikoski">
        	<img alt="GitHub followers" src="https://img.shields.io/github/followers/brunomikoski?style=social">
	</a>	
	<a href="https://twitter.com/brunomikoski">
		<img alt="Twitter Follow" src="https://img.shields.io/twitter/follow/brunomikoski?style=social">
	</a>
</p>

<p align="center">
  <img alt="Example" src="https://user-images.githubusercontent.com/600419/121535785-5f21f500-c9fa-11eb-8660-c56e891f1eec.gif">
</p>

Debug Panel is a tool to expose methods, fields to be activated/tweaked by the Panel, this is useful for the development process where you can expose hacks and tools to be easily accessible.


*This is still in heavy development, please use it carefully*

## Features
- Not Boatload with a lot of unrelated features.
- No extra assets, the whole Debug Panel is build using default `TextMeshPro` font and no additional sprites, so nothing to worry about extra binary size
- Easily expose methods to be accessed by the Debug Panel by simple adding a `[DebuggableAction]` attribute 
- Works with any `MonoBehaviours`, `Scriptable Objects` or `object`
- Support shortcuts for any exposed method.  
- Stores the expanded and previous search on player prefs, so anyone will always have his panel as he needs at that moment.


## How it works? 
- When you open the Debug Panel, will try to load any active `MonoBehaviours` or pre-registered `object` that contains the attribute `[DebuggableClass]` them will look for any exposed object by the attributes `[DebuggableAction]` `[DebuggableField]` or `[DebuggableTextArea]` and expose all of this inside the Debug Panel.

## How to use?
- Simple drag the `Debug Panel` prefab inside your boot scene

### Exposing Methods
To expose a method from a class, you just need to add the `[DebuggableClass("GROU_NAME")]` to the class and also add the `[DebuggableAction]` to the method.

```c#
[DebuggableClass("Examples")]
public class ExampleExposingMethod : MonoBehaviour
{
    [DebuggableAction("Try this method")]
    private void ExampleMethod()
    {
        Debug.Log("This has been called by the debug panel!");
    }
}
```
![exposed-method-example](https://user-images.githubusercontent.com/600419/121535865-719c2e80-c9fa-11eb-9b80-38f387b662be.gif)



### Tweaking Fields
To expose one field to be tweaked by the Panel you have to add `[DebuggableClass]` and the `[DebuggableField]` to the field, when submitting the changes on the keyboard will try to parse the value and apply back to the field
```c#
[DebuggableClass("Examples")]
public sealed class ExampleExposedField : MonoBehaviour
{
    [SerializeField, DebuggableField]
    private Vector3 playerVelocity = Vector3.one;
}
```

![exposed-field](https://user-images.githubusercontent.com/600419/121535880-7660e280-c9fa-11eb-864c-8c53f602e99c.gif)


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

### Debug Text Area
Sometimes you want to expose a big chunk of text on the panel as well, so you can expose any string on the panel like this:
```c#
[DebuggableClass("Examples")]
public sealed class ExampleExposingTextArea : MonoBehaviour
{
    [DebuggableTextArea]
    private string debugString;

    private void Awake()
    {
        debugString = "Big text area\n";
        debugString += "<b>Value:</b> crazy value\n";
        debugString += "<b>Another Value:</b> 78888\n";
    }
}
```

## Shortcuts
You can define hotkeys for specific actions, following Unity hotkey standard, so this method would be triggered by <kbd>Ctrl/Command</kbd>+<kbd>Shift</kbd>+<kbd>A</kbd>

```c#
    [DebuggableAction("One More Test", "%#a")]
    private void OneMoreTest()
    {
        Debug.Log("Called from hotkey");
    }
```

```c#
% (ctrl on Windows, cmd on macOS), # (shift), & (alt).
```



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


