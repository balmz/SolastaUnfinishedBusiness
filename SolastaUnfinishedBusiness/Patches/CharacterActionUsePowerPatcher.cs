﻿using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.Patches;

internal static class CharacterActionUsePowerPatcher
{
    [HarmonyPatch(typeof(CharacterActionUsePower), "CheckInterruptionBefore")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class CheckInterruptionBefore_Patch
    {
        internal static bool Prefix([NotNull] CharacterActionUsePower __instance)
        {
            //PATCH: ignores interruptions processing for certain powers so they won't interrupt invisibility
            return !PowersContext.PowersThatIgnoreInterruptions.Contains(__instance.activePower.PowerDefinition);
        }
    }

    [HarmonyPatch(typeof(CharacterActionUsePower), "CheckInterruptionAfter")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class CheckInterruptionAfter_Patch
    {
        internal static bool Prefix([NotNull] CharacterActionUsePower __instance)
        {
            //PATCH: ignores interruptions processing for certain powers so they won't interrupt invisibility
            return !PowersContext.PowersThatIgnoreInterruptions.Contains(__instance.activePower.PowerDefinition);
        }
    }

    [HarmonyPatch(typeof(CharacterActionUsePower), "GetAdvancementData")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class GetAdvancementData_Patch
    {
        internal static bool Prefix([NotNull] CharacterActionUsePower __instance)
        {
            //PATCH: Calculate advancement data for `RulesetEffectPowerWithAdvancement`
            return RulesetEffectPowerWithAdvancement.GetAdvancementData(__instance);
        }
    }

    [HarmonyPatch(typeof(CharacterActionUsePower), "SpendMagicEffectUses")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class SpendMagicEffectUses_Patch
    {
        internal static bool Prefix([NotNull] CharacterActionUsePower __instance)
        {
            //PATCH: Calculate extra charge usage for `RulesetEffectPowerWithAdvancement`

            if (__instance.activePower.OriginItem == null
                || __instance.activePower is not RulesetEffectPowerWithAdvancement power)
            {
                return true;
            }

            var usableDevice = power.OriginItem;

            foreach (RulesetDeviceFunction usableFunction in usableDevice.UsableFunctions)
            {
                var functionDescription = usableFunction.DeviceFunctionDescription;
                if (functionDescription.Type != DeviceFunctionDescription.FunctionType.Power
                    || functionDescription.FeatureDefinitionPower != power.PowerDefinition)
                {
                    continue;
                }

                __instance.ActingCharacter.RulesetCharacter
                    .UseDeviceFunction(usableDevice, usableFunction, power.ExtraCharges);
                break;
            }

            ServiceRepository.GetService<IGameLocationActionService>()
                .ItemUsed?.Invoke(usableDevice.ItemDefinition.Name);

            return false;
        }
    }
}
