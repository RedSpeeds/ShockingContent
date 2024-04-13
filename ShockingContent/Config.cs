using BepInEx;
using BepInEx.Configuration;
using static ShockingContent.ShockingContent;

namespace ShockingContent
{
    internal static class Config
    {
        private static ConfigFile file { get; set; }
        internal static ConfigEntry<string> PishockUsername;
        internal static ConfigEntry<string> PishockApiKey;
        internal static ConfigEntry<string> PishockCodes;

        internal static class Player
        {
            internal static class Damage
            {
                internal static ConfigEntry<bool> Enabled;
                internal static ConfigEntry<int> Duration;
                internal static ConfigEntry<bool> VibrateOnly;
                internal static ConfigEntry<bool> WarningVibration;
                internal static ConfigEntry<bool> BasedOnRemainingHealth;
                internal static ConfigEntry<int> MaxIntensity;
                internal static ConfigEntry<int> MinIntensity;
                internal static ConfigEntry<ShockModes> ShockMode;
                internal static ConfigEntry<int> Interval;

            }
            internal static class Death
            {
                internal static ConfigEntry<bool> Enabled;
                internal static ConfigEntry<int> Duration;
                internal static ConfigEntry<int> Intensity;
                internal static ConfigEntry<bool> VibrateOnly;
                internal static ConfigEntry<bool> WarningVibration;
                internal static ConfigEntry<ShockModes> ShockMode;
                internal static ConfigEntry<bool> Beep;

            }
            internal static class Heal
            {
                internal static ConfigEntry<bool> Enabled;
                internal static ConfigEntry<int> Duration;
                internal static ConfigEntry<int> Intensity;
                internal static ConfigEntry<ShockModes> ShockMode;
                internal static ConfigEntry<bool> Shock;

            }
            internal static class Revive
            {
                internal static ConfigEntry<bool> Enabled;
                internal static ConfigEntry<int> Duration;
                internal static ConfigEntry<int> Intensity;
                internal static ConfigEntry<bool> WarningVibration;
                internal static ConfigEntry<bool> VibrateOnly;
                internal static ConfigEntry<ShockModes> ShockMode;
                internal static ConfigEntry<bool> Beep;
            }
        }
        internal static class ShockStick
        {
            internal static ConfigEntry<bool> Enabled;
            internal static ConfigEntry<int> Duration;
            internal static ConfigEntry<int> Intensity;
            internal static ConfigEntry<bool> WarningVibration;
            internal static ConfigEntry<bool> VibrateOnly;
            internal static ConfigEntry<ShockModes> ShockMode;
            internal static ConfigEntry<bool> Beep;
        }
        static Config()
        {
            file = new ConfigFile(Paths.ConfigPath + "\\ShockingContent.cfg", true);

            PishockApiKey = file.Bind("PiShock", "ApiKey", "", "PiShock API key");
            // Binding for the username
            PishockUsername = file.Bind("PiShock", "Username", "", "PiShock username.");

            // Binding for the codes
            PishockCodes = file.Bind("PiShock", "Codes", "", "codes for PiShock.");

            // Player Damage configuration bindings
            Player.Damage.Enabled = file.Bind("Player.Damage", "Enabled", true, "Enable or disable damage effects.");
            Player.Damage.Duration = file.Bind("Player.Damage", "Duration", 1, "Duration of the damage effect in seconds.");
            Player.Damage.VibrateOnly = file.Bind("Player.Damage", "VibrateOnly", false, "If true, only vibrate instead of using shocks.");
            Player.Damage.WarningVibration = file.Bind("Player.Damage", "WarningVibration", false, "If true, provides a warning vibration before actual effect.");
            Player.Damage.BasedOnRemainingHealth = file.Bind("Player.Damage", "BasedOnRemainingHealth", false, "If true, effects are based on the remaining health.");
            Player.Damage.MaxIntensity = file.Bind("Player.Damage", "MaxIntensity", 80, "Maximum intensity of the shock.");
            Player.Damage.MinIntensity = file.Bind("Player.Damage", "MinIntensity", 0, "Minimum intensity of the shock.");
            Player.Damage.ShockMode = file.Bind("Player.Damage", "ShockMode", ShockModes.ALL, "Mode of shock to be used.");
            Player.Damage.Interval = file.Bind("Player.Damage", "Interval", 0, "Interval between shocks.");

            // Player Death configuration bindings
            Player.Death.Enabled = file.Bind("Player.Death", "Enabled", true, "Enable or disable death effects.");
            Player.Death.Duration = file.Bind("Player.Death", "Duration", 5, "Duration of the death effect in seconds.");
            Player.Death.Intensity = file.Bind("Player.Death", "Intensity", 100, "Intensity of the death effect.");
            Player.Death.VibrateOnly = file.Bind("Player.Death", "VibrateOnly", false, "If true, only vibrate instead of using shocks.");
            Player.Death.WarningVibration = file.Bind("Player.Death", "WarningVibration", true, "If true, provides a warning vibration before actual effect.");
            Player.Death.ShockMode = file.Bind("Player.Death", "ShockMode", ShockModes.ALL, "Mode of shock to be used.");
            Player.Death.Beep = file.Bind("Player.Death", "Beep", false, "If true, emits a beep sound on death.");

            // Player Heal configuration bindings
            Player.Heal.Enabled = file.Bind("Player.Heal", "Enabled", false, "Enable or disable heal effects.");
            Player.Heal.Duration = file.Bind("Player.Heal", "Duration", 0, "Duration of the heal effect in seconds.");
            Player.Heal.Intensity = file.Bind("Player.Heal", "Intensity", 0, "Intensity of the heal effect.");
            Player.Heal.ShockMode = file.Bind("Player.Heal", "ShockMode", ShockModes.ALL, "Mode of shock to be used.");
            Player.Heal.Shock = file.Bind("Player.Heal", "Shock", false, "If true, uses shock along with healing.");

            // Player Revive configuration bindings
            Player.Revive.Enabled = file.Bind("Player.Revive", "Enabled", true, "Enable or disable revive effects.");
            Player.Revive.Duration = file.Bind("Player.Revive", "Duration", 2, "Duration of the revive effect in seconds.");
            Player.Revive.Intensity = file.Bind("Player.Revive", "Intensity", 30, "Intensity of the revive effect.");
            Player.Revive.WarningVibration = file.Bind("Player.Revive", "WarningVibration", false, "If true, provides a warning vibration before actual effect.");
            Player.Revive.VibrateOnly = file.Bind("Player.Revive", "VibrateOnly", false, "If true, only vibrate instead of using shocks.");
            Player.Revive.ShockMode = file.Bind("Player.Revive", "ShockMode", ShockModes.ALL, "Mode of shock to be used.");
            Player.Revive.Beep = file.Bind("Player.Revive", "Beep", true, "If true, emits a beep sound on revive.");

            // ShockStick configuration bindings
            ShockStick.Enabled = file.Bind("ShockStick", "Enabled", false, "Enable or disable the ShockStick.");
            ShockStick.Duration = file.Bind("ShockStick", "Duration", 0, "Duration of the ShockStick effect in seconds.");
            ShockStick.Intensity = file.Bind("ShockStick", "Intensity", 0, "Intensity of the ShockStick effect.");
            ShockStick.WarningVibration = file.Bind("ShockStick", "WarningVibration", false, "If true, provides a warning vibration before actual effect.");
            ShockStick.VibrateOnly = file.Bind("ShockStick", "VibrateOnly", false, "If true, only vibrate instead of using shocks.");
            ShockStick.ShockMode = file.Bind("ShockStick", "ShockMode", ShockModes.ALL, "Mode of shock to be used.");
            ShockStick.Beep = file.Bind("ShockStick", "Beep", false, "If true, emits a beep sound with the effect.");

        }
    }
}
