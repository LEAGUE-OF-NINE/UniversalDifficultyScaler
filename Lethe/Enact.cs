using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using System.Linq;
using System.Text;
using MyPlugin;
using static DifficultySliderMod.FrogMain;

namespace DifficultySliderMod
{
    internal class Enact
    {
        public static async Task loadDict() {
            string filepath = "BepInEx\\plugins\\dynamicdifficultydata.json";
            string filepath3 = "BepInEx\\plugins\\dynamicdifficultydataforplayers.json";
            var keys = new List<String> { "Positive Coin Power Up" };
            var keysModularOverride = new List<String> { };
            keys.Add("Negative Coin Power Up");
            keys.Add("Final Power Up");
            keys.Add("Clash Power Up");
            keys.Add("Min Speed Adder");
            keys.Add("Max Speed Adder");
            keys.Add("Max HP Multiplier");
            keys.Add("Defense Level");
            keys.Add("Offense Level");
            keys.Add("Damage Taken");
            keys.Add("Damage Dealt");
            keys.Add("Slash Resistance");
            keys.Add("Pierce Resistance");
            keys.Add("Blunt Resistance");
            
            // These ones are coded with modular. Added to a seperate dict, so we can later apply them with setdata
            keysModularOverride.Add("Bonus Damage On Hit"); 
            keysModularOverride.Add("Bonus Flat Healing On Hit");
            keysModularOverride.Add("Bonus Flat Healing On Combat Start");
            keysModularOverride.Add("Change Stagger On Self On Hit");
            keysModularOverride.Add("Change Stagger On Self When Hit");
            keysModularOverride.Add("Encounter Start Shield");
            keysModularOverride.Add("Combat Start Shield (Stacking)");
            keysModularOverride.Add("Combat Start Shield (Non-Stacking)");
            keysModularOverride.Add("Round Start SP Healing");
            keysModularOverride.Add("Clash Win SP Healing");
            keysModularOverride.Add("Clash Lose SP Healing");


            
            // This is used to actually store the correct data ID's to overrides for these effects
            GlobalData.ModularOverridesDataIDs["Bonus Damage On Hit"] = 1406197901;
            GlobalData.ModularOverridesDataIDs["Bonus Flat Healing On Hit"] = 1406197902;
            GlobalData.ModularOverridesDataIDs["Bonus Flat Healing On Combat Start"] = 1406197903;
            GlobalData.ModularOverridesDataIDs["Change Stagger On Self On Hit"] = 1406197904;
            GlobalData.ModularOverridesDataIDs["Change Stagger On Self When Hit"] = 1406197905;
            GlobalData.ModularOverridesDataIDs["Encounter Start Shield"] = 1406197906;
            GlobalData.ModularOverridesDataIDs["Combat Start Shield (Stacking)"] = 1406197907;
            GlobalData.ModularOverridesDataIDs["Combat Start Shield (Non-Stacking)"] = 1406197908;
            
            GlobalData.ModularOverridesDataIDs["Round Start SP Healing"] = 1406197909;
            GlobalData.ModularOverridesDataIDs["Clash Win SP Healing"] = 1406197910;
            GlobalData.ModularOverridesDataIDs["Clash Lose SP Healing"] = 1406197911;


            
            // MyData is used for enemies. MyDataTwo for sinners
            GlobalData.MyData = await CreateAndManageDict.LoadOrCreateDictionaryAsync(filepath, keys);
            GlobalData.MyDataTwo = await CreateAndManageDict.LoadOrCreateDictionaryAsync(filepath3, keys);
            
            // Meanwhile, these store the actual values we want to set
            GlobalData.MyDataModularOverrides = await CreateAndManageDict.LoadOrCreateDictionaryAsync(filepath, keysModularOverride);
            GlobalData.MyDataModularOverridesTwo = await CreateAndManageDict.LoadOrCreateDictionaryAsync(filepath3, keysModularOverride);
        }

        public static void EnactShit() 
        {
            EnactForBuff(
                "DifficultModder",
                FrogMain.GlobalData.MyData
            );

            // Player-only difficulty
            EnactForBuff(
                "DifficultModderPlayer",
                FrogMain.GlobalData.MyDataTwo
            );
        }
        
        private static void EnactForBuff(string buffId, Dictionary<string, float> data)
        {
            foreach (var buff in Singleton<StaticDataManager>.Instance._buffList.list)
            {
                if (buff.id != buffId)
                    continue;

                foreach (var p in buff.list)
                {
                    switch (p.ability)
                    {
                        case "PlusCoinScaleAdder":
                            p.value = data["Positive Coin Power Up"];
                            break;

                        case "MinusCoinScaleAdder":
                            p.value = data["Negative Coin Power Up"];
                            break;

                        case "SkillPowerResultAdder":
                            p.value = data["Final Power Up"];
                            break;

                        case "ParryingResultAdderAsStack":
                            p.value = data["Clash Power Up"];
                            break;

                        case "MinSpeedAdder":
                            p.value = data["Min Speed Adder"];
                            break;

                        case "MaxSpeedAdder":
                            p.value = data["Max Speed Adder"];
                            break;

                        case "DefaultMaxHpMultiplier":
                            p.value = data["Max HP Multiplier"];
                            break;

                        case "DefAdder":
                            p.value = data["Defense Level"];
                            break;

                        case "AtkAdder":
                            p.value = data["Offense Level"];
                            break;

                        case "TakeAtkDamageMultiplierByStack":
                            p.value = data["Damage Taken"];
                            break;

                        case "AtkDamageMultiplierByStack":
                            p.value = data["Damage Dealt"];
                            break;

                        case "AtkResistAdderByStack":
                            if (p.atk == "SLASH")
                                p.value = data["Slash Resistance"];
                            else if (p.atk == "HIT")
                                p.value = data["Blunt Resistance"];
                            else if (p.atk == "PENETRATE")
                                p.value = data["Pierce Resistance"];
                            break;

                        default:
                            FrogMain.Logg.LogMessage(
                                $"Unhandled ability '{p.ability}' in EnactForBuff({buffId})"
                            );
                            break;
                    }
                }

                break;
            }
        }

    }
}
