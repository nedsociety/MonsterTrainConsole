using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace MonsterTrainConsole
{
    static class CheatWindowManager
    {
		public sealed class WindowDefinition
		{
			public bool enabled;
			public Rect rect;
			public string title;
			public string command;
			public GUI.WindowFunction function;
			public int id = 0;
		}

		private static List<WindowDefinition> cachedWindows = new List<WindowDefinition>();

		public static WindowDefinition GetWindow(string command)
		{
			foreach (var cachedWindow in cachedWindows)
			{
				if (cachedWindow.command == command)
					return cachedWindow;
			}
			return null;
		}

		public static void AddWindow(WindowDefinition windowDefinition)
        {
			if (windowDefinition.id == 0)
				windowDefinition.id = GUIUtility.GetControlID(FocusType.Passive);

			cachedWindows.Add(windowDefinition);
		}

		public static void CloseAllWindows()
		{
			foreach (WindowDefinition cachedWindow in cachedWindows)
				cachedWindow.enabled = false;
		}

        public static void OnGUI()
        {
			foreach (WindowDefinition cachedWindow in cachedWindows)
            {
				if (!cachedWindow.enabled)
					continue;

				GUILayout.Window(cachedWindow.id, cachedWindow.rect, cachedWindow.function, cachedWindow.title);
			}
		}
	}

	[HarmonyPatch(typeof(CheatManager))]
	[HarmonyPatch("OnGUI")]
	class CheatManager_OnGUI
	{
		static void Postfix()
		{
			CheatWindowManager.OnGUI();
		}
	}

	[HarmonyPatch(typeof(CheatManager))]
	[HarmonyPatch("CloseAllWindows")]
	class CheatManager_CloseAllWindows
	{
		static void Postfix()
		{
			CheatWindowManager.CloseAllWindows();
		}
	}
}
