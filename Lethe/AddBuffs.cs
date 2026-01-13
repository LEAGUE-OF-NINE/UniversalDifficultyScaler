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
using ModularSkillScripts.Patches;

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
            Enact.loadDict().GetAwaiter().GetResult();
            Enact.EnactShit();
        }

        [HarmonyPatch(typeof(BattleUnitModel), nameof(BattleUnitModel.OnRoundStart_After_Event))]
        [HarmonyPostfix]
        private static void Postfix_BattleUnitModel_OnRoundStart_After_Event(BATTLE_EVENT_TIMING timing, BattleUnitModel __instance)
        {

            SinManager sinManager_inst = Singleton<SinManager>.Instance;
            
            var faction = __instance.Faction;

            System.Collections.Generic.Dictionary<string, float> targetDict;
            if (faction == UNIT_FACTION.PLAYER) targetDict = FrogMain.GlobalData.MyDataModularOverridesTwo;
            else targetDict = FrogMain.GlobalData.MyDataModularOverrides;
            
            long targetPtr_intlong = __instance.Pointer.ToInt64();
            foreach (var (buffKey, dataID) in FrogMain.GlobalData.ModularOverridesDataIDs)
            {
                FrogMain.Logg.LogMessage("Currently overriding setdata to " + __instance.ToString() + " with Ptr_intlong" + targetPtr_intlong);
                var dataValueFloat = targetDict[buffKey];
                int dataValueInt = (int)dataValueFloat;
                SkillScriptInitPatch.SetModUnitData(targetPtr_intlong, dataID, dataValueInt);
            }

            BattleUnitModel_Abnormality abno = __instance.TryCast<BattleUnitModel_Abnormality>();
            BattleUnitModel_Abnormality_Part part = __instance.TryCast<BattleUnitModel_Abnormality_Part>();

            if (abno == null && !__instance.HasPassive(14461979))
            {
                __instance.AddPassive(14461979);
            }

            if (part != null) return;
            
            BUFF_UNIQUE_KEYWORD buf_keyword = faction == UNIT_FACTION.PLAYER ? FrogMain.buf_keywordPlayer : FrogMain.buf_keyword;
            
            if (__instance.GetActivatedBuffStack(buf_keyword, false) > 0)
            {
                FrogMain.Logg.LogMessage("Buff Stack is greater than 0 for" + __instance.ToString());
            }
            else
            {
                __instance.AddBuff_Giver(buf_keyword, 1, __instance, timing, 0, 0, ABILITY_SOURCE_TYPE.PASSIVE,
                                         null, out _, out _, out _, out _);
                //__instance.AddShield((int)FrogMain.GlobalData.MyData["Encounter Start Shield"], false, ABILITY_SOURCE_TYPE.BUFF, timing);
            }

        }
    }
}
