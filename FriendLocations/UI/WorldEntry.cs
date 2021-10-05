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
                        worldButton.onClick.AddListener(new Action(() => VRCUtils.OpenWorldInWorldInfoPage(apiWorld, WorldInstance)));
                        MelonCoroutines.Start(APIUtils.LoadImage(apiWorld.imageUrl, worldImage));
                    }
                }));
            }
            else
            {
                worldName.text = apiWorld.name + " #" + WorldInstance.InstanceId;
                worldImage.transform.GetComponent<Button>().onClick.AddListener(new Action(() => VRCUtils.OpenWorldInWorldInfoPage(apiWorld, WorldInstance)));
                MelonCoroutines.Start(APIUtils.LoadImage(apiWorld.imageUrl, worldImage));
            }
            Update();
        }

        public void Update()
        {
            List<string> players = WorldListManager.WorldList.TryGetValue(WorldInstance, out List<string> list) ? list : new List<string>();
            Dictionary<string, PlayerEntry> newList = new Dictionary<string, PlayerEntry>();

            foreach (KeyValuePair<string, PlayerEntry> user in PlayerList)
            {
                if (players.Contains(user.Key))
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

                Text playerName = playerInstance.transform.Find("PlayerName").GetComponent<Text>();
                Button playerButton = playerInstance.transform.GetComponent<Button>();

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
                            playerButton.onClick.AddListener(new Action(() => UiManager.OpenUserInUserInfoPage(apiUser, true)));
                        }
                    }));
                }
                else
                {
                    playerName.text = apiUser.displayName;
                    playerName.color = VRCUtils.GetRankColor(VRCPlayer.Method_Private_Static_String_APIUser_0(apiUser));
                    playerButton.onClick.AddListener(new Action(() => UiManager.OpenUserInUserInfoPage(apiUser, true)));
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
