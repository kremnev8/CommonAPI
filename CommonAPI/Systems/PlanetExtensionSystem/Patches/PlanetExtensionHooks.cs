﻿using System;
using CommonAPI.Systems;
using HarmonyLib;
// ReSharper disable InconsistentNaming
// ReSharper disable UnusedParameter.Global
// ReSharper disable RedundantAssignment

namespace CommonAPI.Patches
{


    [HarmonyPatch]
    public static class PlanetExtensionHooks
    {
        
        [HarmonyPostfix]
        [HarmonyPatch(typeof(FactoryModel), "OnCameraPostRender")]
        public static void OnRender(FactoryModel __instance)
        {
            if (__instance.planet == null || !__instance.planet.factoryLoaded || __instance.planet != GameMain.localPlanet) return;
            
            PlanetExtensionSystem.DrawUpdate(__instance.planet.factory);
        }

        //Single thread update calls (Only when game is running in single thread mode)
        [HarmonyPatch(typeof(FactorySystem), "GameTickBeforePower")]
        [HarmonyPostfix]
        public static void PowerTick(FactorySystem __instance, long time, bool isActive)
        {
            PlanetExtensionSystem.PowerUpdate(__instance.factory);
        }
        
        [HarmonyPatch(typeof(FactorySystem), "GameTick", new Type[] {typeof(long), typeof(bool)})]
        [HarmonyPrefix]
        public static void PreUpdate(FactorySystem __instance, long time, bool isActive)
        {
            PlanetExtensionSystem.PreUpdate(__instance.factory);
        }

        [HarmonyPatch(typeof(FactorySystem), "GameTickLabOutputToNext", new Type[] {typeof(long), typeof(bool)})]
        [HarmonyPostfix]
        public static void Update(FactorySystem __instance, long time, bool isActive)
        {
            PlanetExtensionSystem.Update(__instance.factory);
        }
        
        [HarmonyPatch(typeof(PlanetFactory), "GameTick")]
        [HarmonyPostfix]
        public static void PostUpdate(PlanetFactory __instance, long time)
        {
            PlanetExtensionSystem.PostUpdate(__instance);
        }
        
        //Fall-back calls for systems that do not support multi-thread update calls
        
        [HarmonyPatch(typeof(MultithreadSystem), "PreparePowerSystemFactoryData")]
        [HarmonyPrefix]
        public static void PowerUpdateSinglethread()
        {
            if (!GameMain.multithreadSystem.multithreadSystemEnable) return;
            PlanetExtensionSystem.PowerUpdateOnlySinglethread(GameMain.data);
        }
        
        [HarmonyPatch(typeof(MultithreadSystem), "PrepareAssemblerFactoryData")]
        [HarmonyPrefix]
        public static void PreUpdateSinglethread()
        {
            if (!GameMain.multithreadSystem.multithreadSystemEnable) return;
            PlanetExtensionSystem.PreUpdateOnlySinglethread(GameMain.data);
        }
        
        [HarmonyPatch(typeof(MultithreadSystem), "PrepareTransportData")]
        [HarmonyPrefix]
        public static void UpdateSinglethread()
        {
            if (!GameMain.multithreadSystem.multithreadSystemEnable) return;
            PlanetExtensionSystem.UpdateOnlySinglethread(GameMain.data);
        }
        
        [HarmonyPatch(typeof(TrashSystem), "GameTick")]
        [HarmonyPrefix]
        public static void PostUpdateSinglethread()
        {
            if (!GameMain.multithreadSystem.multithreadSystemEnable) return;
            PlanetExtensionSystem.PostUpdateOnlySinglethread(GameMain.data);
        }
        
        //Multi-thread update calls, used only if player system support multithreading
        
        [HarmonyPatch(typeof(FactorySystem), "ParallelGameTickBeforePower")]
        [HarmonyPostfix]
        public static void PowerTickMultithread(FactorySystem __instance, long time, bool isActive, int _usedThreadCnt, int _curThreadIdx)
        {
            PlanetExtensionSystem.PowerUpdateMultithread(__instance.factory, _usedThreadCnt, _curThreadIdx, 4);
        }

        [HarmonyPatch(typeof(FactorySystem), "GameTick", new Type[] {typeof(long), typeof(bool), typeof(int), typeof(int), typeof(int)})]
        [HarmonyPrefix]
        public static void PreUpdateMultithread(FactorySystem __instance, long time, bool isActive, int _usedThreadCnt, int _curThreadIdx)
        {
            PlanetExtensionSystem.PreUpdateMultithread(__instance.factory, _usedThreadCnt, _curThreadIdx, 4);
        }
        
        [HarmonyPatch(typeof(FactorySystem), "GameTickLabOutputToNext", new Type[] {typeof(long), typeof(bool), typeof(int), typeof(int), typeof(int)})]
        [HarmonyPrefix]
        public static void UpdateMultithread(FactorySystem __instance, long time, bool isActive, int _usedThreadCnt, int _curThreadIdx)
        {
            PlanetExtensionSystem.UpdateMultithread(__instance.factory, _usedThreadCnt, _curThreadIdx, 4);
        }  
        
        [HarmonyPatch(typeof(CargoTraffic), "PresentCargoPathsAsync", new Type[] {typeof(bool), typeof(int), typeof(int), typeof(int)})]
        [HarmonyPostfix]
        public static void PostUpdateMultithread(CargoTraffic __instance, bool presentCargos, int _usedThreadCnt, int _curThreadIdx)
        {
            PlanetExtensionSystem.PostUpdateMultithread(__instance.factory, _usedThreadCnt, _curThreadIdx, 4);
        }

        [HarmonyPatch(typeof(PlanetFactory), "CreateEntityLogicComponents")]
        [HarmonyPostfix]
        public static void AddComponents(int entityId, PrefabDesc desc, int prebuildId, PlanetFactory __instance)
        {
            PlanetExtensionSystem.CreateEntityComponents(__instance, entityId, desc, prebuildId);
        }

        [HarmonyPatch(typeof(PlanetFactory), "RemoveEntityWithComponents")]
        [HarmonyPrefix]
        public static void RemoveComponents(int id, PlanetFactory __instance)
        {
            PlanetExtensionSystem.RemoveEntityComponents(__instance, id);
        }
        
        [HarmonyPatch(typeof(GameData), "GetOrCreateFactory")]
        [HarmonyPostfix]
        public static void LoadNewPlanet(PlanetData planet)
        {
            PlanetExtensionSystem.InitNewPlanet(planet);
        }
    }
}