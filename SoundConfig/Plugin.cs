using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;

namespace SoundConfig;

[BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    private bool _initialized;
    private static ConfigEntry<float> _boomboxVolume;
    private static ConfigEntry<float> _sprayVolume;
    
    private void Awake()
    {
        var logger = BepInEx.Logging.Logger.CreateLogSource(PluginInfo.PLUGIN_GUID);
        logger.LogInfo($"Loading {PluginInfo.PLUGIN_GUID}");

        _boomboxVolume = Config.Bind("General",
                                   "BoomboxVolume",
                                   .3f,
                                   "Default boombox volume. (0-1.0)");
        
        _sprayVolume = Config.Bind("General",
                                   "SprayVolume",
                                   .3f,
                                   "Default volume of spray paint sound effects. (0-1.0)");
        
        new Harmony(PluginInfo.PLUGIN_GUID).PatchAll();
    }

    private void Init()
    {
        if (_initialized)
            return;

        _initialized = true;
        
        LC_API.ServerAPI.ModdedServer.SetServerModdedOnly();
    }
    
    public void Start() => Init();

    public void OnDestroy() => Init();

    [HarmonyPatch]
    internal class Patches
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(SprayPaintItem), "Start")]
        public static void Start(SprayPaintItem __instance, ref AudioSource ___sprayAudio)
        {
            ___sprayAudio.volume = _sprayVolume.Value;
        }
        
        [HarmonyPostfix]
        [HarmonyPatch(typeof(BoomboxItem), "Start")]
        public static void Start(BoomboxItem __instance, ref AudioSource ___boomboxAudio)
        {
            ___boomboxAudio.volume = _boomboxVolume.Value;
        }
    }
}