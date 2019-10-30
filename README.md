# LeopotamGroupLibraryUnity
LeopotamGroup library for unity3d engine.

#### Unity tested version: 2017.3.0f3.

----------------------------------------------------------------------------

# Installation

## As unity module
This repository can be installed as unity module directly from git url. In this way new line should be added to `Packages/manifest.json`:
```
"com.leopotam.group-library": "https://github.com/Leopotam/LeopotamGroupLibraryUnity.git",
```
By default last released version will be used. If you need trunk / developing version then `develop` name of branch should be added after hash:
```
"com.leopotam.group-library": "https://github.com/Leopotam/LeopotamGroupLibraryUnity.git#develop",
```

## As .unitypackage or source code
[Releases] (https://github.com/Leopotam/LeopotamGroupLibraryUnity/releases)
Or just clone this repo into your `Assets/` folder.

----------------------------------------------------------------------------

> Examples moved to [separate repository](https://github.com/Leopotam/LeopotamGroupLibraryUnity.Examples/)

----------------------------------------------------------------------------

## CODE STRUCTURE

All code separated to independent subsystems (folders),
you can remove unnecessary code for current project:

----------------------------------------------------------------------------

* Analytics

Google Analytics.

----------------------------------------------------------------------------

* Animation

Animator helpers, for updating parameters during graph execution flow.

----------------------------------------------------------------------------

* Collections

Additional collections or replacements of standards with target on performance.

----------------------------------------------------------------------------

* Common

Common helpers, uses by other subsystems.

----------------------------------------------------------------------------

* EditorHelpers

Special helpers: show fps, screen capturing of current platform for multiple
aspects, csv import (for ex, Google Docs), unlit shader generation, etc.

----------------------------------------------------------------------------

* Events

EventBus realization with cancelable events, BehaviourTree (action, sequence,
parallel, selector, condition and custom).

----------------------------------------------------------------------------

* Fx

Visual / audial effect helpers: sound / music manipulations, screen fading.

----------------------------------------------------------------------------

* Localization

Localization support.

----------------------------------------------------------------------------

* Math

Additional types, 'mersenne twister'-based RNG, fast xor-shift RNG, etc.

----------------------------------------------------------------------------

* Pooling

Pooling support for any prefabs.

----------------------------------------------------------------------------

* Protection

Protection for Int, Long, Float types from in-memory searching.

----------------------------------------------------------------------------

* Scripting

Embedded scripting engine, optimized for low gc usage.

----------------------------------------------------------------------------

* Serialization

Csv deserialization, Json serialization / deserialization with support of
structs and nested objects (lists, arrays, structs, etc).

----------------------------------------------------------------------------

* SystemUi

Helpers / performance replacements for uGui. DataBinding.

----------------------------------------------------------------------------

* Threading

Background worker (except WebGl).

----------------------------------------------------------------------------

* Tutorials

Step by step behaviour helpers, useful for creating ingame tutorial or any
other behaviour with ordered / dependent execution. Progress can be saved.

----------------------------------------------------------------------------

* Tweening

Simple tweening.

----------------------------------------------------------------------------

## LICENSE

The software released under the terms of the MIT license. Enjoy.

----------------------------------------------------------------------------