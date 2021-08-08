[toc]

# Unlocks Commands List

## Unlocks.UnlockCovenantRank

Increase the maximum Covenant Rank.

### Arguments

`Unlocks.UnlockCovenantRank [increaseBy]`

- `increaseBy`: Number of Ranks to increase. Default is 1.

### Examples

- `Unlocks.UnlockCovenantRank`: Unlock the Covenant Rank by one.
- `Unlocks.UnlockCovenantRank 25`: Fully unlock the Covenant Rank.



## Unlocks.CompendiumDiscoverAll

Mark all cards, artifacts and Champion upgrades as discovered in the Compendium.

### Examples

- `Unlocks.CompendiumDiscoverAll`



## Unlocks.CompendiumDivineMasterAllCards

Divine master all cards in the Compendium.

### Examples

- `Unlocks.CompendiumDivineMasterAllCards`



## Unlocks.CompendiumMarkAllDivineVictories

Mark all Divine victories for each clan combo on Covenant Rank 25 in the Compendium.

### Examples

- `Unlocks.CompendiumMarkAllDivineVictories`



## Unlocks.ListClans

List available clans.

### Arguments

`Unlocks.ListClans [search]`

- `search`: Text to search.

### Examples

- `Unlocks.ListClans`: Show all clans.



## Unlocks.ClanSetLevel

Set level of a clan.

### Arguments

`Unlocks.ClanSetLevel clan level`

- `clan`: The name/ID of the clan.
- `level`: Level to set.

### Examples

- `Unlocks.ClanSetLevel stygian 10`: Set Stygian Guard's level to maximum level.

### Notes

- Use Unlocks.ListClans to find the name of a clan.



## Unlocks.ClanAddXP

Add XP to a clan.

### Arguments

`Unlocks.ClanAddXP clan amount`

- `clan`: The name/ID of the clan.
- `amount`: Amount of XP to add.

### Examples

- `Unlocks.ClanAddXP stygian 1000`: Add 1,000 XP to Stygian Guard.

### Notes

- Use Unlocks.ListClans to find the name of a clan.



## Unlocks.ClanUnlockAll

Unlock all clans by skipping the tutorial mission, unlocking Covenant Rank 25, and fulfilling all clan requirements. All clans are unlocked to their maximum level.

### Examples

- `Unlocks.ClanUnlockAll`

