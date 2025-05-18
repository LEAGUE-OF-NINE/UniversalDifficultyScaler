using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DifficultySliderMod;
using HarmonyLib;
using Il2CppInterop.Runtime.Injection;
using Il2CppSystem.Collections.Generic;
using Lethe;
using Lethe.Patches;
using UnityEngine;
using static BuffModel;

namespace MyPlugin
{
    internal class AddBuffs : MonoBehaviour
    {
        public static void Setup(Harmony harmony)
        {
            ClassInjector.RegisterTypeInIl2Cpp<AddBuffs>();
            harmony.PatchAll(typeof(AddBuffs));
        }

        [HarmonyPatch(typeof(BattleUnitModel), nameof(BattleUnitModel.OnStageStart))]
        [HarmonyPostfix]
        private static void Postfix_BattleUnitModel_OnStageStart(BATTLE_EVENT_TIMING timing, BattleUnitModel __instance)
        {
            Enact.loadDict();
            Enact.EnactShit();

        }

        [HarmonyPatch(typeof(BattleUnitModel), nameof(BattleUnitModel.OnRoundStart_After_Event))]
        [HarmonyPostfix]
        private static void Postfix_BattleUnitModel_EnactBuffAdding(BATTLE_EVENT_TIMING timing, BattleUnitModel __instance)
        {

            SinManager sinManager_inst = Singleton<SinManager>.Instance;
            BattleObjectManager battleObjectManager = sinManager_inst._battleObjectManager;

            if (!__instance.IsAbnormalityOrPart) { return; }
            if (__instance.Faction != UNIT_FACTION.ENEMY) { return; }

            var outer = 1;
            var inner = 0;
            FrogMain.Logg.LogMessage("Currently adding buff to " + __instance.ToString());
            __instance.AddBuff_Giver(FrogMain.buf_keyword, 1, __instance, timing, 0, 0, ABILITY_SOURCE_TYPE.PASSIVE, null, out outer, out inner);

            foreach (BattleUnitModel unit in battleObjectManager.GetAliveListExceptAbnormalitySelf(UNIT_FACTION.ENEMY, false))
            {
                unit.LoseBuffAllStack(FrogMain.buf_keyword,  timing);
            }


        }

    }
}
