using HarmonyLib;
using SickDev.CommandSystem;
using System;

namespace MonsterTrainConsole
{
    public static class Map
    {
        static MapScreen GetMapScreen()
        {
            var screenManager = SingletonAccessor.ScreenManager;
            if (!screenManager.GetScreenActive(ScreenName.Map))
                throw new InvalidOperationException("Must be used while the map screen is opened.");

            return screenManager.GetScreen(ScreenName.Map) as MapScreen;
        }

        [Command(
            description = "Proceed to the next node.",
            useClassName = true
        )]
        public static void Next()
        {
            var mapScreen = GetMapScreen();
            Traverse.Create(mapScreen).Method("Cheat_AdjustDistance", 1).GetValue();
            DevConsole.singleton.Log("Advancing to the next node.");
        }

        [Command(
            description = "Backtrack to the previous node.",
            useClassName = true
        )]
        public static void Previous()
        {
            var mapScreen = GetMapScreen();
            Traverse.Create(mapScreen).Method("Cheat_AdjustDistance", -1).GetValue();
            DevConsole.singleton.Log("Backtracking to the previous node.");
        }

        [Command(
            description = "Go to the final node.",
            useClassName = true
        )]
        public static void Final()
        {
            var mapScreen = GetMapScreen();
            int nodeidx = SingletonAccessor.SaveManager.GetRunLength() - 1;
            Traverse.Create(mapScreen).Method("Cheat_SetDistance", nodeidx).GetValue();
            DevConsole.singleton.Log("Jumping to the final node.");
        }

        [Command(
            description = "Reset rewards in the current node.",
            useClassName = true
        )]
        public static void Reset()
        {
            var mapScreen = GetMapScreen();
            SingletonAccessor.SaveManager.ResetNode();
            Traverse.Create(mapScreen).Method("RefreshActiveSection").GetValue();
            DevConsole.singleton.Log("Resetting rewards.");
        }

        [Command(
            description = "Change the branch decision in the the current node.",
            useClassName = true
        )]
        public static void ChangeSide()
        {
            var mapScreen = GetMapScreen();
            var saveManager = SingletonAccessor.SaveManager;
            saveManager.SetCurrentBranch((saveManager.GetCurrentBranch() == 0) ? 1 : 0);
            Traverse.Create(mapScreen).Method("SetUpAtCurrentDistance").GetValue();
            DevConsole.singleton.Log("Changing side.");
        }
    }
}

