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
            string filepath = "BepInEx\\plugins\\data.json";
            var keys = new List<String> { "Positive Coin Power Up" };
            keys.Add("Negative Coin Power Up");
            keys.Add("Final Power Up");
            keys.Add("Clash Power Up");
            keys.Add("Min Speed Adder");
            keys.Add("Max Speed Adder");
            keys.Add("Max HP Multiplier");
            keys.Add("Defense Level");
            keys.Add("Offense Level");
            keys.Add("Damage Taken");
            //keys.AddItem("Negative Coin Power Up");



            GlobalData.MyData = await CreateAndManageDict.LoadOrCreateDictionaryAsync(filepath, keys);
        }

        public static void EnactShit() 
        {
            foreach (var buff in Singleton<StaticDataManager>.Instance._buffList.list)
            {
                if (buff.id != "DifficultModder") { continue; }
                foreach (var p in buff.list) 
                {
                    switch (p.ability) 
                    {
                        case "PlusCoinScaleAdder": p.value = FrogMain.GlobalData.MyData["Positive Coin Power Up"];
                            break;
                        case "MinusCoinScaleAdder": p.value = FrogMain.GlobalData.MyData["Negative Coin Power Up"];
                            break;
                        case "SkillPowerResultAdder": p.value = FrogMain.GlobalData.MyData["Final Power Up"];
                            break;
                        case "ParryingResultAdderAsStack": p.value = FrogMain.GlobalData.MyData["Clash Power Up"]; 
                            break;
                        case "MinSpeedAdder": p.value = FrogMain.GlobalData.MyData["Min Speed Adder"];
                            break;
                        case "MaxSpeedAdder": p.value = FrogMain.GlobalData.MyData["Max Speed Adder"];
                            break;
                        case "DefaultMaxHpMultiplier": p.value = FrogMain.GlobalData.MyData["Max HP Multiplier"];
                            break;
                        case "DefAdder": p.value = FrogMain.GlobalData.MyData["Defense Level"];
                            break;
                        case "AtkAdder": p.value = FrogMain.GlobalData.MyData["Offense Level"];
                            break;
                        case "TakeAtkDamageMultiplierByStack": p.value = FrogMain.GlobalData.MyData["Damage Taken"];
                            break;
                        case "AtkDamageMultiplierByStack": p.value = FrogMain.GlobalData.MyData["Damage Dealt"];
                            break;
                        default: FrogMain.Logg.LogMessage("had a failure on the switch case. INSIDE OF ENACT. FUNCTION: EnactShit()");
                            break;
                            
                    };
                }
                break;
            }

        }
    }
}
