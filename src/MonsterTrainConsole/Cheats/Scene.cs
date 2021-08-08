using HarmonyLib;
using ShinyShoe.Loading;
using SickDev.CommandSystem;
using System.Collections.Generic;

namespace MonsterTrainConsole
{
    public static class Scene
    {
        static TabularDataAccessor<ScenarioData> battleDataAccessor = new TabularDataAccessor<ScenarioData>(
            "battles",
            () => Traverse.Create(SingletonAccessor.AllGameData).Field("scenarioDatas").GetValue<List<ScenarioData>>(),
            null,
            ("ID", element => element.GetID()),
            ("Name", element => element.name),
            ("Localized", element => element.GetBattleName())
        );

        [Command(
            description = "List available battles. @search: (optional) text to search.",
            useClassName = true
        )]
        public static void ListBattles(string search = "")
        {
            battleDataAccessor.PrintListToDevConsole(search);
        }

        static bool startBattleWarningShown = false;

        [Command(
            description = "Start a battle. @battleName: the name/ID of the battle.",
            useClassName = true
        )]
        public static void StartBattle(string battleName = "")
        {
            CheatCommon.AssertInGame();

            // Use empty battle if the name is not given.
            battleName = battleName?.Trim();
            if (string.IsNullOrEmpty(battleName))
            {
                DevConsole.singleton.LogWarning("Battle name not given; using sandbox scenario 'Level0EmptyTest'.");
                battleName = "Level0EmptyTest";
            }
            ScenarioData scenario = battleDataAccessor.SelectOne(battleName, nameof(battleName));

            CheatCommon.BusyStart();
        	LoadingScreen.AddTask(new LoadInstantly(LoadingScreen.DisplayStyle.FullScreen, delegate
        	{
                var saveManager = SingletonAccessor.SaveManager;
                var screenManager = SingletonAccessor.ScreenManager;

                if (!startBattleWarningShown)
                {
                    // Warn about persistency of the above call
                    DevConsole.singleton.LogWarning(
                        "This command has a side effect that persists through the game process."
                        + " You must restart the game to start a normal run. Refer to the documentation for more info."
                    );
                    startBattleWarningShown = true;
                }

                saveManager.EnableTestScenario(scenario, battleName == "Level0EmptyTest");

        		LoadingScreen.AddTask(new LoadClassAssets(LoadingScreen.DisplayStyle.Spinner, null));
        		LoadingScreen.AddTask(new LoadScenarioAssets(saveManager.GetCurrentScenarioData(), LoadingScreen.DisplayStyle.Spinner, delegate
        		{
                    // ORIG: if (skipIntroBool)
                    // ORIG: {
                    // ORIG:     saveManager.SetGameSequence(SaveData.GameSequence.InBattle);
                    // ORIG:     screenManager.ShowScreen(ScreenName.Game, delegate
                    // ORIG:     {
                    // ORIG:         CheatCommon.BusyEnd();
                    // ORIG:     });
                    // ORIG: }
                    // Personally I found it to be too buggy for most use cases

                    screenManager.OnScreenActivated(ScreenName.BattleIntro, delegate { CheatCommon.BusyEnd(); });
        			screenManager.ShowScreen(ScreenName.BattleIntro);
        		}));
        	}));
        }

        static TabularDataAccessor<StoryEventData> eventDataAccessor = new TabularDataAccessor<StoryEventData>(
            "events",
            () => Traverse.Create(SingletonAccessor.AllGameData).Field("storyEventDatas").GetValue<List<StoryEventData>>(),
            EventDataDLCFilter,
            ("ID", element => element.GetID()),
            ("Name", element => element.name)
        );

        static bool EventDataDLCFilter(StoryEventData eventData)
        {
            return CheatCommon.IsDLCAvailableOn(
                SingletonAccessor.ScreenManager.IsGameplayScreenActive()
                ? CheatCommon.DLCAvailability.CurrentActiveSaveData
                : CheatCommon.DLCAvailability.EnabledOnMainMenu,
                eventData.DLC
            );
        }

        [Command(
            description = "List available Concealed Cavern events. @search: (optional) text to search.",
            useClassName = true
        )]
        public static void ListEvents(string search = "")
        {
            eventDataAccessor.PrintListToDevConsole(search);
        }

        [Command(
            description = "Start a Concealed Cavern event. @eventName: the name/ID of the event.",
            useClassName = true
        )]
        public static void StartEvent(string eventName)
        {
            CheatCommon.AssertSingleplayerSaveExists();

            var saveManager = SingletonAccessor.SaveManager;
            var screenManager = SingletonAccessor.ScreenManager;

            StoryEventData storyEvent = eventDataAccessor.SelectOne(eventName, nameof(eventName));

            CheatCommon.BusyStart();
            screenManager.ShowScreen(ScreenName.StoryEvent, delegate (IScreen screen)
            {
                (screen as StoryEventScreen).SetupStory(storyEvent);
                saveManager.QueueFollowupEvents(storyEvent);
                CheatCommon.BusyEnd();
            });
        }
    }
}
