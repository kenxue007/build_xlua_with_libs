using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using System.Collections;
using XLua;


#if HOTFIX_ENABLE
[LuaCallCSharp]
public static class LuaHelper
{
    public static bool IsNull(UnityEngine.Object o)
    {
        return o == null;
    }
}

public static class XLuaAdapter
{
    [LuaCallCSharp]
    public static List<Type> ClassWrapper = new List<Type>
        {
            typeof(Vector3),
            typeof(Quaternion),
            typeof(Component),
            typeof(Transform),
            typeof(MonoBehaviour),
        };

    public static readonly IEnumerable<Type> HotfixBaseClassFilter = new List<Type>
    {
        typeof (Delegate),
        typeof (PMEngine.BaseSystemConfigData),
        typeof (Audio.AudioConfig.Base.SoundConfigItemBase<>),
        typeof (PMEngine.BaseConfig),
        typeof (ScriptableObject),
    };

    private readonly static IEnumerable<string> HotfixClassNameFilter = new List<string>
    {
        "Apollo",
        "CardBoard",
        "QGamekit",
        "Gamejoy",
        "msdk",
        "gsdk",
        "WNPlugins",
        "<",
        "+",
        "WNNetWork",
        //"PMCore",
        "EasyList",
        "AICmd",
        "StoryGame",
        "Login",
        //"WNGUI",
        //"WNFrontEndGame",
        "CSV",
        //"UI",
        "View",
        "Tutorial",
        "Vip",
        "AIWeapon",
        "AICommand",
        "AIProjectile",
        "AIAction",
        "AIReact",
        "PVEGame.PVEAI",
        "PVEGame.AI",
        "WNPVEGame."
    };
    private readonly static IEnumerable<string> HotfixNamespaceNameFilter = new List<string>
    {
        //"WNGamebase.WN"
    };

    private readonly static HashSet<Type> HotfixClassWhiteList = new HashSet<Type>
    {
        typeof(WNGUI.UIWindowController),
        typeof(WNGUI.UIBuilder),
        typeof(PMEngine.UIScene),
        typeof(SceneTilePartition.Deserialization.SceneTileBasedDeserializer),
        typeof(WNFrontEndGame.RootNavigationUIController),
        typeof(WNIndivdualGame.AvatarViewSwitchBtnController),
        typeof(PMEngine.JobScheduler),
        typeof(PMEngine.NativeJobSchedulerProxy),
        typeof(WNIndivdualGame.EquipmentSlotManager),
        typeof(WNIndivdualGame.EquipmentBagController),
        typeof(WNIndivdualGame.EquipmentBagManager),
        typeof(WNNetWork.GlobalServerTimeSync),
        typeof(WNSurvivalTutorialGame.SurvivalTutorialGameUIScene),
        typeof(WNSurvivalTutorialGame.SurvivalTutorialGamePlayerPawn),
        typeof(WNSurvivalTutorialGame.SurvivalTutorialGame),
        typeof(WNSurvivalTutorialGame.STStep),
        typeof(WNSurvivalTutorialGame.STSimNetwork),
    };

    private readonly static string HotfixClassNamespacePrefix = "WN";

    private static bool ExportHotfixClass(Type type)
    {
        if (HotfixClassWhiteList.Contains(type))
        {
            return true;
        }

        if (!type.IsEnum && type.Namespace != null && type.Namespace.StartsWith(HotfixClassNamespacePrefix) && !type.IsGenericType && type.IsClass)
        {
            foreach (string s in HotfixClassNameFilter)
            {
                if (type.FullName.ToLower().Contains(s.ToLower()))
                {
                    return false;
                }
            }
            foreach (string s in HotfixNamespaceNameFilter)
            {
                if (!string.IsNullOrEmpty(type.Namespace) && type.Namespace.ToLower().Contains(s.ToLower()))
                {
                    return false;
                }
            }

            foreach (Type baseClass in HotfixBaseClassFilter)
            {
                if (type.IsSubclassOf(baseClass))
                {
                    return false;
                }
            }
            return true;
        }
        return false;
    }


    [Hotfix(HotfixFlag.IgnoreProperty | HotfixFlag.ValueTypeBoxing | HotfixFlag.IntKey)]
    public static IEnumerable<Type> HotfixWhiteList
    {
        get
        {
            Assembly assembly = typeof(XLuaAdapter).Assembly;
            foreach (Type type in assembly.GetTypes())
            {
                if (ExportHotfixClass(type))
                {
                    yield return type;
                }
            }
        }
    }

#if UNITY_EDITOR
    [UnityEditor.MenuItem("XLua/Export Hotfix Class List")]
    public static void ExportHotfixClassList()
    {
        using (StreamWriter sw = new StreamWriter(Application.dataPath + "/../HotfixClassList.txt"))
        {
            foreach (Type type in HotfixWhiteList)
            {
                sw.WriteLine(type.FullName);
            }
        }
    }
#endif
}

#endif