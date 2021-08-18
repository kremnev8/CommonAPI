﻿using HarmonyLib;

namespace CommonAPI
{
    [HarmonyPatch]
    public static class LoadSaveOnLoad
    {
        public static string saveName;
        public static bool isValid => saveName != null && !saveName.Equals("");
        
        private static string GetArg(string name)
        {
            var args = System.Environment.GetCommandLineArgs();
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == name && args.Length > i + 1)
                {
                    return args[i + 1];
                }
            }
            return null;
        }

        public static void Init()
        {
            saveName = GetArg("loadSave");
            if (isValid)
            {
                DSPGame.LoadFile = saveName;
                CommonAPIPlugin.logger.LogInfo($"Loading save {saveName} by default!");
            }
        }

        [HarmonyPatch(typeof(VFPreload), "InvokeOnLoadWorkEnded")]
        [HarmonyPostfix]
        public static void LoadSave()
        {
            if (!isValid || !GameSave.SaveExist(saveName)) return;
            
            DSPGame.StartGame(saveName);
        }
    }
}