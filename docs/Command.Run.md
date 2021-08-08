[toc]

# Run Commands List

## Run.SetSeed

Force any single player runs created from now on to use a specific RNG seed.

### Arguments

`Run.SetSeed seed`

- `seed`: The seed value to force, or 0 to deactivate.

### Examples

- `Run.SetSeed 123456789`: Forces new single player runs from now on to use the RNG seed 123456789.
- `Run.SetSeed 0`: Deactivate the effect of this command so that new runs from now on uses random RNG seeds.

### Notes

- The current run is unaffected. Only new single player runs created afterward get affected.
- The effect of this command persists through the game process (until it exits). If you want to disable the effect, use `Run.SetSeed 0` or restart the game.
- Use Run.GetSeed to obtain a seed value of the known runs.



## Run.GetSeed

Retrieve RNG seed from the current single player run.

### Examples

- `Run.GetSeed`

### Notes

- A single player save must exist and loaded at least once. Use this command inside the run to make sure.



## Run.AdjustGold

Adjust gold by given amount. 

### Arguments

`Run.AdjustGold amount`

- `amount`: Amount of gold to add from the current value. Can be negative.

### Examples

- `Run.AdjustGold 1000`: Gain 1,000 gold from nowhere.
- `Run.AdjustGold -1000`: Spend 1,000 gold to nowhere.

### Notes

- Must be used in the run.



## Run.SetPactShards

Set the number of Pact Shards.

### Arguments

`Run.SetPactShards pactShards`

- `pactShards`: The number of Pact Shards to set.

### Examples

- `Run.SetPactShards 100`: Set Pact Shards to 100.
- `Run.SetPactShards 0`: Remove all Pact Shards.

### Notes

- Must be used in the run.
- The current run must have DLC (The Last Divinity) activated.
- Note that it takes time to compute Shard-enhanced stats for bosses, proportionately to the number of Shards. If you set too much shards, you'll effectively freeze the game when they get spawned.



## Run.ListMutators

List available Mutators.

### Arguments

`Run.ListMutators [search]`

- `search`: Text to search.

### Examples

- `Run.ListMutators`: Show all Mutators available.
- `Run.ListMutators pyre`: Show Mutators that are related to the Pyre.



## Run.AddMutator

Add a Mutator into the current run.

### Arguments

`Run.AddMutator mutatorName`

- `mutatorName`: The name/ID of the Mutator to add.

### Examples

- `Run.AddMutator MultistrikePyre`: Apply Multi-Pyre to the current run.
  - Multi-Pyre: Your Pyre gets **Multistrike** 1.
- `Run.AddMutator "vampiric touch"`: Apply Vampiric Touch the current run.
  - Vampiric Touch: All units enter with **Lifesteal** 3.

### Notes

- Must be used in the run.
- Use Run.ListMutators to find the name of a Mutator.



## Run.ListArtifacts

List available Artifacts.

### Arguments

`Run.ListArtifacts [search]`

- `search`: Text to search.

### Examples

- `Run.ListMutators`: Show all Artifacts available.
- `Run.ListMutators stygian`: Show Stygian Guard Artifacts.



## Run.AddArtifact

Add an Artifact into the current run.

### Arguments

`Run.AddArtifact artifactName`

- `artifactName`: The name/ID of the Artifact to add.

### Examples

- `Run.AddArtifact PyreArmor_Divine`: Obtain Divine Pyrewall.
  - Divine Pyrewall: Your Pyre starts the next battle with **Armor 30**. Remove this artifact after 1 battle.
- `Run.AddArtifact "founding seal"`: Obtain Founding Seal.
  - Founding Seal: **Incant** abilities trigger an additional time.

### Notes

- Must be used in the run.
- Use Run.ListArtifacts to find the name of an Artifact.



## Run.ListCards

List available cards.

### Arguments

`Run.ListCards [search]`

- `search`: Text to search.

### Examples

- `Run.ListCards`: Show all cards available.
- `Run.ListCards imp`: Show Imp cards.



## Run.AddCard

Add a raw, unmodified card to the deck.

### Arguments

`Run.AddCard cardName [drawImmediately]`

- `cardName `: The name/ID of the card to add.
- `drawImmediately`: Either 'yes' or 'no'. If this cheat is used in battle and this argument is 'yes', immediately draw the added card to hand.

### Examples

- `Run.AddCard ApexImp`: Obtain Apex Imp.
- `Run.AddCard AwokensRailSpike yes`: Obtain Awoken’s Rail Spike and draw it into your hand.

### Notes

- Must be used in the run.
- Use Run.ListCards to find the name of a card.
- `drawImmediately = yes` only works in battle.
- The deck is copied to the draw pile only once at the beginning of the battle. If you use this command during a battle without `drawImmediately = yes`, you have no way to access the added card in that battle. It should be accessible in the next battle though.
- Use [Stoker Deck Editor](https://steamcommunity.com/sharedfiles/filedetails/?id=2221577882) for more advanced deck editing features.





## Run.Win

Win the current run.

### Examples

- `Run.Win`

### Notes

- Must be used in the run.

