using System;
using System.Threading.Tasks;
using UnityEngine;
using static ShockingContent.ShockingContentPlugin;
using Random = System.Random;

namespace ShockingContent.Util
{
    internal static class Operations
    {
        internal static readonly string pishockLogId = "ShockingContentPlugin (Content warning)";
        private static readonly Random rnd = new();
        private static DateTime lastShock;
        internal static async void DoOperation(int intensity, int duration, ShockModes mode, bool warning = false, bool vibrateOnly = false, bool beep = false)
        {
            string[] codes = Config.PishockCodes.Value.Split(',');
            bool[] picked = PickShockers(mode, codes.Length);
            for (int i = 0; i < codes.Length; i++)
            {
                logger.LogDebug("Running DoOperation for shocker coded " + codes[i]);
                if (!picked[i]) continue;
                PiShockApi user = new()
                {
                    username = Config.PishockUsername.Value,
                    apiKey = Config.PishockApiKey.Value,
                    code = codes[i],
                    senderName = pishockLogId
                };
                if (beep)
                {
                    await user.Beep(3000);
                }
                if (vibrateOnly || warning)
                {
                    await user.Vibrate(intensity, duration);
                    if (!vibrateOnly)
                    {
                        logger.LogDebug("Vibrating with delay");
                        await Task.Delay(duration + 1 * 1000);
                        logger.LogDebug("Shocking after delay");
                        await user.Shock(intensity, duration);
                    }
                }
                else
                {
                    await user.Shock(intensity, duration);
                }

            }
        }

        public static bool NextBoolean(Random random)
        {
            return random.Next() > (Int32.MaxValue / 2);
        }

        private static int roundRobin = 0;

        private static bool[] PickShockers(ShockModes mode, int length)
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
        internal static void DoDamage(int dmg, int health)
        {
            TimeSpan calculatedTime = DateTime.Now - lastShock;
            if (calculatedTime < TimeSpan.FromSeconds(Config.Player.Damage.Interval.Value))
            {
                logger.LogDebug("Didn't shock due to interval. LastShock: " + lastShock.ToLongTimeString());
                return;
            }
            int maxDmgShock = Mathf.Clamp(dmg, Config.Player.Damage.MinIntensity.Value, Config.Player.Damage.MaxIntensity.Value);
            int shockHealth = 100 - health;
            int maxHealthShock = Mathf.Clamp(shockHealth, Config.Player.Damage.MinIntensity.Value, Config.Player.Damage.MaxIntensity.Value);
            if (Config.Player.Damage.BasedOnRemainingHealth.Value)
            {
                logger.LogInfo("Shocking based on health for " + maxHealthShock);
                DoOperation(maxHealthShock, Config.Player.Damage.Duration.Value, Config.Player.Damage.ShockMode.Value, Config.Player.Damage.WarningVibration.Value, Config.Player.Damage.VibrateOnly.Value);
            }
            else if (Config.Player.Damage.Enabled.Value)
            {
                logger.LogInfo("Shocking based on damage for " + maxDmgShock);
                DoOperation(maxDmgShock, Config.Player.Damage.Duration.Value, Config.Player.Damage.ShockMode.Value, Config.Player.Damage.WarningVibration.Value, Config.Player.Damage.VibrateOnly.Value);
            }
            lastShock = DateTime.Now;
        }
        private static bool DidDeath = false;
        internal static void DoDeath()
        {
            if (DidDeath || !Config.Player.Death.Enabled.Value) return;
            logger.LogInfo("Death shock");
            if (Config.Player.Death.Beep.Value)
            {

            }
            DoOperation(Config.Player.Death.Intensity.Value, Config.Player.Death.Duration.Value, Config.Player.Death.ShockMode.Value, Config.Player.Death.WarningVibration.Value, Config.Player.Death.VibrateOnly.Value, Config.Player.Death.Beep.Value);
            DidDeath = true;
            Task.Run(async () =>
            {
                await Task.Delay(20000);
                DidDeath = false;
            });
        }
    }
}
