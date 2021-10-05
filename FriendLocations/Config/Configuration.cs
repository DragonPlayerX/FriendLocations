using System;
using MelonLoader;

namespace FriendLocations.Config
{
    public static class Configuration
    {

        private static readonly MelonPreferences_Category Category = MelonPreferences.CreateCategory("FriendLocations", "Friend Locations");

        public static MelonPreferences_Entry<bool> ShowAPIInfo;

        public static bool HasChanged;

        public static void Init()
        {
            ShowAPIInfo = CreateEntry("ShowAPIInfo", true, "Show API Info");
        }

        public static void Save()
        {
            if (RoomManager.field_Internal_Static_ApiWorldInstance_0 == null) return;
            if (HasChanged)
            {
                MelonPreferences.Save();
                HasChanged = false;
            }
        }

        private static MelonPreferences_Entry<T> CreateEntry<T>(string name, T defaultValue, string displayname, string description = null)
        {
            MelonPreferences_Entry<T> entry = Category.CreateEntry<T>(name, defaultValue, displayname, description);
            entry.OnValueChangedUntyped += new Action(() => HasChanged = true);
            return entry;
        }
    }
}
