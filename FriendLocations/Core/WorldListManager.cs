using System.Collections.Generic;
using VRC.Core;
using VRC.UI;

namespace FriendLocations.Core
{
    public class WorldListManager
    {

        public static Dictionary<WorldInstance, List<string>> WorldList = new Dictionary<WorldInstance, List<string>>();
        public static int FriendCount;
        public static int OnlineCount;
        public static int PrivateCount;

        public static void FetchLists()
        {
            WorldList.Clear();
            FriendCount = 0;
            OnlineCount = 0;
            PrivateCount = 0;
            FriendsListManager friendsListManager = FriendsListManager.field_Private_Static_FriendsListManager_0;
            foreach (Il2CppSystem.Collections.Generic.KeyValuePair<string, APIUser> entry in friendsListManager.field_Private_Dictionary_2_String_APIUser_0)
            {
                APIUser apiUser = entry.Value;
                string location = apiUser.location;
                if (location.Equals("private"))
                {
                    PrivateCount++;
                }
                else if (location.Contains("~friends") || location.Contains("~hidden") || location.StartsWith("wrld_"))
                {
                    WorldInstance worldInstance = new WorldInstance() { Location = location, WorldId = location.Substring(0, location.IndexOf(":")), InstanceId = location.Substring(location.IndexOf(":") + 1, 5) };    
                    if (WorldList.TryGetValue(worldInstance, out List<string> list))
                        list.Add(apiUser.id);
                    else
                        WorldList.Add(worldInstance, new List<string>() { apiUser.id });
                }
                else
                {
                    PrivateCount++;
                }
            }
            FriendCount = friendsListManager.field_Protected_Dictionary_2_String_APIUser_0.Count;
            OnlineCount = friendsListManager.field_Private_Dictionary_2_String_APIUser_0.Count;
        }

        public struct WorldInstance
        {
            public string Location;
            public string WorldId;
            public string InstanceId;
        }
    }
}
