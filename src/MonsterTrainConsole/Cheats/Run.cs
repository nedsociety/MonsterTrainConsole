using HarmonyLib;
using ShinyShoe;
using ShinyShoe.Loading;
using SickDev.CommandSystem;
using System;
using System.Collections.Generic;

namespace MonsterTrainConsole
{
    public static class Run
    {
        [Command(
            description = "Force any single player runs created from now on to use a specific RNG seed. @seed: the seed value to force, or 0 to deactivate.",
            useClassName = true
        )]
        public static void SetSeed(int seed = 0)
        {
            SingletonAccessor.SaveManager.Cheat_ForceSeed(seed);
            DevConsole.singleton.Log(
                (seed == 0)
                ? "Forcing seed is disabled; New single player games will be seeded randomly from now on."
                : $"Forcing seed is enabled; New single player games will use seed {seed} until disabled or the game process exits."
            );
        }

        [Command(
            description = "Retrieve RNG seed from the current single player run.",
            useClassName = true
        )]
        public static void GetSeed()
        {
            CheatCommon.AssertSingleplayerSaveExists();
            var saveData = Traverse.Create(SingletonAccessor.SaveManager)
                .Field("singlePlayerSave").Field("saveData").GetValue<SaveData>();
            DevConsole.singleton.Log($"Seed for the current single player run: {saveData.GetSeed()}");
        }

        [Command(
            description = "Adjust gold by given amount. @amount: Amount of gold to add from the current value. Can be negative.",
            useClassName = true
        )]
        public static void AdjustGold(int amount)
        {
            CheatCommon.AssertInGame();

            var saveManager = SingletonAccessor.SaveManager;
            int gold = saveManager.GetGold();
            saveManager.AdjustGold(amount);
            DevConsole.singleton.Log($"Gold adjusted from <b>{gold}</b> to <b>{saveManager.GetGold()}</b>.");
        }

        [Command(
            description = "Set the number of Pact Shards. @pactShards: number of Pact Shards to set.",
            useClassName = true
        )]
        public static void SetPactShards(int pactShards)
        {
            CheatCommon.AssertInGame();
            CheatCommon.AssertDLCAvailableOn(CheatCommon.DLCAvailability.CurrentActiveSaveData);

            var saveManager = SingletonAccessor.SaveManager;
            HellforgedSaveData dlcSaveData = saveManager.GetDlcSaveData<HellforgedSaveData>(DLC.Hellforged);
            if (dlcSaveData == null)
                throw new InvalidOperationException("no save file with The Last Divinity enabled found");

            // Note: there's no range check on both the original cheat and SetCrystals().
            // I'm not sure of the implication of the negative pact shards, but certainly it can do things or might just break down.
            dlcSaveData.SetCrystals(pactShards);
            saveManager.pactCrystalsChangedSignal?.Dispatch(pactShards);
            DevConsole.singleton.Log($"Pact Shards set to {pactShards}.");
        }

        static TabularDataAccessor<MutatorData> mutatorDataAccessor = new TabularDataAccessor<MutatorData>(
            "Mutators",
            () => Traverse.Create(SingletonAccessor.AllGameData).Field("mutatorDatas").GetValue<List<MutatorData>>(),
            CheatCommon.GenericDLCFilter,
            ("ID", element => element.GetID()),
            ("Name", element => element.name),
            ("Localized", element => element.GetName())
        );

        [Command(
            description = "List available Mutators. @search: (optional) text to search.",
            useClassName = true
        )]
        public static void ListMutators(string search = "")
        {
            mutatorDataAccessor.PrintListToDevConsole(search);
        }

        [Command(
            description = "Add a Mutator to the current run. @mutatorName: the name/ID of the Mutator.",
            useClassName = true
        )]
        public static void AddMutator(string mutatorName)
        {
            CheatCommon.AssertInGame();

            MutatorData mutator = mutatorDataAccessor.SelectOne(mutatorName, nameof(mutatorName));
            SingletonAccessor.SaveManager.AddMutator(mutator);
            DevConsole.singleton.Log($"Added Mutator: '{mutator.GetName()}'");
        }

        static TabularDataAccessor<CollectableRelicData> artifactDataAccessor = new TabularDataAccessor<CollectableRelicData>(
            "Artifacts",
            () => Traverse.Create(SingletonAccessor.AllGameData)
                  .Field("collectableRelicDatas").GetValue<List<CollectableRelicData>>(),
            CheatCommon.GenericDLCFilter,
            ("ID", element => element.GetID()),
            ("Name", element => element.name),
            ("Localized", element => element.GetName()),
            ("Class", element => element.GetLinkedClass()?.name ?? "Clanless")
        );

        [Command(
            description = "List available Artifacts. @search: (optional) text to search.",
            useClassName = true
        )]
        public static void ListArtifacts(string search = "")
        {
            artifactDataAccessor.PrintListToDevConsole(search);
        }

        [Command(
            description = "Add an Artifact to the current run. @artifactName: the name/ID of the Artifact.",
            useClassName = true
        )]
        public static void AddArtifact(string artifactName)
        {
            CheatCommon.AssertInGame();

            CollectableRelicData artifact = artifactDataAccessor.SelectOne(artifactName, nameof(artifactName));
            SingletonAccessor.SaveManager.AddRelic(artifact);
            DevConsole.singleton.Log($"Added Artifact: '{artifact.GetName()}'");
        }

        static TabularDataAccessor<CardData> cardDataAccessor = new TabularDataAccessor<CardData>(
            "cards",
            () => SingletonAccessor.AllGameData.GetAllCardData(),
            CheatCommon.GenericDLCFilter,
            ("ID", element => element.GetID()),
            ("Name", element => element.name),
            ("Localized", element => element.GetName())
        );

        [Command(
            description = "List available cards. @search: (optional) text to search.",
            useClassName = true
        )]
        public static void ListCards(string search = "")
        {
            cardDataAccessor.PrintListToDevConsole(search);
        }

        [Command(
            description = "Add a raw, unmodified card to the deck. @cardName: The name/ID of the card. @drawImmediately: (Optional) Either 'yes' or 'no'. If this cheat is used in battle and this argument is 'yes', immediately draw the added card to hand.",
            useClassName = true
        )]
        public static void AddCard(string cardName, string drawImmediately = "no")
        {
            CheatCommon.AssertInGame();

            CardData cardData = cardDataAccessor.SelectOne(cardName, nameof(cardName));

            bool drawImmediatelyValue = drawImmediately.ParseYesNo(nameof(drawImmediately));

            CheatCommon.BusyStart();
            LoadingScreen.AddTask(
                new LoadAdditionalCards(
                    cardData, loadSpawnedCharacters: true, LoadingScreen.DisplayStyle.Spinner,
                    delegate
                    {
                        var saveManager = SingletonAccessor.SaveManager;

                        CardState card = saveManager.AddCardToDeck(
                            cardData, null, applyExistingRelicModifiers: true, applyExtraCopiesMutator: false, showAnimation: true
                        );
                        DevConsole.singleton.Log($"Adding {cardData.GetName()}.");

                        if (drawImmediatelyValue)
                        {
                            if (saveManager.IsInBattle())
                            {
                                saveManager.DrawSpecificCard(card);
                                DevConsole.singleton.Log($"Drawing {cardData.GetName()}.");
                            }
                            else
                                DevConsole.singleton.LogWarning($"@drawImmediately is set to 'yes' but you're not in battle.");
                        }

                        CheatCommon.BusyEnd();
                    }
                )
            );
        }

        [Command(
            description = "Win the current run.",
            useClassName = true
        )]
        public static void Win()
        {
            CheatCommon.AssertInGame();

            SingletonAccessor.SaveManager.Cheat_WinRun();
            DevConsole.singleton.Log("Autowinning the run...");
        }
    }
}
