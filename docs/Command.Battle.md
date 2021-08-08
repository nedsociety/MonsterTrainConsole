[toc]

# Battle Commands List

## Battle.SetEchoes

Set the number of Charged Echoes (![](Echo.png)) on a floor.

### Arguments

`Battle.SetEchoes echoes [floor]`

- `echoes`: The number of Charged Echoes to set.
- `floor`: The floor index to set Echoes. If unspecified, it will set on the current floor.
  - The floor index is one of 0, 1, 2 or 3, where 0 corresponds to the bottom floor and 3 to the Pyre room.

### Examples

- `Battle.SetEchoes 10`: Set the number of echoes on the current floor to 10.
- `Battle.SetEchoes 0 0 `: Remove all echoes on the bottom floor.

### Notes

- Must be used in battle.
- The current run must have DLC (The Last Divinity) activated.



## Battle.SetMaxEchoes

Set the number of Charged Echoes capacity (![](EchoSlot.png)) on a floor.

### Arguments

`Battle.SetMaxEchoes maxEchoes [floor]`

- `maxEchoes`: The number of Charged Echoes capacity to set.
- `floor`: The floor index to set Echoes capacity. If unspecified, it will set on the current floor.
  - The floor index is one of 0, 1, 2 or 3, where 0 corresponds to the bottom floor and 3 to the Pyre room.

### Examples

- `Battle.SetMaxEchoes 10`: Set the number of echoes capacity on the current floor to 10.
- `Battle.SetMaxEchoes 10 0 `: Remove all echoes on the bottom floor.

### Notes

- Must be used in battle.
- The current run must have DLC (The Last Divinity) activated.



## Battle.ListTrials

List available Trials.

### Arguments

`Battle.ListTrials [search]`

- `search`: Text to search.

### Examples

- `Battle.ListTrials`: Show all trials available.
- `Battle.ListTrials Seraph`: Show trials that are related to Seraph's battle.



## Battle.AddTrial

Add a trial into the current battle.

### Arguments

`Battle.AddTrial trialName`

- `trialName`: The name/ID of the trial to add.

### Examples

- `Battle.AddTrial AddMultistrike`: Apply Seal of Aggression to the current battle.
  - Seal of Aggression: Non-boss enemy units get **Multistrike**.
- `Battle.AddTrial "explosive sigil"`: Apply Explosive Sigil to the current battle.
  - Explosive Sigil: Enemy units deal 1 damage to the front unit on death.

### Notes

- Must be used in battle.
- Use Battle.ListTrials to find the name of a trial.



## Battle.Win

Win the current battle.

### Examples

- `Battle.Win`

### Notes

- Must be used in battle.

