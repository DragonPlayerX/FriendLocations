using System;
using UnityEngine;
using UnityEngine.UI;

using FriendLocations.Config;

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

            MenuManager.WorldTemplate.transform.Find("WorldInformation").gameObject.SetActive(Configuration.ShowInstanceInformation.Value);
            Configuration.ShowInstanceInformation.OnValueChanged += new Action<bool, bool>((oldValue, newValue) =>
            {
                MenuManager.WorldTemplate.transform.Find("WorldInformation").gameObject.SetActive(newValue);
                MenuManager.RequireUpdate();
            });

            MenuManager.PlayerTemplate.transform.Find("PlayerOutline").gameObject.SetActive(Configuration.ShowPlayerOutlines.Value);
            Configuration.ShowPlayerOutlines.OnValueChanged += new Action<bool, bool>((oldValue, newValue) =>
            {
                MenuManager.PlayerTemplate.transform.Find("PlayerOutline").gameObject.SetActive(newValue);
                MenuManager.RequireUpdate();
            });
        }

        private static void ApplyUseUIOutlineColor(bool value)
        {
            Color color = GameObject.Find("UserInterface/MenuContent/Screens/WorldInfo/Back Button").GetComponent<Button>().colors.normalColor;
            Image worldListOutline = MenuManager.FriendLocationsUI.transform.Find("WorldListOutline").GetComponent<Image>();
            Image worldImageOutline = MenuManager.WorldTemplate.transform.Find("WorldImageOutline").GetComponent<Image>();
            Image playerOutline = MenuManager.PlayerTemplate.transform.Find("PlayerOutline").GetComponent<Image>();
            if (value)
            {
                worldListOutline.color = color;
                worldImageOutline.color = color;
                playerOutline.color = color;
            }
            else
            {
                worldListOutline.color = Color.white;
                worldImageOutline.color = Color.white;
                playerOutline.color = Color.white;
            }
        }
    }
}
