using BepInEx;
using BepInEx.Logging;
using ShockingContent.Attributes;
using System;
using System.Reflection;
using Zorro.Core;

namespace ShockingContent
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class ShockingContentPlugin : BaseUnityPlugin
    {
        internal enum ShockModes
        {
            FIRST, LAST, RANDOM, ROUND_ROBIN, RANDOM_ALL, ALL
        }
        public static ShockingContentPlugin Instance { get; private set; } = null!;
        public static ManualLogSource logger;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Used by loader")]
        private void Awake()
        {

            logger = Logger;
            Instance = this;

            Hook();
            Logger.LogInfo("API KEY IS: "+ShockingContent.Config.PishockApiKey.Value);
            Logger.LogInfo($"ShockingContentPlugin {PluginInfo.PLUGIN_GUID} is loaded!");
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
