using BepInEx;
using BepInEx.Logging;
using ShockingContent.Attributes;
using System;
using System.Reflection;
using Zorro.Core;

namespace ShockingContent
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class ShockingContent : BaseUnityPlugin
    {
        internal enum ShockModes
        {
            FIRST, LAST, RANDOM, ROUND_ROBIN, RANDOM_ALL, ALL
        }
        public static ShockingContent Instance { get; private set; } = null!;
        internal DateTime lastShock;
        public static ManualLogSource logger;

        private void Awake()
        {

            logger = Logger;
            Instance = this;

            Hook();

            Logger.LogInfo($"ShockingContent {PluginInfo.PLUGIN_GUID} is loaded!");
        }

        private void Hook()
        {
            ValueTuple<MethodInfo, Attribute>[] patchMethods = ReflectionUtility.GetMethodsWithAttribute<Patch>();
            foreach (var item in patchMethods)
            {
                var method = item.Item1;
                method.Invoke(null, Array.Empty<object>());
            }

            Logger.LogInfo("Patching complete");
        }
    }
}
