using SickDev.CommandSystem;

namespace MonsterTrainConsole
{
    public static class Console
    {
        [Command(
            description = "Show help for a command. @command: the name of the command.",
            alias = "help"
        )]
        public static void Help(string command)
        {
            DevConsoleManager.ShowHelp(command);
        }

        [Command(
            description = "Clear the console.",
            alias = "clear"
        )]
        public static void Clear()
        {
            DevConsole.singleton.ClearLog();
        }

        [Command(
            description = "Turn on/off the console busy guard. @onOff: either 'on' or 'off'.",
            useClassName = true
        )]
        public static void BusyGuard(string onOff)
        {
            if (onOff.ParseOnOff())
            {
                CheatCommon.BusyGuard = true;
                DevConsole.singleton.Log("Busy state is checked.");
            }
            else
            {
                CheatCommon.BusyGuard = false;
                DevConsole.singleton.Log("No more check for busy state. Use at your own risk.");
            }
        }
    }
}
