# Animation Sequencer

<p align="center">
    <a href="https://github.com/brunomikoski/Debug-Panel/blob/master/LICENSE.md">
		<img alt="GitHub license" src ="https://img.shields.io/github/license/Thundernerd/Unity3D-PackageManagerModules" />
	</a>

</p> 
<p align="center">
	<a href="https://www.codacy.com/gh/brunomikoski/Animation-Sequencer/dashboard?utm_source=github.com&amp;utm_medium=referral&amp;utm_content=brunomikoski/Animation-Sequencer&amp;utm_campaign=Badge_Grade"><img src="https://app.codacy.com/project/badge/Grade/ab4c5923ca0545c9b8c85d87adbd689a"/></a>
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
  <img alt="Example" src="https://user-images.githubusercontent.com/600419/109826506-c299cb00-7c32-11eb-8b0d-8c0e97c4b5b7.gif">
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

### Debug Text Area
Sometimes you want to expose a big chunk of text on the panel as well, so you can expose any string on the panel like this:




  
- Animation Sequencer rely on DOTween for now, so it a requirement that you have `DOTween` on your project with properly created `asmdef` for it (Created by the `DOTween` setup panel)
- Add the Animation Sequencer to any GameObject and start your animation! 
- Using the <kbd>+</kbd> button under the `Animation Steps` you can add a new step
- Select <kbd>Tween Target</kbd>
- Use the <kbd>Add Actions</kbd> to add specific tweens to your target
- Press play on the Preview bar to view it on Editor Time.
- To play it by code, just call use `animationSequencer.Play();`

## FAQ

<details>
    
<summary>How can I create my custom DOTween actions?</summary> 
Lets say you want to create a new action to play a specific sound from your sound manager on the game, you just need to extend the `AnimationStepBase`

```c#
[Serializable]
public class PlayAudioClipAnimationStep : AnimationStepBase
{
    [SerializeField]
    private AudioClip audioClip;

    //Duration of this step, in this case will return the length of the clip.
    public override float Duration => audioClip.length;
    //This is the name that will be displayed on the + button on the Animation Sequencer
    public override string DisplayName => "Play Audio Clip";

    //Here is actually the action of this step
    public override void Play()
    {
        base.Play();
        AudioManager.Play(audioClip);
    }
}
```

</details>

<details>

<summary>I have my own DOTween extensions, can I use that? </summary>

Absolutely! The same as the step, you can add any new DOTween action by extending `DOTweenActionBase`. In order to avoid any performance issues all the tweens are created on the PrepareToPlay method on Awake, and are paused.

```c#
[Serializable]
public sealed class ChangeMaterialStrengthDOTweenAction : DOTweenActionBase
{
    public override string DisplayName => "Change Material Strength";
        
    public override Type TargetComponentType => typeof(Renderer);

    [SerializeField, Range(0,1)]
    private float materialStrength = 1;

     public override bool CreateTween(GameObject target, float duration, int loops, LoopType loopType)
     {
        Renderer renderer = target.GetComponent<Renderer>();
        if (renderer == null)
            return false;

        TweenerCore<float, float, FloatOptions> materialTween = renderer.sharedMaterial.DOFloat(materialStrength, "Strength", duration);
        
        SetTween(materialTween, loops, loopType);
        return true;
    }
}
```

![custom-tween-action](https://user-images.githubusercontent.com/600419/109774425-3965a280-7bf8-11eb-9bfe-90b0be8b8617.gif)

</details>

<details>
    <summary>Using custom animation curve as easing </summary>
    
You can use the Custom ease to define an *AnimationCurve* for the Tween.
    
![custom-ease](https://user-images.githubusercontent.com/600419/109780020-7af94c00-7bfe-11eb-8f0f-52480dd97ea3.gif)

</details>

<details>
   <summary>What are the differences between the initialization settings</summary>
	
- <kbd>None</kbd> *Don't do anything on the AnimationSequencer Awake method*	
- <kbd>PrepareToPlayOnAwake</kbd> *This will make sure the Tweens that are from are prepared to play at the intial value on Awake.*
- <kbd>PlayOnAwake</kbd> Will play the tween on Awake.*
   
</details>

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


