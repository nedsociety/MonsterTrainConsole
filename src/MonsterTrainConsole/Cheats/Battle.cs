using SickDev.CommandSystem;
using System;

namespace MonsterTrainConsole
{
    public static class Battle
    {
        [Command(
            description = "Set the number of Charged Echoes. @echoes: the number of Charged Echoes to set. @floor: The floor index to set Echoes. If unspecified, it will set on the current floor.",
            useClassName = true
        )]
        public static void SetEchoes(int echoes, int floor = -1)
        {
            CheatCommon.AssertInBattle();
            CheatCommon.AssertDLCAvailableOn(CheatCommon.DLCAvailability.CurrentActiveSaveData);

            if (echoes < 0)
                throw new ArgumentException($"@echoes cannot be negative.");
            floor = CheatCommon.SanitizeFloorArgument(floor);

            var saveManager = SingletonAccessor.SaveManager;

            CheatCommon.BusyStart();
            saveManager.StartCoroutine(saveManager.Cheat_SetCorruption(echoes, null, floor, delegate { CheatCommon.BusyEnd(); }));

            DevConsole.singleton.Log($"{echoes} Charged Echoes set in floor {floor}.");
        }

        [Command(
            description = "Set the number of Charged Echoes capacity. @maxEchoes: the number of Charged Echoes capacity to set. @floor: The floor index to set Echoes capacity. If unspecified, it will set on the current floor.",
            useClassName = true
        )]
        public static void SetMaxEchoes(int maxEchoes, int floor = -1)
        {
            CheatCommon.AssertInBattle();
            CheatCommon.AssertDLCAvailableOn(CheatCommon.DLCAvailability.CurrentActiveSaveData);

            if (maxEchoes < 0)
                throw new ArgumentException($"@maxEchoes cannot be negative.");
            floor = CheatCommon.SanitizeFloorArgument(floor);

            var saveManager = SingletonAccessor.SaveManager;

            CheatCommon.BusyStart();
            saveManager.StartCoroutine(saveManager.Cheat_SetCorruption(null, maxEchoes, floor, delegate { CheatCommon.BusyEnd(); }));

            DevConsole.singleton.Log($"{maxEchoes} Charged Echoes capacity set in floor {floor}.");
        }

        static TabularDataAccessor<SinsData> trialDataAccessor = new TabularDataAccessor<SinsData>(
            "Trials",
            () => SingletonAccessor.AllGameData.GetAllSinsDatas(),
            null,
            ("ID", element => element.GetID()),
            ("Name", element => element.name),
            ("Localized", element => element.GetName())
        );

        [Command(
            description = "List available Trials. @search: (optional) text to search.",
            useClassName = true
        )]
        public static void ListTrials(string search = "")
        {
            trialDataAccessor.PrintListToDevConsole(search);
        }

        [Command(
            description = "Add a Trial to the the current battle. @trialName: the name/ID of the Trial.",
            useClassName = true
        )]
        public static void AddTrial(string trialName)
        {
            CheatCommon.AssertInBattle();
            
            SinsData trial = trialDataAccessor.SelectOne(trialName, nameof(trialName));
            SingletonAccessor.SaveManager.Cheat_AddSin(trial);
            DevConsole.singleton.Log($"Added Trial: '{trial.GetName()}'");
        }

        [Command(
            description = "Win the current battle.",
            useClassName = true
        )]
        public static void Win()
        {
            CheatCommon.AssertInBattle();

            var gameScreen = SingletonAccessor.ScreenManager.GetScreen(ScreenName.Game) as GameScreen;
            var heroManager = SingletonAccessor.CombatManager.GetHeroManager();

            gameScreen.StartCoroutine(heroManager.Cheat_EndBattle());
        }
    }
}
