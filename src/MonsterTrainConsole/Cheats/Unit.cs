using HarmonyLib;
using SickDev.CommandSystem;
using System;
using System.Collections;
using UnityEngine;

namespace MonsterTrainConsole
{
    public static class Unit
    {
        [Command(
            description = "Turn on/off overlay for units' instance ID. Instance ID can be used for other commands. @onOff: either 'on' or 'off'.",
            useClassName = true
        )]
        public static void Id(string onOff)
        {
            bool enable = onOff.ParseOnOff();

            var combatManager = SingletonAccessor.CombatManager;
            if (combatManager.CheatUnitInfoEnabled != enable)
                combatManager.Cheat_ToggleUnitInfo();

            DevConsole.singleton.Log(enable ? "Unit ID overlay is enabled." : "Unit ID overlay is disabled.");
        }

        internal static int BypassSubtypeCheckForPyreCount { get; set; }

        [Command(
            description = "Kill a unit. @unitInstanceId: Instance ID of the unit.",
            useClassName = true
        )]
        public static void Kill(string unitInstanceId)
        {
            CheatCommon.AssertInBattle();
            unitInstanceId = unitInstanceId.ToUpperInvariant();

            CheatManager.CheatTestResult Try()
            {
                var saveManager = SingletonAccessor.SaveManager;
                var combatManager = SingletonAccessor.CombatManager;

                CharacterState character = combatManager.Cheat_GetCharacterById(unitInstanceId);
                if (character == null)
                    return CheatManager.CheatTestResult.UnitNotFound;

                bool isPyre = character.IsPyreHeart();
                if (isPyre)
                {
                    // By default a CardEffect that does NOT target specific subtypes (e.g., Sacrifice Imps) passes the target
                    // filtering logic at MonsterManager.DoesCharacterPassSubtypeCheck() for all EXCEPT the PyreHeart.
                    // We'll override the logic to make sacrifice effects work on it.
                    // Note that BypassSubtypeCheckForPyreCount set to 2 because it checks twice:
                    // one for testing if effect is valid, and one for actually applying the effect.
                    BypassSubtypeCheckForPyreCount = 2;

                    // Also, it seems that in case of the game over it does not properly call the finish callback,
                    // so it does not "unbusy" the console :( So we'll skip it.
                }
                else
                    CheatCommon.BusyStart();

                // Use Sacrifice effect
                var coroutine = Traverse.Create(saveManager).Method("Cheat_StartCombatEffectCoroutine",
                    (Action)delegate
                    {
                        CardEffectData cardEffectData = new CardEffectData(
                            "CardEffectSacrifice", character.GetSourceCharacterData(), character.GetTeamType()
                        );
                        cardEffectData.Cheat_SetTargetMode(TargetMode.DropTargetCharacter);
                        CardEffectState cardEffectState = new CardEffectState();
                        cardEffectState.Setup(cardEffectData, character.GetTeamType(), saveManager);
                        combatManager.Cheat_QueueEffectOnCharacter(
                            cardEffectState, character, delegate {
                                if (!isPyre)
                                    CheatCommon.BusyEnd();
                            }
                        );
                    }
                ).GetValue<IEnumerator>();

                saveManager.StartCoroutine(coroutine);
                return CheatManager.CheatTestResult.Pass;
            }

            CheatCommon.ReportCheatTestResult(Try(), "The unit is killed.");
        }

        [Command(
            description = "Set a unit's attack stat. @unitInstanceId: Instance ID of the unit. @attack: attack value.",
            useClassName = true
        )]
        public static void SetAttack(string unitInstanceId, int attack)
        {
            CheatCommon.AssertInBattle();
            unitInstanceId = unitInstanceId.ToUpperInvariant();

            SingletonAccessor.SaveManager.Cheat_UnitSetAttack(unitInstanceId.ToUpperInvariant(), attack, out var result);
            CheatCommon.ReportCheatTestResult(result, $"The unit's attack damage is set to {attack}.");
        }

        [Command(
            description = "Set a unit's HP stat. @unitInstanceId: Instance ID of the unit. @hp: HP value.",
            useClassName = true
        )]
        public static void SetHp(string unitInstanceId, int hp)
        {
            CheatCommon.AssertInBattle();
            unitInstanceId = unitInstanceId.ToUpperInvariant();

            if (hp == 0)
            {
                // Setting HP cannot kill units properly; short-circuit it to Kill().
                Kill(unitInstanceId);
                return;
            }
            else if (hp < 0)
                throw new ArgumentException("@hp cannot be negative");
            
            CheatManager.CheatTestResult Try()
            {
                var saveManager = SingletonAccessor.SaveManager;
                var combatManager = SingletonAccessor.CombatManager;

                CharacterState characterState = combatManager.Cheat_GetCharacterById(unitInstanceId);

                if (characterState == null)
                    return CheatManager.CheatTestResult.UnitNotFound;
                else if (characterState.IsPyreHeart())
                    Traverse.Create(saveManager).Method("SetTowerHP", hp).GetValue();
                else
                {
                    // type: CharacterState.StateInformation (private)
                    var PrimaryStateInformation = Traverse.Create(characterState).Property("PrimaryStateInformation").GetValue();
                    var maxHp = Traverse.Create(PrimaryStateInformation).Property("MaxHp").GetValue<int>();
                    Traverse.Create(PrimaryStateInformation).Property("Hp").SetValue(Mathf.Clamp(hp, 0, maxHp));
                    characterState.UpdateCharacterStateUI();
                }

                return CheatManager.CheatTestResult.Pass;
            }

            CheatCommon.ReportCheatTestResult(Try(), $"The unit's HP is changed.");
        }

        [Command(
            description = "Set a unit's max HP stat. @unitInstanceId: Instance ID of the unit. @maxHp: max HP value.",
            useClassName = true
        )]
        public static void SetMaxHp(string unitInstanceId, int maxHp)
        {
            CheatCommon.AssertInBattle();
            unitInstanceId = unitInstanceId.ToUpperInvariant();

            if (maxHp <= 0)
                throw new ArgumentException("@maxHp must be positive.");

            CheatManager.CheatTestResult Try()
            {
                var saveManager = SingletonAccessor.SaveManager;
                var combatManager = SingletonAccessor.CombatManager;

                CharacterState characterState = combatManager.Cheat_GetCharacterById(unitInstanceId);

                if (characterState == null)
                    return CheatManager.CheatTestResult.UnitNotFound;
                else if (characterState.IsPyreHeart())
                {
                    var ActiveSaveData = Traverse.Create(saveManager).Property("ActiveSaveData").GetValue<SaveData>();
                    ActiveSaveData.SetMaxTowerHP(maxHp);
                    Traverse.Create(saveManager).Method("SetTowerHP", saveManager.GetTowerHP()).GetValue();
                }
                else
                {
                    // type: CharacterState.StateInformation (private)
                    var PrimaryStateInformation = Traverse.Create(characterState).Property("PrimaryStateInformation").GetValue();
                    Traverse.Create(PrimaryStateInformation).Property("MaxHp").SetValue(maxHp);
                    var hpStat = Traverse.Create(PrimaryStateInformation).Property("Hp");
                    if (hpStat.GetValue<int>() > maxHp)
                        hpStat.SetValue(maxHp);
                    characterState.UpdateCharacterStateUI();
                }

                return CheatManager.CheatTestResult.Pass;
            }

            CheatCommon.ReportCheatTestResult(Try(), $"The unit's max HP is changed.");
        }

        static TabularDataAccessor<StatusEffectData> statusEffectDataAccessor = new TabularDataAccessor<StatusEffectData>(
            "status effects",
            () => SingletonAccessor.StatusEffectManager.GetAllStatusEffectsData().GetStatusEffectData(),
            null,
            // Replace space to hypen because space needs quoting on the console
            ("Name", element => element.GetStatusId().Replace(' ', '-')),
            ("Localized", element => StatusEffectManager.GetLocalizedName(element.GetStatusId()))
        );

        [Command(
            description = "List available status effects. @search: (optional) text to search.",
            useClassName = true
        )]
        public static void ListStatusEffects(string search = "")
        {
            statusEffectDataAccessor.PrintListToDevConsole(search);
        }

        [Command(
            description = "Apply status effect to a unit. @unitInstanceId: Instance ID of the unit. @statusEffect: Name of the status effect. @stack: (Optional) Number of stacks to add. Default is 1. Can be negative to remove stacks.",
            useClassName = true
        )]
        public static void AddStatusEffect(string unitInstanceId, string statusEffect, int stacks = 1)
        {
            CheatCommon.AssertInBattle();
            unitInstanceId = unitInstanceId.ToUpperInvariant();

            var statusEffectEntry = statusEffectDataAccessor.SelectOne(statusEffect, nameof(statusEffect));
            CheatCommon.BusyStart();
            SingletonAccessor.SaveManager.Cheat_UnitSetStatus(
                unitInstanceId, statusEffectEntry.GetStatusId(), stacks, delegate { CheatCommon.BusyEnd(); }, out var result
            );

            CheatCommon.ReportCheatTestResult(
                result, $"Applied {StatusEffectManager.GetLocalizedName(statusEffectEntry.GetStatusId())} by {stacks}."
            );
        }

        static TabularDataAccessor<CharacterData> unitDataAccessor = new TabularDataAccessor<CharacterData>(
            "units",
            () => SingletonAccessor.AllGameData.GetAllCharacterData(),
            null,
            ("ID", element => element.GetID()),
            ("Name", element => element.name),
            ("Localized", element => element.GetName())
        );

        [Command(
            description = "List available units. @search: (optional) text to search.",
            useClassName = true
        )]
        public static void ListUnits(string search = "")
        {
            unitDataAccessor.PrintListToDevConsole(search);
        }

        [Command(
            description = "Spawn a unit. @unitId: Name/ID of the unit. @team: Either 'player' (monster) or 'enemy' (hero). @floor: The floor index to spawn. If unspecified, it will be spawned on the current floor.",
            useClassName = true
        )]
		public static void Spawn(string unitId, string team, int floor = -1)
		{
            CheatCommon.AssertInBattle();
            floor = CheatCommon.SanitizeFloorArgument(floor);

            bool isPlayerTeam;
            switch(team.ToLowerInvariant())
            {
                case "monster":
                case "player":
                    isPlayerTeam = true;
                    break;
                case "hero":
                case "enemy":
                    isPlayerTeam = false;
                    break;
                default:
                    throw new ArgumentException(
                        $"@team must be either 'player' or 'enemy'. Got '{team}'."
                    );
            }

            var saveManager = SingletonAccessor.SaveManager;
            CharacterData characterData = unitDataAccessor.SelectOne(unitId, nameof(unitId));

            CheatCommon.BusyStart();
            if (isPlayerTeam)
                saveManager.Cheat_SpawnMonster(characterData, floor, delegate { CheatCommon.BusyEnd(); });
            else
                saveManager.Cheat_SpawnHero(characterData, floor, delegate { CheatCommon.BusyEnd(); });

            DevConsole.singleton.Log($"Spawning '{characterData.GetName()}' in floor {floor} for team {team}.");
		}
	}

    [HarmonyPatch(typeof(MonsterManager))]
    [HarmonyPatch(nameof(MonsterManager.DoesCharacterPassSubtypeCheck), new Type[] { typeof(CharacterState), typeof(SubtypeData) })]
    class MonsterManager_DoesCharacterPassSubtypeCheck
    {
        public static bool Prefix(ref bool __result, CharacterState character, SubtypeData subtype)
        {
            if (Unit.BypassSubtypeCheckForPyreCount > 0 && (subtype == null || subtype.IsNone) && character.IsPyreHeart())
            {
                __result = true;
                --Unit.BypassSubtypeCheckForPyreCount;
                return false;
            }
            return true;
        }
    }
}
