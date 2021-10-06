using FriendLocations.Config;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace FriendLocations.Core
{
    public class TemplateManager
    {
        public static void PrepareTemplates()
        {
            ApplyUseUIOutlineColor(Configuration.UseUIOutlineColor.Value);
            Configuration.UseUIOutlineColor.OnValueChanged += new Action<bool, bool>((oldValue, newValue) =>
            {
                ApplyUseUIOutlineColor(newValue);
                MenuManager.RequireUpdate();
            });
        }

        private static void ApplyUseUIOutlineColor(bool value)
        {
            Color color = GameObject.Find("UserInterface/MenuContent/Screens/WorldInfo/Back Button").GetComponent<Button>().colors.normalColor;
            Image worldListOutline = MenuManager.FriendLocationsUI.transform.Find("WorldListBackground").GetComponent<Image>();
            Image worldImageOutline = MenuManager.WorldTemplate.transform.Find("WorldImageOutline").GetComponent<Image>();
            if (value)
            {
                worldListOutline.color = color;
                worldImageOutline.color = color;
            }
            else
            {
                worldListOutline.color = Color.white;
                worldImageOutline.color = Color.white;
            }
        }
    }
}
