using System;
using UnityEngine;
using VRC.Core;
using VRChatUtilityKit.Ui;

using FriendLocations.Core;

namespace FriendLocations.Utils
{
    public class VRCUtils
    {

        private static readonly Color VeteranColor = new Color(171f / 255, 205f / 255, 239f / 255);
        private static readonly Color LegendaryColor = new Color(255f / 255, 105f / 255, 180f / 255);

        public static void OpenWorldInWorldInfoPage(ApiWorld apiWorld, WorldListManager.WorldInstance worldInstance)
        {
            UiManager.MainMenu(0, false, true, false);
            APIUtils.FetchAPIWorldInstance(worldInstance.Location, new Action<ApiModel>(apiWorldInstance => UiWorldList.Method_Public_Static_Void_ApiWorld_ApiWorldInstance_Boolean_APIUser_0(apiWorld, apiWorldInstance.Cast<ApiWorldInstance>())));
        }

        public static Color GetRankColor(string rank)
        {
            switch (rank)
            {
                case "Visitor":
                    return VRCPlayer.field_Internal_Static_Color_2;
                case "New User":
                    return VRCPlayer.field_Internal_Static_Color_3;
                case "User":
                    return VRCPlayer.field_Internal_Static_Color_4;
                case "Known User":
                    return VRCPlayer.field_Internal_Static_Color_5;
                case "Trusted User":
                    return VRCPlayer.field_Internal_Static_Color_6;
                case "Veteran User":
                    return VeteranColor;
                case "Legendary User":
                    return LegendaryColor;
                default:
                    return VRCPlayer.field_Internal_Static_Color_2;
            }
        }
    }
}
