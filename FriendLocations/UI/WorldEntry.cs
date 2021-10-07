using System;
using System.Collections.Generic;
using MelonLoader;
using UnityEngine;
using UnityEngine.UI;
using VRC.Core;
using VRChatUtilityKit.Ui;

using Object = UnityEngine.Object;

using FriendLocations.Core;
using FriendLocations.Utils;
using FriendLocations.Config;

namespace FriendLocations.UI
{
    public class WorldEntry
    {
        public GameObject GameObject;
        public Dictionary<string, PlayerEntry> PlayerList;

        public WorldListManager.WorldInstance WorldInstance;
        private GameObject playerListObject;
        private GameObject playerTemplate;
        private Button worldButton;

        public WorldEntry(WorldListManager.WorldInstance worldInstance)
        {
            WorldInstance = worldInstance;
            GameObject = Object.Instantiate(MenuManager.WorldTemplate, MenuManager.WorldTemplate.transform.parent);
            GameObject.name = worldInstance.WorldId + ":" + worldInstance.InstanceId;
            GameObject.SetActive(true);
            playerListObject = GameObject.transform.Find("PlayerList/Layout").gameObject;
            playerTemplate = playerListObject.transform.Find("Template").gameObject;
            PlayerList = new Dictionary<string, PlayerEntry>();

            Text worldName = GameObject.transform.Find("WorldName").GetComponent<Text>();
            RawImage worldImage = GameObject.transform.Find("WorldImage").GetComponent<RawImage>();
            worldButton = worldImage.transform.GetComponent<Button>();

            GameObject instanceInformation = GameObject.transform.Find("WorldInformation").gameObject;
            Text worldType = instanceInformation.transform.Find("InstanceType").GetComponent<Text>();
            Text regionName = instanceInformation.transform.Find("InstanceRegion/RegionName").GetComponent<Text>();
            RawImage regionIcon = instanceInformation.transform.Find("InstanceRegion/RegionIcon").GetComponent<RawImage>();

            GameObject worldStateInfo = GameObject.transform.Find("StateInfo").gameObject;
            worldStateInfo.SetActive(false);

            instanceInformation.SetActive(false);

            ApiWorld apiWorld = APIUtils.FetchAPIWorld(WorldInstance.WorldId);
            if (apiWorld == null)
            {
                worldName.text = "Fetching...";
                APIUtils.QueueFetch(WorldInstance.WorldId, typeof(ApiWorld), new Action<ApiModel>(apiModel =>
                {
                    if (worldName != null)
                    {
                        ApiWorld apiWorld = apiModel.Cast<ApiWorld>();
                        worldName.text = apiWorld.name + " #" + WorldInstance.InstanceId;
                        if (Configuration.ShowInstanceInformation.Value)
                        {
                            worldType.text = WorldInstance.InstanceType == InstanceAccessType.Public ? "Public" : WorldInstance.InstanceType == InstanceAccessType.FriendsOfGuests ? "Friends+" : WorldInstance.InstanceType == InstanceAccessType.FriendsOnly ? "Friends" : "Error";
                            regionName.text = WorldInstance.InstanceRegion == NetworkRegion.US ? "US" : WorldInstance.InstanceRegion == NetworkRegion.Europe ? "EU" : "JP";
                            regionIcon.texture = WorldInstance.InstanceRegion == NetworkRegion.US ? MenuManager.RegionUSTexture : WorldInstance.InstanceRegion == NetworkRegion.Europe ? MenuManager.RegionEUTexture : MenuManager.RegionJPTexture;
                            if (!apiWorld.IsPublicPublishedWorld)
                            {
                                worldStateInfo.SetActive(true);
                            }
                            instanceInformation.SetActive(true);
                        }
                        worldButton.onClick.AddListener(new Action(() => VRCUtils.OpenWorldInWorldInfoPage(apiWorld, WorldInstance)));
                        MelonCoroutines.Start(APIUtils.LoadImage(apiWorld.imageUrl, worldImage));
                    }
                }));
            }
            else
            {
                worldName.text = apiWorld.name + " #" + WorldInstance.InstanceId;
                if (Configuration.ShowInstanceInformation.Value)
                {
                    worldType.text = WorldInstance.InstanceType == InstanceAccessType.Public ? "Public" : WorldInstance.InstanceType == InstanceAccessType.FriendsOfGuests ? "Friends+" : WorldInstance.InstanceType == InstanceAccessType.FriendsOnly ? "Friends" : "Error";
                    regionName.text = WorldInstance.InstanceRegion == NetworkRegion.US ? "US" : WorldInstance.InstanceRegion == NetworkRegion.Europe ? "EU" : "JP";
                    regionIcon.texture = WorldInstance.InstanceRegion == NetworkRegion.US ? MenuManager.RegionUSTexture : WorldInstance.InstanceRegion == NetworkRegion.Europe ? MenuManager.RegionEUTexture : MenuManager.RegionJPTexture;
                    if (!apiWorld.IsPublicPublishedWorld)
                    {
                        worldStateInfo.SetActive(true);
                    }
                    instanceInformation.SetActive(true);
                }
                worldImage.transform.GetComponent<Button>().onClick.AddListener(new Action(() => VRCUtils.OpenWorldInWorldInfoPage(apiWorld, WorldInstance)));
                MelonCoroutines.Start(APIUtils.LoadImage(apiWorld.imageUrl, worldImage));
            }
            Update();
        }

        public void Update()
        {
            List<string> players = WorldListManager.WorldList.TryGetValue(WorldInstance, out List<string> list) ? list : new List<string>();
            Dictionary<string, PlayerEntry> newList = new Dictionary<string, PlayerEntry>();

            bool recalculateSize = ((PlayerList.Count <= 8 && players.Count > 8) || (PlayerList.Count > 8 && players.Count <= 8)) && Configuration.DynamicListScale.Value;

            if (recalculateSize)
            {
                Vector2 newSize;
                Vector3 newPosition;
                int fontSize;
                if (players.Count > 8)
                {
                    newSize = new Vector2(280, 35);
                    newPosition = new Vector3(140, -17.5f);
                    fontSize = 25;
                }
                else
                {
                    newSize = new Vector2(400, 50);
                    newPosition = new Vector3(200, -25);
                    fontSize = 32;
                }

                playerListObject.GetComponent<GridLayoutGroup>().cellSize = newSize;
                RectTransform playerName = playerTemplate.transform.Find("PlayerName").GetComponent<RectTransform>();
                playerName.sizeDelta = newSize;
                playerName.anchoredPosition = newPosition;
                playerTemplate.transform.Find("PlayerOutline").GetComponent<RectTransform>().sizeDelta = newSize;

                Text playerNameText = playerTemplate.transform.Find("PlayerName").GetComponent<Text>();
                playerNameText.fontSize = fontSize;
                playerNameText.resizeTextMaxSize = fontSize;
            }

            foreach (KeyValuePair<string, PlayerEntry> user in PlayerList)
            {
                if (players.Contains(user.Key) && !recalculateSize)
                {
                    newList.Add(user.Key, user.Value);
                    players.Remove(user.Key);
                }
                else
                {
                    Object.Destroy(user.Value.GameObject);
                }
            }

            foreach (string userId in players)
            {
                GameObject playerInstance = Object.Instantiate(playerTemplate, playerListObject.transform);
                playerInstance.name = userId;
                playerInstance.SetActive(true);

                Button playerButton = playerInstance.transform.GetComponent<Button>();
                Text playerName = playerInstance.transform.Find("PlayerName").GetComponent<Text>();

                APIUser apiUser = APIUtils.FetchAPIUser(userId);
                if (apiUser == null)
                {
                    playerName.text = "Fetching...";
                    APIUtils.QueueFetch(userId, typeof(APIUser), new Action<ApiModel>(apiModel =>
                    {
                        if (playerName != null)
                        {
                            APIUser apiUser = apiModel.Cast<APIUser>();
                            playerName.text = apiUser.displayName;
                            playerName.color = VRCUtils.GetRankColor(VRCPlayer.Method_Private_Static_String_APIUser_0(apiUser));
                            playerButton.onClick.AddListener(new Action(() => UiManager.OpenUserInUserInfoPage(apiUser, false)));
                        }
                    }));
                }
                else
                {
                    playerName.text = apiUser.displayName;
                    playerName.color = VRCUtils.GetRankColor(VRCPlayer.Method_Private_Static_String_APIUser_0(apiUser));
                    playerButton.onClick.AddListener(new Action(() => UiManager.OpenUserInUserInfoPage(apiUser, false)));
                }
                newList.Add(userId, new PlayerEntry() { GameObject = playerInstance, Button = playerButton });
            }

            PlayerList = newList;
        }

        public void FixColors()
        {

            // Fix for weird button coloring after UI color change like from emmVRC

            worldButton.colors = ColorBlock.defaultColorBlock;
            foreach (PlayerEntry playerEntry in PlayerList.Values)
            {
                playerEntry.Button.colors = ColorBlock.defaultColorBlock;
            }
        }

        public void Destroy()
        {
            Object.Destroy(GameObject);
        }

        public struct PlayerEntry
        {
            public GameObject GameObject;
            public Button Button;
        }
    }
}
