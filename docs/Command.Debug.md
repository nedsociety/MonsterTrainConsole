[toc]

# Debug Commands List

## Debug.DirtyBattleState

Mark the battle state as dirty to force updating the combat preview.

### Examples

- `Debug.DirtyBattleState`



## Debug.SetGraphicsQuality

Turn on/off graphics settings individually.

### Arguments

`Debug.SetGraphicsQuality qualitySetting onOff`

- `qualitySetting`: One of the graphics features: 'msaa', 'motionBlur', 'bloom', 'fastBloom', 'lightsCastShadows' or 'iceRefraction'. Or 'all' to turn everything on/off at a time.
- `onOff`: Either 'on' or 'off'.

### Examples

- `Debug.SetGraphicsQuality motionBlur off`: Disable motion blur specifically.
- `Debug.SetGraphicsQuality all on`: Turn every graphics quality options on (equivalent to setting the Quality option to High).



## Debug.WrapUntranslatedText

Turn on/off wrapping untranslated strings with the marker.

### Arguments

`Debug.WrapUntranslatedText onOff`

- `onOff`: Either 'on' or 'off'.

### Examples

- `Debug.WrapUntranslatedText on`: Enable untranslated text indicator.
- `Debug.WrapUntranslatedText off`: Disable untranslated text indicator.

### Notes

- This feature is useful if you're developing mods with custom localization strings. Any untranslated strings will be highlighted with `>>` `<<` markers to indicate the string has no translation in current language. (Example: `French>>Single player<<`)



## Debug.TimingDisplay

Turn on/off the timing display window for animation processing.

### Arguments

`Debug.TimingDisplay onOff`

- `onOff`: Either 'on' or 'off'.

### Examples

- `Debug.WrapUntranslatedText on`: Open the timing display window.
- `Debug.WrapUntranslatedText off`: Close the timing display window.

### Notes

- This feature is useful if you're developing a new effect that affects the animation queue. The timing display will show which part of the animation they're currently processing.



## Debug.Timescale

Modify the global Unity time scale value. Essentially a speedhack.

### Arguments

`Debug.Timescale scale`

- `scale`: The scaling value for new speed. Values smaller than 1 slow down time. Values larger than 1 speed up time.

### Examples

- `Debug.Timescale 0.5`: Make the game slower by half than the usual.
- `Debug.Timescale 2`: Make the game twice faster than the usual.

### Notes

- This option applies to ALL Unity timings. It is independent from combat speed modifiers and they interact multiplicatively.
- A practical usage is to slow down the combat animations, or to increase the speed of UI animations (fade-in/out/etc.).

