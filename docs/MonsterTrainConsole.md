

[toc]

# MonsterTrainConsole

## DISCLAIMER

*This mod intents to bring the developer's testing facilities into the game for testing and cheating. As is usual with dev tools in other games, the game might temporarily **AND/OR PERMANENTLY** break by using any of these commands, and the mod devs are **NOT RESPONSIBLE** for any damage it causes. Use at your own risk. Backup your save before using the commands. **Never use these commands before/after entering multiplayer sessions**.*

## Installation

Make sure that you have all the prerequisites below:

- [DLC: The Last Divinity](https://store.steampowered.com/app/1359320/Monster_Train_The_Last_Divinity_DLC/)
- [Mod: Mod Loader](https://steamcommunity.com/sharedfiles/filedetails/?id=2187468759)

Then subscribe the mod at [workshop]().

## Usage

Use backtick (`) key to open the console.

Commands are organized by categories. For example, if you want to find a command that leads to an instant win of the current battle, find one in Battle category (Battle.Win).

## List of Commands

For a quickhelp for the usage of each command, use help command. Example: `help Battle.Win`. For detailed usage, click on the following links.

### [Battle category](Command.Battle.md)

Commands that modify the current battle.

- **Battle.SetEchoes**: Set the number of Charged Echoes (![](Echo.png)) on a floor.
- **Battle.SetMaxEchoes**: Set the number of Charged Echoes capacity (![](EchoSlot.png)) on a floor.
- **Battle.ListTrials**: List available Trials.
- **Battle.AddTrial**: Add a trial into the current battle.
- **Battle.Win**: Win the current battle.

### [Hand category](Command.Hand.md)

Commands that manipulate your hand.

- **Hand.Discard**: Discard a card in hand.
- **Hand.DiscardAll**: Discard ALL cards in hand.
- **Hand.Draw**: Draw cards.

### [Unit category](Command.Unit.md)

Commands that manipulate units in the current battle.

- **Unit.Id**: Turn on/off overlay for units' instance ID. Instance ID can be used for other commands.
- **Unit.Kill**: Kill a unit.
- **Unit.SetAttack**: Set a unit's attack stat.
- **Unit.SetHp**: Set a unit's HP stat.
- **Unit.SetMaxHp**: Set a unit's max HP stat.
- **Unit.ListStatusEffects**: List available status effects.
- **Unit.AddStatusEffect**: Apply status effect to a unit.
- **Unit.ListUnits**: List available units.
- **Unit.Spawn**: Spawn a new unit.

### [Run category](Command.Run.md)

Commands that manipulate the current run.

- **Run.SetSeed**: Force any single player runs created from now on to use a specific RNG seed.
- **Run.GetSeed**: Retrieve RNG seed from the current single player run.
- **Run.AdjustGold**: Adjust gold by given amount.
- **Run.SetPactShards**: Set the number of Pact Shards.
- **Run.ListMutators**: List available Mutators.
- **Run.AddMutator**: Add a Mutator into the current run.
- **Run.ListArtifacts**: List available Artifacts.
- **Run.AddArtifact**: Add an Artifact into the current run.
- **Run.ListCards**: List available cards.
- **Run.AddCard**: Add a raw, unmodified card to the deck.
- **Run.Win**: Win the current run.

### [Map category](Command.Map.md)

Commands that manipulate the map elements.

- **Map.Next**: Skip the next battle and proceed to the next node.
- **Map.Previous**: Backtrack to the previous node.
- **Map.Final**: Skip to the final node (after Seraph battle).
- **Map.Reset**: Reset rewards in the current node so that you may pick them up again.
- **Map.ChangeSide**: Change the branch decision in the the current node.

### [Scene category](Command.Scene.md)

Commands that force you to start specific scenes.

- **Scene.ListBattles**: List available battles.
- **Scene.StartBattle**: Start a battle.
- **Scene.ListEvents**: List available Concealed Cavern events.
- **Scene.StartEvent**: Start a Concealed Cavern event.

### [UI category](Command.UI.md)

Commands that interact with UI elements.

- **UI.Hud**: Show/hide HUD.
- **UI.UnitStats**: Show/hide unit stat displays.
- **UI.Chatters**: Show/hide unit chatters.
- **UI.ShuffleMusic**: Start a random battle music.

### [Unlocks category](Command.Unlocks.md)

Commands that fiddle with unlocks.

**Warning: The effect of these commands persist even if you disable this mod. Use carefully. Make sure to backup your save file before using any of these commands.**

- **Unlocks.UnlockCovenantRank**: Increase the maximum Covenant Rank.
- **Unlocks.CompendiumDiscoverAll**: Mark all cards, artifacts and Champion upgrades as discovered in the Compendium.
- **Unlocks.CompendiumDivineMasterAllCards**: Divine master all cards in the Compendium.
- **Unlocks.CompendiumMarkAllDivineVictories**: Mark all Divine victories for each clan combo on Covenant Rank 25 in the Compendium.
- **Unlocks.ListClans**: List available clans.
- **Unlocks.ClanSetLevel**: Set level of a clan.
- **Unlocks.ClanAddXP**: Add XP to a clan.
- **Unlocks.ClanUnlockAll**: Unlock all clans by skipping the tutorial mission, unlocking Covenant Rank 25, and fulfilling all clan requirements. All clans are unlocked to their maximum level.

### [Debug category](Command.Debug.md)

Commands useful for mod devs.

- **Debug.DirtyBattleState**: Mark the battle state as dirty to force updating the combat preview.
- **Debug.SetGraphicsQuality**: Turn on/off graphics settings individually.
- **Debug.WrapUntranslatedText**: Turn on/off wrapping untranslated strings with the marker.
- **Debug.TimingDisplay**: Turn on/off the timing display window for animation processing.
- **Debug.Timescale**: Modify the global Unity time scale value. Essentially a speedhack.

### [Console category](Command.Console.md)

Commands related to the console usage.

- **help**: Show help for a command. Effectively a quick one-line summary of a command.
- **clear**: Clear the console.
- **Console.BusyGuard**: Turn on/off the console busy guard.

## FAQ

### The console is spitting the error: *Console is busy executing a command. Try later.*

If the processing for previous command is not over, this is normal. Try again after it finishes.

If you think the game is in idle state but the console still refuses to do things, see Console.BusyGuard command.

