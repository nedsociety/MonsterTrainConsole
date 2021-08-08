using HarmonyLib;
using SickDev.DevConsole;
using System.Linq;
using System.Collections.Generic;
using System;

namespace MonsterTrainConsole
{
    public static class DevConsoleManager
    {
        private static HashSet<string> assembliesWithCommands = new HashSet<string>();
        internal static bool SettingsModified { get; set; } = false;
        internal static bool ExpandByDefault { get; set; } = false;
        internal static bool SuppressInitializationMessage { get; set; } = false;

        internal static void Initialize()
        {
            // REF: https://docs.google.com/document/d/1LTNRVwY8R4AqgcdEsF9cxtmlcuyoWzVvwo6GjQm8Z3Q/edit#heading=h.ymv4o7v9jgvd
            // Ensure DevConsole and its member CommandsManager is created
            DevConsole.singleton.commandsManager.ToString();
        }

        public static void AddCommandsFromAssembly(string assemblyName)
        {
            if (assembliesWithCommands.Add(assemblyName))
            {
                // REF: SickDev.DevConsole.DevConsole.CreateCommandsManager()
                var devConsole = DevConsole.singleton;

                devConsole.Log("Reloading DevConsole command list...");

                SickDev.CommandSystem.CommandsManager commandsManager = new SickDev.CommandSystem.CommandsManager(
                    new SickDev.CommandSystem.Configuration(
                        // ORIG: Application.platform != RuntimePlatform.WebGLPlayer,
                        // We're not modding the game in WebGL
                        true,
                        assembliesWithCommands.ToArray()
                    )
                );
                commandsManager.LoadCommands();

                // ORIG: new SickDev.DevConsole.BuiltInCommandsBuilder(commandsManager).Build();
                // We're not using any built-in commands, and by default the game also turns them off

                // Replace CommandsSystem to our own
                Traverse.Create(devConsole).Field("_commandsManager").SetValue(commandsManager);
            }
        }

        public static void ShowHelp(string command)
        {
            var commandExecuter = DevConsole.singleton.commandsManager.GetCommandExecuter(command);
            var overloads = commandExecuter.GetOverloads();
            if (overloads.IsNullOrEmpty())
            {
                DevConsole.singleton.LogError($"Unknown command ${command}.");
                return;
            }
            DevConsole.singleton.Log(overloads[0].description);
        }
    }

    [HarmonyPatch(typeof(DevConsole))]
    [HarmonyPatch("settings", MethodType.Getter)]
    class DevConsole_settings_Getter
    {
        public static void Postfix(ref Settings __result)
        {
            if (!DevConsoleManager.SettingsModified)
            {
                var settings = Traverse.Create(typeof(DevConsole)).Field("_settings").GetValue<Settings>();

                settings.autoCompleteBehaviour = Settings.AutoCompleteBehaviour.Auto;
                settings.autoCompleteWithEnter = true;
                settings.preferredHeight = 0.5f;
                settings.fontSize = 14;

                Traverse.Create(typeof(DevConsole)).Field("_settings").SetValue(settings);

                DevConsoleManager.SettingsModified = true;
                __result = settings;
            }
        }
    }

    [HarmonyPatch(typeof(EntryData))]
    [HarmonyPatch(nameof(EntryData.CutOffExceedingText))]
    class EntryData_CutOffExceedingText
    {
        public static bool Prefix()
        {
            // Disable 16300 text length limit
            return false;
        }
    }

    [HarmonyPatch(typeof(Entry))]
    [HarmonyPatch(MethodType.Constructor, new Type[] { typeof(EntryData) })]
    class Entry_ctor
    {
        public static void Postfix(Entry __instance)
        {
            __instance.isExpanded = DevConsoleManager.ExpandByDefault;
        }
    }

    [HarmonyPatch(typeof(Entry))]
    [HarmonyPatch("DrawFoldoutToggle")]
    class Entry_DrawFoldoutToggle
    {
        // REF: Entry.DrawFoldoutToggle()
        // This function overwrites isExpanded to false if needsExpandToggle is turned off.
        // We'll undo the effect here.

        static Dictionary<Entry, bool> isExpandedMemory = new Dictionary<Entry, bool>();

        public static void Prefix(Entry __instance)
        {
            var needsExpandToggle = Traverse.Create(__instance).Field("builder").Field("needsExpandToggle").GetValue<bool>();

            if (!needsExpandToggle)
                isExpandedMemory[__instance] = __instance.isExpanded;
        }

        public static void Postfix(Entry __instance)
        {
            if (isExpandedMemory.TryGetValue(__instance, out var isExpanded))
            {
                __instance.isExpanded = isExpanded;
                isExpandedMemory.Remove(__instance);
            }
        }
    }

    [HarmonyPatch(typeof(SickDev.CommandSystem.CommandsManager))]
    [HarmonyPatch("SendMessage")]
    class CommandsManager_SendMessage
    {
        public static bool Prefix(string message)
        {
            if (DevConsoleManager.SuppressInitializationMessage)
            {
                MonsterTrainConsole.Logger.LogInfo("Redirected from DevConsole initialization:");
                MonsterTrainConsole.Logger.LogInfo(message);
                return false;
            }
            return true;
        }
    }
}
