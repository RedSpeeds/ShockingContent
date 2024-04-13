using BepInEx;
using LethalShock;
using System.Threading.Tasks;
using System;
using BepInEx.Configuration;
using BepInEx.Logging;
using static ShockingContent.ShockingContent;
using UnityEngine;
using Random = System.Random;

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
        private readonly Random rnd = new();
        internal DateTime lastShock;
        internal ManualLogSource mls;
        internal readonly string pishockLogId = "ShockingContent (Content warning)";
        internal ConfigEntry<string> pishockUsername;
        internal ConfigEntry<string> pishockApiKey;
        internal ConfigEntry<string> pishockCodes;
        internal ConfigEntry<int> duration;
        internal ConfigEntry<int> maxIntensity;
        internal ConfigEntry<int> minIntensity;
        internal ConfigEntry<ShockModes> shockMode;
        internal ConfigEntry<bool> vibrateOnly;
        internal ConfigEntry<bool> warningVibration;
        internal ConfigEntry<bool> enableInterval;
        internal ConfigEntry<int> interval;
        internal ConfigEntry<bool> shockBasedOnHealth;
        internal ConfigEntry<bool> shockOnDamage;
        internal ConfigEntry<bool> shockOnDeath;
        internal ConfigEntry<int> durationDeath;
        internal ConfigEntry<int> intensityDeath;
        internal ConfigEntry<ShockModes> modeDeath;
        private void Awake()
        {
            // Plugin startup logic
            Logger.LogInfo($"ShockingContent {PluginInfo.PLUGIN_GUID} is loaded!");
        }

        internal async void DoOperation(int intensity, int duration, ShockModes mode)
        {
            string[] codes = pishockCodes.Value.Split(',');
            bool[] picked = PickShockers(mode, codes.Length);
            for (int i = 0; i < codes.Length; i++)
            {
                mls.LogDebug("Running DoOperation for shocker coded " + codes[i]);
                if (!picked[i]) continue;
                PiShockApi user = new()
                {
                    username = pishockUsername.Value,
                    apiKey = pishockApiKey.Value,
                    code = codes[i],
                    senderName = pishockLogId
                };

                if (vibrateOnly.Value || warningVibration.Value)
                {
                    await user.Vibrate(intensity, duration);
                    if (!vibrateOnly.Value)
                    {
                        mls.LogDebug("Vibrating with delay");
                        await Task.Delay(duration + 1 * 1000);
                        mls.LogDebug("Shocking after delay");
                        await user.Shock(intensity, duration);
                    }
                }
                else
                {
                    await user.Shock(intensity, duration);
                }

            }
        }

        public bool NextBoolean(Random random)
        {
            return random.Next() > (Int32.MaxValue / 2);
        }

        private int roundRobin = 0;

        private bool[] PickShockers(ShockModes mode, int length)
        {
            bool[] shocks = new bool[length];
            int ranindex = rnd.Next(0, length);
            if (roundRobin >= length) roundRobin = 0;

            for (int i = 0; i < length; i++)
            {
                switch (mode)
                {
                    case ShockModes.ALL:
                        shocks[i] = true;
                        break;
                    case ShockModes.RANDOM_ALL:
                        shocks[i] = NextBoolean(rnd);
                        break;
                    case ShockModes.RANDOM:
                        shocks[i] = i == ranindex;
                        break;
                    case ShockModes.FIRST:
                        shocks[i] = i == 0;
                        break;
                    case ShockModes.LAST:
                        shocks[i] = i == length - 1;
                        break;
                    case ShockModes.ROUND_ROBIN:
                        shocks[i] = i == roundRobin;
                        break;
                }
            }
            roundRobin++;
            if (mode == ShockModes.RANDOM_ALL)
            {
                bool hasShock = false;
                foreach (bool item in shocks)
                {
                    if (item)
                    {
                        hasShock = true;
                        break;
                    }
                }
                if (!hasShock) shocks[ranindex] = true;
            }
            return shocks;
        }
        internal void DoDamage(int dmg, int health)
        {
            TimeSpan calculatedTime = DateTime.Now - lastShock;
            if (enableInterval.Value && calculatedTime < TimeSpan.FromSeconds(interval.Value))
            {
                Logger.LogDebug("Didn't shock due to interval. LastShock: " + lastShock.ToLongTimeString());
                return;
            }
            int maxDmgShock = Mathf.Clamp(dmg, minIntensity.Value, maxIntensity.Value);
            int shockHealth = 100 - health;
            int maxHealthShock = Mathf.Clamp(shockHealth, minIntensity.Value, maxIntensity.Value);
            if (shockBasedOnHealth.Value)
            {
                mls.LogInfo("Shocking based on health for " + maxHealthShock);
                DoOperation(maxHealthShock, duration.Value, shockMode.Value);
            }
            else if (shockOnDamage.Value)
            {
                mls.LogInfo("Shocking based on damage for " + maxDmgShock);
                DoOperation(maxDmgShock, duration.Value, shockMode.Value);
            }
            lastShock = DateTime.Now;
        }
        private bool DidDeath = false;
        internal void DoDeath()
        {
            if (DidDeath || !shockOnDeath.Value) return;
            Logger.LogInfo("Death shock");
            DoOperation(intensityDeath.Value, durationDeath.Value, modeDeath.Value);
            DidDeath = true;
            Task.Run(async () =>
            {
                await Task.Delay(20000);
                DidDeath = false;
            });
        }
    }
}
