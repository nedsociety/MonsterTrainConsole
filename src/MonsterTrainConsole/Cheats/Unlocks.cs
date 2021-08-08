using HarmonyLib;
using SickDev.CommandSystem;
using System;
using System.Collections.Generic;

namespace MonsterTrainConsole
{
    public static class Unlocks
    {
        static CompendiumScreen GetCompendiumScreen()
        {
            var screenManager = SingletonAccessor.ScreenManager;
            if (!screenManager.GetScreenActive(ScreenName.Compendium))
                throw new InvalidOperationException("Must be used while the Compendium screen is opened.");

            return screenManager.GetScreen(ScreenName.Compendium) as CompendiumScreen;
        }

        [Command(
            description = "Increase the maximum Covenant Rank. @increaseBy: the number of ranks to unlock from the current limit.",
            useClassName = true
        )]
        public static void UnlockCovenantRank(int increaseBy = 1)
        {
            if (increaseBy <= 0)
                throw new ArgumentException("@increaseBy must be bigger than 0");

            for (int i = 0; i < increaseBy; ++i)
                SingletonAccessor.SaveManager.GetMetagameSave().IncreaseMaxAscensionLevel();

            DevConsole.singleton.Log($"Covenant Rank unlocked by {increaseBy}.");
        }

        [Command(
            description = "Discover ALL cards, artifacts and Champion upgrades in the Compendium.",
            useClassName = true
        )]
        public static void CompendiumDiscoverAll()
        {
            var compendiumScreen = GetCompendiumScreen();
            var saveManager = SingletonAccessor.SaveManager;

            var pagesBySection = Traverse.Create(compendiumScreen).Field("pagesBySection")
                .GetValue<Dictionary<CompendiumScreen.Section, CompendiumSection>>();

            foreach (CompendiumSection compendiumSection in pagesBySection.Values)
                compendiumSection.DiscoverAllCheat(saveManager.GetMetagameSave());

            saveManager.StartSavingMetagame();
            Traverse.Create(compendiumScreen).Method("TryInitialize", true).GetValue();

            DevConsole.singleton.Log($"Discovered all cards, artifacts and Champion upgrades in the Compendium.");
        }

        [Command(
            description = "Divine master all cards in the Compendium.",
            useClassName = true
        )]
        public static void CompendiumDivineMasterAllCards()
        {
            var compendiumScreen = GetCompendiumScreen();
            var saveManager = SingletonAccessor.SaveManager;

            var pagesBySection = Traverse.Create(compendiumScreen).Field("pagesBySection")
                .GetValue<Dictionary<CompendiumScreen.Section, CompendiumSection>>();

            foreach (var compendiumSection in pagesBySection.Values)
            {
                var compendiumSectionCards = compendiumSection as CompendiumSectionCards;
                if (compendiumSectionCards)
                {
                    compendiumSectionCards.MasterAllCardsCheat(saveManager.GetMetagameSave());
                }
            }

            saveManager.StartSavingMetagame();
            Traverse.Create(compendiumScreen).Method("TryInitialize", true).GetValue();

            DevConsole.singleton.Log($"Discovered all cards, artifacts and Champion upgrades in the Compendium.");
        }

        [Command(
            description = "Mark all Divine victories on Covenant Rank 25 in the Compendium.",
            useClassName = true
        )]
        public static void CompendiumMarkAllDivineVictories()
        {
            SingletonAccessor.SaveManager.GetMetagameSave().Cheat_WinWithAllClassCombos(
                SingletonAccessor.AllGameData.GetAllClassDatas()
            );

            DevConsole.singleton.Log($"All class combos have been marked for victory in the Compendium.");
        }

        [Command(
            description = "Unlock all available card frames in the Compendium.",
            useClassName = true
        )]
        public static void CompendiumUnlockAllFrames()
        {
            var metagameSave = SingletonAccessor.SaveManager.GetMetagameSave();
            foreach (MasteryFrameType value in Enum.GetValues(typeof(MasteryFrameType)))
                metagameSave.UnlockMasteryFrameType(value);

            DevConsole.singleton.Log($"All card frames unlocked.");
        }

        static TabularDataAccessor<ClassData> clanDataAccessor = new TabularDataAccessor<ClassData>(
            "classes",
            () => SingletonAccessor.AllGameData.GetAllClassDatas(),
            null,
            ("ID", element => element.GetID()),
            ("Name", element => element.name),
            ("Localized", element => element.GetTitle())
        );

        [Command(
            description = "List available clans. @search: (optional) text to search.",
            useClassName = true
        )]
        public static void ListClans(string search = "")
        {
            clanDataAccessor.PrintListToDevConsole(search);
        }

        [Command(
            description = "Set level of a clan. @clan: the clan name/ID. @level: Level to set.",
            useClassName = true
        )]
        public static void ClanSetLevel(string clan, int level)
        {
            if (level <= 0 || level > SingletonAccessor.BalanceData.GetMaximumClassLevel())
                throw new ArgumentException($"@level must be from 1 to {SingletonAccessor.BalanceData.GetMaximumClassLevel()}.");

            ClassData classData = clanDataAccessor.SelectOne(clan, nameof(clan));

            var saveManager = SingletonAccessor.SaveManager;
            saveManager.GetMetagameSave().SetLevelAndXP(classData.GetID(), level, 0);
            // Signal the change
            saveManager.AddXPToClass(classData.GetID(), 0, true);
            DevConsole.singleton.Log($"The level of {classData.GetTitle()} has been set to {level}.");
        }

        [Command(
            description = "Add XP to a clan. @clan: the clan name/ID. @amount: XP to add.",
            useClassName = true
        )]
        public static void ClanAddXP(string clan, int amount)
        {
            if (amount <= 0)
                throw new ArgumentException("@amount must be positive");

            ClassData classData = clanDataAccessor.SelectOne(clan, nameof(clan));
            SingletonAccessor.SaveManager.AddXPToClass(classData.GetID(), amount, true);

            DevConsole.singleton.Log($"Added {amount} XP to {classData.GetTitle()}.");
        }

        [Command(
            description = "Unlock all clans by skipping the tutorial mission, unlocking Covenant Rank 25, and fulfilling all clan requirements. All clans are unlocked to their maximum level.",
            useClassName = true
        )]
        public static void ClanUnlockAll()
        {
            var saveManager = SingletonAccessor.SaveManager;
            saveManager.GetMetagameSave().Cheat_UnlockAllClasses(saveManager);

            foreach (ClassData classData in SingletonAccessor.AllGameData.GetAllClassDatas())
                saveManager.unlockClassSignal?.Dispatch(classData);

            DevConsole.singleton.Log($"Unlocked all clans to their fullest.");
        }
    }
}
