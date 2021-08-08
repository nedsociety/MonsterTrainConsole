using HarmonyLib;
using I2.Loc;
using SickDev.CommandSystem;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace MonsterTrainConsole
{
    static class LastTimingInformation
    {
        public static SaveManager.GameSpeed activeGameSpeed;
        public static string timingName = string.Empty;
        public static float timingValue;
        public static string dateString = string.Empty;

        public static void Update(float seconds)
        {
            var saveManager = SingletonAccessor.SaveManager;
            if (seconds == saveManager.GetActiveTiming().LastTimeUsed)
            {
                activeGameSpeed = saveManager.GetActiveGameSpeed();
                timingName = saveManager.GetActiveTiming().LastNameUsed;
                timingValue = saveManager.GetActiveTiming().LastTimeUsed;
                dateString = DateTime.UtcNow.ToString("HH:mm:ss.fff");
            }
        }
    }

    public static class Debug
    {
        [Command(
            description = "Dirty the battle state to force-update previews.",
            useClassName = true
        )]
        public static void DirtyBattleState()
        {
            if (!SingletonAccessor.SaveManager.Cheat_DirtyBattleState())
                throw new InvalidOperationException("No combat manager found to dirty.");

            DevConsole.singleton.Log("Dirtied the state.");
        }

        [Command(
            description = "Turn on/off graphics settings individually. @qualitySetting: one of 'all', 'msaa', 'motionBlur', 'bloom', 'fastBloom', 'lightsCastShadows' or 'iceRefraction'. @onOff: either 'on' or 'off'.",
            useClassName = true
        )]
        public static void SetGraphicsQuality(string qualitySetting, string onOff)
        {
            PreferencesManager preferencesManager = SingletonAccessor.SaveManager.GetPreferencesManager();
            if (preferencesManager == null)
                throw new InvalidOperationException("preferencesManager is null");

            bool enable = onOff.ParseOnOff();

            GraphicsSettingsManager.GraphicsQuality graphicsQuality = preferencesManager.GraphicsQuality;
            if (qualitySetting.Equals("all", StringComparison.InvariantCultureIgnoreCase))
            {
                graphicsQuality = (
                    enable
                    ? GraphicsSettingsManager.GraphicsQuality.CreateHighQuality()
                    : GraphicsSettingsManager.GraphicsQuality.CreateLowQuality()
                );
            }
            else if (qualitySetting.Equals("msaa", StringComparison.InvariantCultureIgnoreCase))
                graphicsQuality.msaa = enable;
            else if (qualitySetting.Equals("motionBlur", StringComparison.InvariantCultureIgnoreCase))
                graphicsQuality.motionBlur = enable;
            else if (qualitySetting.Equals("bloom", StringComparison.InvariantCultureIgnoreCase))
                graphicsQuality.bloom = enable;
            else if (qualitySetting.Equals("fastBloom", StringComparison.InvariantCultureIgnoreCase))
                graphicsQuality.fastBloom = enable;
            else if (qualitySetting.Equals("lightsCastShadows", StringComparison.InvariantCultureIgnoreCase))
                graphicsQuality.LightsCastShadows = enable;
            else if (qualitySetting.Equals("iceRefraction", StringComparison.InvariantCultureIgnoreCase))
                graphicsQuality.IceRefraction = enable;
            else
            {
                throw new ArgumentException(
                    $"@qualitySetting must be one of 'all', 'msaa', 'motionBlur', 'bloom', 'fastBloom', 'lightsCastShadows' or 'iceRefraction'; got '{qualitySetting}'"
                );
            }
            preferencesManager.GraphicsQuality = graphicsQuality;

            List<string> enabledFeatures = new List<string>();
            if (graphicsQuality.msaa)
                enabledFeatures.Add("msaa");
            if (graphicsQuality.motionBlur)
                enabledFeatures.Add("motionBlur");
            if (graphicsQuality.bloom)
                enabledFeatures.Add("bloom");
            if (graphicsQuality.fastBloom)
                enabledFeatures.Add("fastBloom");
            if (graphicsQuality.LightsCastShadows)
                enabledFeatures.Add("lightsCastShadows");
            if (graphicsQuality.IceRefraction)
                enabledFeatures.Add("iceRefraction");

            string enabledFeaturesText = ((enabledFeatures.Count == 0) ? "none" : string.Join(", ", enabledFeatures));
            DevConsole.singleton.Log($"Enabled graphics settings: {enabledFeaturesText}");
        }

        [Command(
            description = "Turn on/off wrapping untranslated text. Example: French>>Single player<<. @onOff: either 'on' or 'off'.",
            useClassName = true
        )]
        public static void WrapUntranslatedText(string onOff)
        {
            if (onOff.ParseOnOff())
            {
                LocalizationManager.WrapUntranslatedText = true;
                DevConsole.singleton.Log("Untranslated text will be wrapped with indicators from now on.");
            }
            else
            {
                LocalizationManager.WrapUntranslatedText = false;
                DevConsole.singleton.Log("Untranslated text will be shown as-is from now on.");
            }
        }

        [Command(
            description = "Turn on/off the timing display window. @onOff: either 'on' or 'off'.",
            useClassName = true
        )]
        public static void TimingDisplay(string onOff)
        {
            string command = "Debug.TimingDisplay";
            bool enable = onOff.ParseOnOff();
            
            var window = CheatWindowManager.GetWindow(command);
            if (window == null)
            {
                GUIStyle guiStyle = new GUIStyle(GUI.skin.label)
                {
                    margin = new RectOffset(),
                    padding = new RectOffset()
                };
                CheatWindowManager.AddWindow(
                    new CheatWindowManager.WindowDefinition
                    {
                        enabled = false,
                        rect = new Rect(0f, 120f, 300f, 90f),
                        title = "Active Game Timing Diagnostics",
                        command = command,
                        function = delegate {
                            try
                            {
                                GUILayout.Label($"Speed: {LastTimingInformation.activeGameSpeed}", guiStyle);
                                GUILayout.Label($"Name: {LastTimingInformation.timingName}", guiStyle);
                                GUILayout.Label($"Delay: {LastTimingInformation.timingValue * 1000f} ms", guiStyle);
                                GUILayout.Label($"Time: {LastTimingInformation.dateString}", guiStyle);
                            }
                            catch
                            {
                            }
                        }
                    }
                );

                window = CheatWindowManager.GetWindow(command);
            }

            if (window.enabled != enable)
            {
                window.enabled = enable;
                if (enable)
                    CoreUtil.WaitForSecondsListener += LastTimingInformation.Update;
                else
                    CoreUtil.WaitForSecondsListener -= LastTimingInformation.Update;
            }
        }

        [Command(
            description = "Modify the global Unity time scale value. Essentially a speedhack. @scale: Scaling value. Values smaller than 1 slow down time. Values larger than 1 speed up time.",
            useClassName = true
        )]
        public static void Timescale(float scale)
        {
            if (scale <= 0)
                throw new ArgumentException("@scale must be positive");

            Time.timeScale = scale;
            DevConsole.singleton.Log($"Unity time scale set to {scale}x.");
        }
    }
}
