using ShockingContent.Attributes;
using ShockingContent.Util;
using System;

namespace ShockingContent.Patches
{
    public static class PlayerPatches
    {
        [Patch]
        public static void Init()
        {
            ShockingContent.logger.LogInfo("Patching player");

            On.Player.TakeDamage += Player_TakeDamage;
            On.Player.Die += Player_Die;
            On.Player.CallRevive += Player_CallRevive;
            On.Player.Heal += Player_Heal;
        }

        private static bool Player_Heal(On.Player.orig_Heal orig, Player self, float healAmount)
        {
            var og = orig(self, healAmount);
            if (!self.IsLocal || !Config.Player.Heal.Enabled.Value) return og;
            Operations.DoOperation(Config.Player.Heal.Intensity.Value, Config.Player.Heal.Duration.Value, Config.Player.Heal.ShockMode.Value, false, !Config.Player.Heal.Shock.Value);
            return og;
        }

        private static void Player_CallRevive(On.Player.orig_CallRevive orig, Player self)
        {
            orig(self);
            if (!self.IsLocal || !Config.Player.Revive.Enabled.Value) return;

            Operations.DoOperation(Config.Player.Revive.Intensity.Value, Config.Player.Revive.Duration.Value, Config.Player.Revive.ShockMode.Value, Config.Player.Revive.WarningVibration.Value, Config.Player.Revive.VibrateOnly.Value, Config.Player.Revive.Beep.Value);
        }

        private static void Player_Die(On.Player.orig_Die orig, Player self)
        {
            orig(self);

            if (!self.IsLocal || !Config.Player.Death.Enabled.Value) return;

            Operations.DoDeath();
        }

        private static void Player_TakeDamage(On.Player.orig_TakeDamage orig, Player self, float damage)
        {
            orig(self, damage);

            if (!self.IsLocal || !Config.Player.Damage.Enabled.Value) return;

            Operations.DoDamage((int)Math.Round(damage), (int)Math.Round(self.data.health));
        }
    }
}
