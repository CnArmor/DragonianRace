﻿using HarmonyLib;
using RimWorld;
using System.Linq;
using Verse;

namespace Dragonian
{

    [HarmonyPatch(typeof(PawnGenerator), "TryGenerateNewPawnInternal")]
    public static class Patch_PawnGenerator
    {
        [HarmonyPostfix]
        static void PawnGeneratorPostfix(ref Pawn __result)
        {
            if (__result != null)
            {
                if (__result.IsDragonian())
                {
                    __result.abilities?.GainAbility(DragonianAbilityDefOf.Dragonian_DragonbloodOverload);
                    if (!__result.health.hediffSet.HasHediff(DragonianHediffDefOf.Dragonian_AutoRecovery))
                        __result.health.AddHediff(DragonianHediffDefOf.Dragonian_AutoRecovery);
                    if (__result.skills.GetSkill(SkillDefOf.Melee).passion != Passion.Major)
                    {
                        if (Rand.Value < 0.3)
                            __result.skills.GetSkill(SkillDefOf.Melee).passion = Passion.Major;
                        else
                            __result.skills.GetSkill(SkillDefOf.Melee).passion = Passion.Minor;
                    }
                    if (__result.skills.GetSkill(SkillDefOf.Shooting).passion != Passion.None)
                        __result.skills.GetSkill(SkillDefOf.Shooting).passion = Passion.None;
                }
            }
        }
    }

    // Backward compatiblity
    [HarmonyPatch(typeof(Pawn_AbilityTracker), "ExposeData")]
    public static class Patch_Pawn_AbilityTracker
    {
        [HarmonyPostfix]
        static void Pawn_AbilityTrackerPostfix(ref Pawn_AbilityTracker __instance, ref Pawn ___pawn)
        {
            if (Scribe.mode == LoadSaveMode.ResolvingCrossRefs)
            {
                if (___pawn.IsDragonian() && !___pawn.abilities.abilities.Any(ab => ab.def == DragonianAbilityDefOf.Dragonian_DragonbloodOverload))
                {
                    __instance.GainAbility(DragonianAbilityDefOf.Dragonian_DragonbloodOverload);
                }
            }
        }
    }
}