using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using Lethe.Patches;
using DifficultySliderMod;
using UnityEngine.Rendering.Universal.LibTessDotNet;
using MyPlugin;


namespace DifficultySliderMod;

[BepInPlugin(GUID, NAME, VERSION)]
[BepInDependency("Lethe")]
public class FrogMain : BasePlugin
{
    public const string NAME = "FroggoDifficultyAdder";
    public const string VERSION = "1.0.0";
    public const string AUTHOR = "Froggo";
    public const string GUID = AUTHOR + "." + NAME;

    public static BUFF_UNIQUE_KEYWORD buf_keyword = CustomBuffs.ParseBuffUniqueKeyword("DifficultModder");

    public static class GlobalData
    {
        public static Dictionary<string, float> MyData { get; set; }
    }


    public override async void Load()
    {
        FrogMain.Logg = new ManualLogSource(NAME);
        BepInEx.Logging.Logger.Sources.Add(FrogMain.Logg);
        var harmony = new Harmony(NAME);

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
        keys.Add("Damage Dealt");
        //keys.AddItem("Negative Coin Power Up");



        GlobalData.MyData = await CreateAndManageDict.LoadOrCreateDictionaryAsync(filepath, keys);

        AddBuffs.Setup(harmony);
    }

    public static ManualLogSource Logg;
}
