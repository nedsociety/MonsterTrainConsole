[toc]

# Unit Commands List

## Unit.Id

Turn on/off overlay for units' instance ID. Instance ID can be used for other commands.

### Arguments

`Unit.Id onOff`

- `onOff`: Either 'on' or 'off'.

### Examples

- `Unit.Id on`: Show the overlay for unit instance ID.
- `Unit.Id off`: Hide the overlay.



## Unit.Kill

Kill a unit.

### Arguments

`Unit.Kill unitInstanceId`

- `unitInstanceId`: Instance ID of the unit.

### Examples

- `Unit.Kill e5`: Kill enemy unit with instance ID E5.
- `Unit.Kill p0`: Kill your Pyre.

### Notes

- Must be used in battle.
- Use Unit.Id to find the instance ID for a unit.



## Unit.SetAttack

Set a unit's attack stat.

### Arguments

`Unit.SetAttack unitInstanceId attack`

- `unitInstanceId`: Instance ID of the unit.
- `attack`: Attack value.

### Examples

- `Unit.SetAttack e5 0`: Make enemy unit with instance ID E5 unattackable.
- `Unit.SetAttack p0 10000`: Make your Pyre one-punch man.

### Notes

- Must be used in battle.
- Use Unit.Id to find the instance ID for a unit.



## Unit.SetHp

Set a unit's HP stat.

### Arguments

`Unit.SetHp unitInstanceId hp`

- `unitInstanceId`: Instance ID of the unit.
- `hp`: HP value.

### Examples

- `Unit.SetHp e5 1`: Make enemy unit with instance ID E5's HP to 1.
- `Unit.SetHp p0 99999`: Fully heal your Pyre.

### Notes

- Must be used in battle.
- Use Unit.Id to find the instance ID for a unit.
- Cannot increase HP beyond a unit's max HP. Use Unit.SetMaxHp to adjust it.
- Setting HP to 0 effectively kills the unit.



## Unit.SetMaxHp

Set a unit's max HP stat.

### Arguments

`Unit.SetMaxHp unitInstanceId maxHp`

- `unitInstanceId`: Instance ID of the unit.
- `maxHp`: Max HP value.

### Examples

- `Unit.SetMaxHp e5 1`: Make enemy unit with instance ID E5's max HP to 1.
- `Unit.SetMaxHp p0 99999`: Make your Pyre's max HP to 99,999.

### Notes

- Must be used in battle.
- Use Unit.Id to find the instance ID for a unit.



## Unit.ListStatusEffects

List available status effects.

### Arguments

`Unit.ListStatusEffects [search]`

- `search`: Text to search.

### Examples

- `Unit.ListStatusEffects`: Show all status effects available.
- `Unit.ListStatusEffects weakness`: List status effects related to damage weakness.



## Unit.AddStatusEffect

Apply status effect to a unit.

### Arguments

`Unit.AddStatusEffect unitInstanceId statusEffect [stacks]`

- `unitInstanceId`: Instance ID of the unit.
- `artifactName`: The name/ID of the status effect to apply.
- `stacks`: Number of stacks to apply. Default is 1.

### Examples

- `Unit.AddStatusEffect e5 dazed 5`: Make enemy unit with instance ID E5 dazed for five turns.
- `Unit.AddStatusEffect p1 armor 200`: Give 200 armor to your unit with instance ID P1.

### Notes

- Must be used in battle.
- Use Unit.Id to find the instance ID for a unit.
- Use Unit.ListStatusEffects to find the name of status effect.



## Unit.ListUnits

List available units.

### Arguments

`Unit.ListStatusEffects [search]`

- `search`: Text to search.

### Examples

- `Unit.ListUnits`: Show all units available.
- `Unit.ListUnits imp`: Show Imp units.



## Unit.Spawn

Spawn a new unit.

### Arguments

`Unit.Spawn unitId team [floor]`

- `unitId `: The name/ID of a unit.
- `team`: Either 'player' or 'enemy'.
- `floor`: The floor index to spawn a unit. If unspecified, it will spawn on the current floor.
  - The floor index is one of 0, 1, 2 or 3, where 0 corresponds to the bottom floor and 3 to the Pyre room.

### Examples

- `Unit.Spawn apeximp player`: Spawn Apex Imp to your side on the current floor.
- `Unit.Spawn treasurebaglvl3gold enemy 3`: Create a collector right in front of your Pyre.

### Notes

- Must be used in battle.
- Use Unit.ListUnits to find the name of a unit.
- Player units are spawned with Cardless status effect.

