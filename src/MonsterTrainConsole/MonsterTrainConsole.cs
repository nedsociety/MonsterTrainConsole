using HarmonyLib;
using SickDev.CommandSystem;

namespace MonsterTrainConsole
{
    [BepInEx.BepInPlugin("com.nedsociety.monstertrainconsole", "MonsterTrainConsole", "1.0.0.0")]
    [BepInEx.BepInDependency("tools.modding.trainworks")]
    public class MonsterTrainConsole : BepInEx.BaseUnityPlugin, Trainworks.Interfaces.IInitializable
    {
        new public static BepInEx.Logging.ManualLogSource Logger { get; private set; }

        public void Awake()
        {
        }

        public void Initialize() {
            Logger = base.Logger;

            if (!CheatCommon.IsDLCAvailableOn(CheatCommon.DLCAvailability.Installed))
            {
                Logger.LogError("MonsterTrainConsole requires DLC (The Last Divinity) to be enabled. Disabling the mod.");
                return;
            }

            new Harmony("com.nedsociety.monstertrainconsole").PatchAll();

            DevConsoleManager.Initialize();
            DevConsoleManager.SuppressInitializationMessage = true;
            DevConsoleManager.ExpandByDefault = true;
            DevConsoleManager.AddCommandsFromAssembly(System.Reflection.Assembly.GetExecutingAssembly().GetName().Name);

            DevConsole.singleton.Log("Welcome to MonsterTrainConsole! For quickhelp on individual commands, use 'help [command]'.");
        }
    }
}
