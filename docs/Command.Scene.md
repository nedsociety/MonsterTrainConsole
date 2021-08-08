[toc]

# Scene Commands List

## Scene.ListBattles

List available battles.

### Arguments

`Scene.ListBattles [search]`

- `search`: Text to search.

### Examples

- `Scene.ListBattles`: Show all battles available.
- `Scene.ListBattles Level8`: Show battles that can appear as 8th one (that is, Seraph's variants).



## Scene.StartBattle

Start a battle.

### Arguments

`Scene.StartBattle [battleName]`

- `battleName`: The name/ID of the battle to begin. If unspecified, the sandbox battle (`Level0EmptyTest`) will be chosen.

### Examples

- `Scene.StartBattle`: Start the sandbox battle.
- `Scene.StartBattle Level9TrueFinalBoss`: Start the TLD fight.

### Notes

- Must be used in the run.
- Use Scene.ListBattles to find the name of a battle.
- **Imp-ortant note**: This command puts the game into a completely test-driving mode, disabling the ability to continue a normal run:
  - When the battle is over, the game is over. You cannot proceed.
  - Your current run ***and all new runs afterward*** will have every battle substituted to the battle you've specified with this command.
- The effect of this command persists through the game process (until it exits). As there's no way to deactivate the test mode with commands, you'll need to restart the game to get out of it.



## Scene.ListEvents

List available Concealed Cavern events.

### Arguments

`Scene.ListEvents [search]`

- `search`: Text to search.

### Examples

- `Scene.ListEvents`: Show all events available.
- `Scene.ListEvents malicka`: Show events that features the mischievous Titan.



## Scene.StartEvent

Start a Concealed Cavern event.

### Arguments

`Scene.StartEvent eventName`

- `eventName`: The name/ID of the event to begin.

### Examples

- `Scene.StartEvent BuildCard`: Start the Olde Magic event.
- `Scene.StartEvent SpellMerge`: Start the Nexus Spike event.

### Notes

- A single player save must exist and loaded at least once. Use this command inside the run to make sure.
- Use Scene.ListEvents to find the name of an event.
- This command seems to softlock the run when used in battle. Use it in the map screen for safety.

