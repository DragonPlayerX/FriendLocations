using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections;
using MelonLoader;
using HarmonyLib;
using UnhollowerRuntimeLib;
using UnityEngine;
using UnityEngine.UI;
using VRC.Core;
using VRChatUtilityKit.Ui;
using VRChatUtilityKit.Components;

using Object = UnityEngine.Object;

using FriendLocations.UI;
using FriendLocations.Config;

namespace FriendLocations.Core
{
    public class MenuManager
    {

        public static Font NormalFont;
        public static Font BoldFont;
        public static GameObject WorldTemplate;
        public static GameObject PlayerTemplate;

        public static GameObject FriendLocationsUI;
        private static RectTransform friendLocationsUIRect;
        private static FriendLocationsPage pageComponent;

        public static Texture2D RegionUSTexture;
        public static Texture2D RegionEUTexture;
        public static Texture2D RegionJPTexture;

        public static void PrepareAssets()
        {
            Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("FriendLocations.Resources.friendlocations.assetbundle");
            MemoryStream memoryStream = new MemoryStream((int)stream.Length);

            stream.CopyTo(memoryStream);

            AssetBundle assetBundle = AssetBundle.LoadFromMemory_Internal(memoryStream.ToArray(), 0);
            assetBundle.hideFlags |= HideFlags.DontUnloadUnusedAsset;
            FriendLocationsUI = Object.Instantiate(assetBundle.LoadAsset_Internal("Assets/FriendLocationsUI.prefab", Il2CppType.Of<GameObject>()).Cast<GameObject>(), GameObject.Find("UserInterface/MenuContent/Screens").transform);
            FriendLocationsUI.name = "FriendLocationsPage";

            RegionUSTexture = assetBundle.LoadAsset("Assets/Sprites/Region_US.png", Il2CppType.Of<Sprite>()).Cast<Sprite>().texture;
            RegionUSTexture.hideFlags |= HideFlags.DontUnloadUnusedAsset;
            RegionEUTexture = assetBundle.LoadAsset("Assets/Sprites/Region_EU.png", Il2CppType.Of<Sprite>()).Cast<Sprite>().texture;
            RegionEUTexture.hideFlags |= HideFlags.DontUnloadUnusedAsset;
            RegionJPTexture = assetBundle.LoadAsset("Assets/Sprites/Region_JP.png", Il2CppType.Of<Sprite>()).Cast<Sprite>().texture;
            RegionJPTexture.hideFlags |= HideFlags.DontUnloadUnusedAsset;

            friendLocationsUIRect = FriendLocationsUI.GetComponent<RectTransform>();
            friendLocationsUIRect.pivot = new Vector2(0f, 0f);

            friendLocationsUIRect.Find("Version").GetComponent<Text>().text = "Version\n" + FriendLocationsMod.Version;

            FriendLocationsUI.SetActive(false);

            NormalFont = GameObject.Find("UserInterface/MenuContent/Screens/Settings/OtherOptionsPanel/TooltipsToggle/Label").GetComponent<Text>().font;
            BoldFont = GameObject.Find("UserInterface/MenuContent/Screens/Settings/OtherOptionsPanel/TitleText (1)").GetComponent<Text>().font;

            FriendLocationsUI.transform.Find("Title").GetComponent<Text>().font = BoldFont;

            WorldTemplate = friendLocationsUIRect.Find("WorldList/Layout/Template").gameObject;
            WorldTemplate.transform.Find("WorldName").GetComponent<Text>().font = NormalFont;
            WorldTemplate.transform.Find("WorldInformation/InstanceType").GetComponent<Text>().font = NormalFont;
            PlayerTemplate = WorldTemplate.transform.Find("PlayerList/Layout/Template").gameObject;
            PlayerTemplate.transform.Find("PlayerName").GetComponent<Text>().font = NormalFont;
        }

        public static void Init()
        {
            MelonLogger.Msg("Creating UI...");
            GameObject buttonParent = GameObject.Find("UserInterface/MenuContent/Screens/Social/UserProfileAndStatusSection");
            GameObject statusButton = buttonParent.transform.Find("Status/EditStatusButton").gameObject;
            GameObject flButton = Object.Instantiate(statusButton, buttonParent.transform);

            if (MelonHandler.Mods.Any(mod => mod.Info.Name.Equals("emmVRCLoader")))
                flButton.transform.localPosition = new Vector3(flButton.transform.localPosition.x + 980, flButton.transform.localPosition.y, flButton.transform.localPosition.z);
            else
                flButton.transform.localPosition = new Vector3(flButton.transform.localPosition.x + 1220, flButton.transform.localPosition.y, flButton.transform.localPosition.z);

            RectTransform flButtonRect = flButton.GetComponent<RectTransform>();
            flButtonRect.sizeDelta = new Vector2(flButtonRect.sizeDelta.x + 50, flButtonRect.sizeDelta.y);

            Text flButtonText = flButton.transform.Find("Text").GetComponent<Text>();
            flButtonText.rectTransform.sizeDelta = new Vector2(flButtonText.rectTransform.sizeDelta.x + 75, flButtonText.rectTransform.sizeDelta.y);
            flButtonText.text = "Friend Locations";
            flButtonText.color = Color.yellow;

            RectTransform flButtonImageRect = flButton.transform.Find("Image").GetComponent<RectTransform>();
            flButtonImageRect.sizeDelta = new Vector2(flButtonImageRect.sizeDelta.x + 75, flButtonImageRect.sizeDelta.y);

            flButton.SetActive(true);

            pageComponent = FriendLocationsUI.AddComponent<FriendLocationsPage>();
            pageComponent.field_Public_String_0 = "SCREEN";
            pageComponent.Prepare();

            Configuration.ShowAPIInfo.OnValueChanged += new Action<bool, bool>((oldValue, newValue) =>
            {
                pageComponent.ApiRequestInfoText.gameObject.SetActive(newValue);
                pageComponent.ApiObjectInfoText.gameObject.SetActive(newValue);
            });

            Button backButton = Object.Instantiate(GameObject.Find("UserInterface/MenuContent/Screens/UserInfo/BackButton"), FriendLocationsUI.transform).GetComponent<Button>();
            backButton.onClick.RemoveAllListeners();
            backButton.onClick.AddListener(new Action(() =>
            {
                UiManager.MainMenu(2, false, true, false);

                // Setting back UserInfo to the local player property to prevent showing the wrong user when clicking on "Edit Profile"
                QuickMenu.field_Private_Static_QuickMenu_0.field_Private_APIUser_0 = APIUser.CurrentUser;
            }));
            backButton.gameObject.SetActive(true);

            GameObject hiddenTabs = new GameObject("Hidden");
            hiddenTabs.transform.parent = GameObject.Find("UserInterface/MenuContent/Backdrop/Header").transform;

            GameObject friendLocationsTab = Object.Instantiate(GameObject.Find("UserInterface/MenuContent/Backdrop/Header/Tabs/ViewPort/Content/WorldsPageTab"), hiddenTabs.transform);
            friendLocationsTab.name = "FriendLocationsTab";
            VRCUiPageTab friendLocationsTabPage = friendLocationsTab.GetComponent<VRCUiPageTab>();
            friendLocationsTabPage.field_Public_String_1 = "UserInterface/MenuContent/Screens/FriendLocationsPage";

            Button button = flButton.transform.GetComponent<Button>();
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(new Action(() =>
            {
                WorldListManager.FetchLists();
                pageComponent.UpdatePage();
                MelonCoroutines.Start(WaitOneFrame(new Action(() => friendLocationsTabPage.ShowPage())));
            }));

            FriendLocationsMod.Instance.HarmonyInstance.Patch(typeof(VRCUiPageTab).GetMethod("ShowPage"), new HarmonyMethod(typeof(MenuManager).GetMethod(nameof(ShowPage), BindingFlags.NonPublic | BindingFlags.Static)));
        }

        private static void ShowPage()
        {
            pageComponent.field_Public_String_0 = "SCREEN";
        }

        public static void RequireUpdate()
        {
            pageComponent.NeedsReload = true;
        }

        private static IEnumerator WaitOneFrame(Action action)
        {
            yield return null;
            action.Invoke();
        }
    }
}
