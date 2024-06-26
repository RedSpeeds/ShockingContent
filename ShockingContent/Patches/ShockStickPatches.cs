﻿using ShockingContent.Attributes;
using ShockingContent.Util;

namespace ShockingContent.Patches
{
    public static class ShockStickPatches
    {
        [Patch]
        public static void Init()
        {
            ShockingContentPlugin.logger.LogInfo("Patching shockstick");

            On.ShockStick.OnShock += ShockStick_OnShock;
        }

        private static void ShockStick_OnShock(On.ShockStick.orig_OnShock orig, ShockStick self, Player playerToShock)
        {
            orig(self, playerToShock);
            ShockingContentPlugin.logger.LogDebug("Checking if enabled");
            if (!playerToShock.IsLocal || !Config.ShockStick.Enabled.Value) return;
            ShockingContentPlugin.logger.LogDebug("Doing operation");
            Operations.DoOperation(Config.ShockStick.Intensity.Value, Config.ShockStick.Duration.Value, Config.ShockStick.ShockMode.Value, Config.ShockStick.WarningVibration.Value, Config.ShockStick.VibrateOnly.Value, Config.ShockStick.Beep.Value);
        }
    }
}
