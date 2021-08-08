using HarmonyLib;
using ShinyShoe;
using SickDev.CommandSystem;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MonsterTrainConsole
{
    class TabularDataAccessor<T>
    {
        struct ColumnAccessor
        {
            public string title;
            public Func<T, string> func;
        };

        private string title;
        private Func<IEnumerable<T>> dataAccessor;
        private Func<T, bool> filter;
        private ColumnAccessor[] columnAccessors;

        public TabularDataAccessor(
            string title,
            Func<IEnumerable<T>> dataAccessor,
            Func<T, bool> filter,
            params (string title, Func<T, string> func)[] columnAccessors
        )
        {
            this.title = title;
            this.dataAccessor = dataAccessor;
            this.filter = filter;
            this.columnAccessors = (
                from column in columnAccessors
                select new ColumnAccessor { title = column.title, func = column.func }
            ).ToArray();
        }

        public IEnumerable<T> Select(string hint)
        {
            bool ColumnsContainHint(T row)
            {
                if (String.IsNullOrEmpty(hint))
                    return true;

                foreach (var columnAccessor in columnAccessors)
                {
                    if (columnAccessor.func(row).ToLowerInvariant().Contains(hint.ToLowerInvariant()))
                        return true;
                }

                return false;
            }

            IEnumerable<T> data = dataAccessor();

            return from row in data
                   where (row != null) && ((filter == null) || filter(row)) && ColumnsContainHint(row)
                   select row;
        }

        string GetTabularRepr(IEnumerable<T> searchResult)
        {
            return String.Join(
                "\n",
                from row in searchResult
                select String.Join(
                       " / ",
                       from columnAccessor in columnAccessors
                       select String.IsNullOrEmpty(columnAccessor.title)
                              ? columnAccessor.func(row)
                              : $"{columnAccessor.title}: {columnAccessor.func(row)}"
                )
            );
        }

        public void PrintListToDevConsole(string hint)
        {
            hint = hint?.Trim();
            var searchResult = Select(hint).ToArray();

            if (searchResult.Length == 0)
            {
                if (String.IsNullOrEmpty(hint))
                    DevConsole.singleton.Log($"Cannot find any of {title} -- possibly a bug.");
                else
                    DevConsole.singleton.Log($"Searching {hint} in {title}: no matches found.");
            }
            else
            {
                if (String.IsNullOrEmpty(hint))
                    DevConsole.singleton.Log($"List of all {title}: {searchResult.Length} results.\n" + GetTabularRepr(searchResult));
                else
                    DevConsole.singleton.Log($"Searching {hint} in {title}: {searchResult.Length} results.\n" + GetTabularRepr(searchResult));
            }
        }

        public T SelectOne(string hint, string hintArgName)
        {
            hint = hint?.Trim();
            var searchResult = Select(hint).ToArray();

            if (searchResult.Length == 1)
                return searchResult[0];
            else if (searchResult.Length == 0)
                throw new ArgumentException($"none of {title} with '{hint}' could be found");
            else
            {
                if (String.IsNullOrEmpty(hint))
                    throw new ArgumentException($"@{hintArgName} is missing");
                else if (searchResult.Length < 5)
                {
                    throw new ArgumentException(
                        $"Multiple entries with '{hint}' have been found."
                        + $" Try to narrow it down by providing more accurate @{hintArgName}.\n"
                        + "Possible candidates:\n" + GetTabularRepr(searchResult)
                    );
                }
                else
                {
                    throw new ArgumentException(
                        $"Multiple entries (#{searchResult.Length}) with '{hint}' have been found."
                        + $" Try to narrow it down by providing more accurate @{hintArgName}."
                    );
                }
            }
        }
    }
    static class CheatCommon
    {
        public static bool BusyGuard { get; set; } = true;
        static bool isBusy = false;

        public static void BusyStart()
        {
            if (isBusy && BusyGuard)
                throw new InvalidOperationException("Console is busy executing a command. Try again later.");
            isBusy = true;
        }

        public static void BusyEnd()
        {
            if (!isBusy && BusyGuard)
                throw new InvalidOperationException("Console is in an inconsistent state.");
            isBusy = false;
        }

        public static bool ParseOnOff(this string onOffString, string onOffArgName = "onOff")
        {
            if (onOffString.Equals("on", StringComparison.InvariantCultureIgnoreCase))
                return true;
            else if (onOffString.Equals("off", StringComparison.InvariantCultureIgnoreCase))
                return false;
            else
                throw new ArgumentException($"@{onOffArgName} must be either 'on' or 'off'; got '{onOffString}'");
        }

        public static bool ParseYesNo(this string yesNoString, string yesNoArgName = "yesNo")
        {
            if (yesNoString.Equals("yes", StringComparison.InvariantCultureIgnoreCase))
                return true;
            else if (yesNoString.Equals("no", StringComparison.InvariantCultureIgnoreCase))
                return false;
            else
                throw new ArgumentException($"@{yesNoArgName} must be either 'on' or 'off'; got '{yesNoString}'");
        }

        public static void ReportCheatTestResult(CheatManager.CheatTestResult result, string successMessage)
        {
            successMessage = successMessage ?? "Operation successful.";
            switch (result)
            {
                case CheatManager.CheatTestResult.Pass:
                    DevConsole.singleton.Log(successMessage);
                    break;
                case CheatManager.CheatTestResult.UnitNotFound:
                    DevConsole.singleton.LogError("Command failed (unit not found).");
                    break;
                case CheatManager.CheatTestResult.StatusEffectNotFound:
                    DevConsole.singleton.LogError("Command failed (status effect not found).");
                    break;
                case CheatManager.CheatTestResult.Fail:
                    DevConsole.singleton.LogError("Command failed (unknown).");
                    break;
            }
        }

        public static int SanitizeFloorArgument(int floor, string floorArgName = "floor")
        {
            AssertInBattle();

            // floor = -1 means the current floor
            if (floor < -1 || floor > 3)
            {
                throw new ArgumentException(
                    $"@{floorArgName} must be from 0 to 3, with 0 being the bottom floor and 3 being the Pyre room. Got {floor}."
                );
            }
            else if (floor == -1)
                return SingletonAccessor.RoomManager.GetSelectedRoom();

            return floor;
        }

        public enum DLCAvailability
        {
            Installed,
            EnabledOnMainMenu,
            CurrentActiveSaveData
        }

        public static bool GenericDLCFilter<T>(T elem)
        {
            DLC dlc = DLC.None;

            var dlcCheckerMethod = Traverse.Create(elem).Method("GetRequiredDLC");
            if (dlcCheckerMethod.MethodExists())
                dlc = dlcCheckerMethod.GetValue<DLC>();

            return IsDLCAvailableOn(
                SingletonAccessor.ScreenManager.IsGameplayScreenActive()
                ? DLCAvailability.CurrentActiveSaveData
                : DLCAvailability.EnabledOnMainMenu,
                dlc
            );
        }

        public static bool IsDLCAvailableOn(DLCAvailability availability, DLC dlc = DLC.Hellforged)
        {
            // DLC affects followings:
            // 1. Pact Shard and TLD
            // 2. Card
            // 3. Class
            // 4. CollectableRelic
            // 5. Mutator
            // 6. SpChallenge
            // 7. MapNodeData

            if (dlc == DLC.None)
                return true;

            switch (availability)
            {
                case DLCAvailability.Installed:
                    return SingletonAccessor.SaveManager.IsDlcInstalled(dlc);
                case DLCAvailability.EnabledOnMainMenu:
                    return SingletonAccessor.SaveManager.GetInstalledAndMetagameEnabledDLCs().Contains(dlc);
                case DLCAvailability.CurrentActiveSaveData:
                    return SingletonAccessor.SaveManager.IsDlcEnabledForCurrentRun(dlc);
                default:
                    throw new ArgumentException("invalid @availability");
            }
        }

        public static void AssertDLCAvailableOn(DLCAvailability availability, DLC dlc = DLC.Hellforged)
        {
            if (IsDLCAvailableOn(availability, dlc))
                return;

            switch (availability)
            {
                case DLCAvailability.Installed:
                    throw new InvalidOperationException("Can only be used when DLC is installed.");
                case DLCAvailability.EnabledOnMainMenu:
                    throw new InvalidOperationException("Can only be used when DLC is turned on from menu.");
                case DLCAvailability.CurrentActiveSaveData:
                    throw new InvalidOperationException("Can only be used when DLC is enabled in the current game.");
                default:
                    throw new ArgumentException("invalid @availability");
            }
        }

        public static void AssertInBattle()
        {
            if (!(SingletonAccessor.SaveManager.IsInBattle() && SingletonAccessor.ScreenManager.IsGameplayScreenActive()))
                throw new InvalidOperationException("Must be used in battle.");
        }

        public static void AssertInGame()
        {
            if (!SingletonAccessor.ScreenManager.IsGameplayScreenActive())
                throw new InvalidOperationException("Must be used in the run.");
        }

        public static void AssertSingleplayerSaveExists()
        {
            if (!SingletonAccessor.SaveManager.HasRun(RunType.Class))
                throw new InvalidOperationException("Requires a single player save to be present.");
        }
    }

    public static class SingletonAccessor
    {
        public static SaveManager SaveManager
        {
            get { return Trainworks.Managers.ProviderManager.SaveManager; }
        }

        public static CombatManager CombatManager
        {
            get { return Trainworks.Managers.ProviderManager.CombatManager; }
        }

        public static RoomManager RoomManager
        {
            get { return Traverse.Create(CombatManager).Field("roomManager").GetValue<RoomManager>(); }
        }

        public static AllGameData AllGameData
        {
            get { return SaveManager.GetAllGameData(); }
        }

        public static BalanceData BalanceData
        {
            get { return SaveManager.GetBalanceData(); }
        }

        public static StatusEffectManager StatusEffectManager
        {
            get { return StatusEffectManager.Instance; }
        }

        public static ScreenManager ScreenManager
        {
            get { return Traverse.Create(SaveManager).Field("screenManager").GetValue<ScreenManager>(); }
        }

        public static CardManager CardManager
        {
            get { return Traverse.Create(SaveManager).Field("cardManager").GetValue<CardManager>(); }
        }

        public static RunHistoryManager RunHistoryManager
        {
            get { return Traverse.Create(SaveManager).Field("runHistoryManager").GetValue<RunHistoryManager>(); }
        }
    }
}
