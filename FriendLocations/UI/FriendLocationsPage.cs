using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

using FriendLocations.Core;
using FriendLocations.Utils;
using FriendLocations.Config;

namespace FriendLocations.UI
{
    public class FriendLocationsPage : VRCUiPage
    {

        public Text ApiRequestInfoText;
        public Text ApiObjectInfoText;
        private Text onlineText;
        private Text instanceText;
        private Text privateText;
        private Dictionary<WorldListManager.WorldInstance, WorldEntry> worlds;

        public bool NeedsReload;

        public FriendLocationsPage(IntPtr value) : base(value)
        {

        }

        public void Prepare()
        {
            ApiRequestInfoText = transform.Find("ApiInfo/ApiRequests").GetComponent<Text>();
            ApiObjectInfoText = transform.Find("ApiInfo/ApiObjectCount").GetComponent<Text>();
            onlineText = transform.Find("Info/OnlineInfo").GetComponent<Text>();
            instanceText = transform.Find("Info/InstanceInfo").GetComponent<Text>();
            privateText = transform.Find("Info/PrivateInfo").GetComponent<Text>();
            ApiRequestInfoText.font = MenuManager.NormalFont;
            ApiObjectInfoText.font = MenuManager.NormalFont;
            onlineText.font = MenuManager.NormalFont;
            instanceText.font = MenuManager.NormalFont;
            privateText.font = MenuManager.NormalFont;
            worlds = new Dictionary<WorldListManager.WorldInstance, WorldEntry>();

            APIUtils.OnInfoUpdate += new Action(() =>
            {
                ApiRequestInfoText.text = APIUtils.GetCurrentQueueCount() + " Pending\nAPI Requests";
                ApiObjectInfoText.text = APIUtils.GetCacheSize() + " Objects\nin Cache";
            });

            if (!Configuration.ShowAPIInfo.Value)
            {
                ApiRequestInfoText.gameObject.SetActive(false);
                ApiObjectInfoText.gameObject.SetActive(false);
            }
        }

        public void UpdatePage()
        {
            Dictionary<WorldListManager.WorldInstance, List<string>> newWorldList = new Dictionary<WorldListManager.WorldInstance, List<string>>(WorldListManager.WorldList);
            Dictionary<WorldListManager.WorldInstance, WorldEntry> newList = new Dictionary<WorldListManager.WorldInstance, WorldEntry>();

            foreach (KeyValuePair<WorldListManager.WorldInstance, WorldEntry> existingWorld in worlds)
            {
                if (newWorldList.ContainsKey(existingWorld.Key) && !NeedsReload)
                {
                    newList.Add(existingWorld.Key, existingWorld.Value);
                    existingWorld.Value.Update();
                    newWorldList.Remove(existingWorld.Key);
                }
                else
                {
                    existingWorld.Value.Destroy();
                }
            }

            foreach (KeyValuePair<WorldListManager.WorldInstance, List<string>> world in newWorldList)
            {
                newList.Add(world.Key, new WorldEntry(world.Key));
            }

            worlds = newList.OrderBy(entry => entry.Value.PlayerList.Count).ToDictionary(entry => entry.Key, entry => entry.Value); ;

            for (int i = 0; i < worlds.Count; i++)
            {
                WorldEntry world = worlds.Values.ToArray()[i];
                if (world.GameObject != null)
                {
                    world.FixColors();
                    world.GameObject.transform.SetAsFirstSibling();
                }
            }

            onlineText.text = "Online: " + WorldListManager.OnlineCount + "/" + WorldListManager.FriendCount;
            instanceText.text = "Instances: " + WorldListManager.WorldList.Count;
            privateText.text = "Private: " + WorldListManager.PrivateCount;

            NeedsReload = false;
        }
    }
}
