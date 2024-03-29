# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

## [1.0.0]
### Add
- Profilable methods can be displayed on the Debug Panel, just add the Attribute `[ProfilableMethodttribute]` to any Methoid.
- Added support for domain reload
### Changed
- Cleanup and refactor for release.

## [0.3.3]
### Changed
- Dusabled Native share on Editor
- Removed unused text on MultilineInputField

## [0.3.2]
### Changed
- Fixed SOC integration

## [0.3.1]
### Added
- Added support for [Yasirkula Native Share](https://github.com/yasirkula/UnityNativeShare) on mobile devices for `Multiline` debuggable field
- Multiline texts are not using the InputField to allow copy

### Changed
- Fixed search using full path
- Fixed SOC integration
- Fixed HotKey display too close to corners
- Parent Path will not be displayed on each item while performing search
- Fixed issue while Toggle Debug panel on Mobile was not working


## [0.3.0]
### Changed
- Refactored to be a single static access point
- Implemented display for the Hotkeys for actions
- Improved namespaces
- Removed extra dependencies
- `DebuggableField` now have the option to set as `ReadOnly`
- Refactored `DebuggableInvokable`
- 

### Removed 
- Removed the `ReadOnlyFieldAttribute`

## [0.2.3]
### Changed
- Fixed SOC wrong usage when not enabled
- Fixed Prefab started disabled
- Renamed `DebugPanel` to `DebugPanelService` for consistency 

### Added 
- Added `ReadOnlyFieldAttribute` to make fields only readable
- Ability to display `Object` display name on the `DebugPanel`

## [0.2.2]
### Changed
- Fixed SOC Support not working correctly
- Added support for [ServiceLocator](https://github.com/brunomikoski/ServiceLocator)
- Refactored Favorite functionality
- Renamed some files to keep consistent
- Cleanup of some classes
- Fixed possible NullReference when trying to display `GetValue<>` 
- Changed Favorite Icon

### Added
- New Favorites folder that shows everything saved as favorite

## [0.2.1]
### Changed
- Convert most of the classes to internal, so avoids appearing on code completion in other places
- Fixed meta files and Readme file.
 
## [0.2.0]
### Changed
- Refactored the whole system
- Implemented new UI based on this [UnityDebugSheet Package](https://github.com/Haruma-K/UnityDebugSheet) 

## [0.1.3]
### Changed
- Fixed missing SCO conditional directive

## [0.1.2]
### Added
- Added Debuggable Toggle for `bools`

### Changed
- Refactored a little bit the layout items.

## [0.1.1]
### Added
- Added Debuggable Dropdown for `Enums` and `ScriptableObjectItems`
- Added Debuggable Slider, if a `DebuggableField` is of type `float` or `int` and has the `Range` attribute will be displayed as a Slider now.  
- Added support for [Scriptable Object Collection](https://github.com/brunomikoski/ScriptableObjectCollection), will be displayed as a dropdown

### Changed
- Refactored a little bit the layout items.

## [0.1.0]
### Added
- Initial working version


[1.0.0]: https://github.com/brunomikoski/Debug-Panel/releases/tag/v1.0.0s
[0.3.2]: https://github.com/brunomikoski/Debug-Panel/releases/tag/v0.3.2
[0.3.1]: https://github.com/brunomikoski/Debug-Panel/releases/tag/v0.3.1
[0.3.0]: https://github.com/brunomikoski/Debug-Panel/releases/tag/v0.3.0
[0.2.3]: https://github.com/brunomikoski/Debug-Panel/releases/tag/v0.2.3
[0.2.2]: https://github.com/brunomikoski/Debug-Panel/releases/tag/v0.2.2
[0.2.1]: https://github.com/brunomikoski/Debug-Panel/releases/tag/v0.2.1
[0.2.0]: https://github.com/brunomikoski/Debug-Panel/releases/tag/v0.2.0
[0.1.3]: https://github.com/brunomikoski/Debug-Panel/releases/tag/v0.1.3
[0.1.2]: https://github.com/brunomikoski/Debug-Panel/releases/tag/v0.1.2
[0.1.1]: https://github.com/brunomikoski/Debug-Panel/releases/tag/v0.1.1
[0.1.0]: https://github.com/brunomikoski/Debug-Panel/releases/tag/v0.1.0
