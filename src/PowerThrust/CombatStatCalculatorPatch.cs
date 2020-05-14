﻿using HarmonyLib;
using TaleWorlds.Core;


namespace CombatModCollection
{
    [HarmonyPatch(typeof(CombatStatCalculator), "CalculateStrikeMagnitudeForThrust")]
    public class CalculateStrikeMagnitudeForThrustPatch
    {
        public static bool Prefix(ref float __result,
            float thrustWeaponSpeed,
            float weaponWeight,
            float extraLinearSpeed,
            bool isThrown)
        {
            extraLinearSpeed = extraLinearSpeed / 0.5f * 0.7f; // Compensate Mission.CalculateBaseBlowMagnitude
            float num = thrustWeaponSpeed + extraLinearSpeed;
            if (!isThrown)
                weaponWeight += 2.5f;
            __result = Settings.Instance.Battle_PowerThrust_kE * (0.5f * weaponWeight * num * num)
                + Settings.Instance.Battle_PowerThrust_kP * weaponWeight * num
                + Settings.Instance.Battle_PowerThrust_kC;

            return false;
        }

        public static bool Prepare()
        {
            return Settings.Instance.Battle_PowerThrust;
        }
    }
}
