using System.Collections;
using MelonLoader;
using UnhollowerRuntimeLib;

using FriendLocations;
using FriendLocations.Core;
using FriendLocations.UI;
using FriendLocations.Utils;
using FriendLocations.Config;

[assembly: MelonInfo(typeof(FriendLocationsMod), "FriendLocations", "1.0.0", "DragonPlayer", "https://github.com/DragonPlayerX/FriendLocations")]
[assembly: MelonGame("VRChat", "VRChat")]

namespace FriendLocations
{
    public class FriendLocationsMod : MelonMod
    {
        public static readonly string Version = "1.0.0";

        public static FriendLocationsMod Instance { get; private set; }

        public override void OnApplicationStart()
        {
            Instance = this;
            MelonLogger.Msg("Initializing FriendLocations " + Version + "...");

            Configuration.Init();

            ClassInjector.RegisterTypeInIl2Cpp<FriendLocationsPage>();
            MelonCoroutines.Start(Init());
        }

        private IEnumerator Init()
        {
            while (VRCUiManager.field_Private_Static_VRCUiManager_0 == null) yield return null;

            MenuManager.PrepareAssets();
            MenuManager.Init();
            TemplateManager.PrepareTemplates();

            APIUtils.Init();

            MelonLogger.Msg("Running version " + Version + " of FriendLocations.");
        }
    }
}
