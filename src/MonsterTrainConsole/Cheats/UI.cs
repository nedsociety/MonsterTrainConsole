using HarmonyLib;
using SickDev.CommandSystem;
using System;
using UnityEngine;

namespace MonsterTrainConsole
{
    public static class UI
    {
        static void EnableObjects(Func<GameObject, bool> filter, bool enable)
        {
            var gameObjects = Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[];
            foreach (GameObject gameObject in gameObjects)
            {
                if (
                    gameObject.hideFlags != HideFlags.NotEditable
                    && gameObject.hideFlags != HideFlags.HideAndDontSave
                    && filter(gameObject)
                )
                    gameObject.SetActive(enable);
            }
        }

        [Command(
            description = "Show/hide HUD. @onOff: either 'on' or 'off'.",
            useClassName = true
        )]
        public static void Hud(string onOff)
        {
            bool enable = onOff.ParseOnOff();
            EnableObjects(gameObject => gameObject.name == "Hud" || gameObject.name == "Battle Hud", enable);
            DevConsole.singleton.Log(enable ? "HUD is enabled." : "HUD is disabled.");
        }

        [Command(
            description = "Show/hide unit stats. @onOff: either 'on' or 'off'.",
            useClassName = true
        )]
        public static void UnitStats(string onOff)
        {
            bool enable = onOff.ParseOnOff();
            EnableObjects(gameObject => gameObject.name == "DetailsUIRoot", enable);
            DevConsole.singleton.Log(enable ? "Unit stats display is enabled." : "Unit stats display is disabled.");
        }

        [Command(
            description = "Show/hide chatters. @onOff: either 'on' or 'off'.",
            useClassName = true
        )]
        public static void Chatters(string onOff)
        {
            bool enable = onOff.ParseOnOff();
            Chatter.ChatterEnabled = enable;
            DevConsole.singleton.Log(enable ? "Chatter is enabled." : "Chatter is disabled.");
        }

        [Command(
            description = "Start a random battle music.",
            useClassName = true
        )]
        public static void ShuffleMusic()
        {
            var gameScreen = SingletonAccessor.ScreenManager.GetScreen(ScreenName.Game) as GameScreen;
            var soundManager = Traverse.Create(gameScreen).Field("soundManager").GetValue<SoundManager>();

            soundManager.StopMusic(0f);
            soundManager.PlayBattleMusic();

            DevConsole.singleton.Log("Music restarted.");
        }
    }
}
