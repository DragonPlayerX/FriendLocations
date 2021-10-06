using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MelonLoader;
using UnhollowerRuntimeLib;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using VRC.Core;

namespace FriendLocations.Utils
{
    public class APIUtils
    {

        private static readonly System.Random Random = new System.Random();
        private static readonly LinkedList<QueueData> ObjectsToFetch = new LinkedList<QueueData>();
        private static readonly Dictionary<string, ApiModel> PersistentObjectCache = new Dictionary<string, ApiModel>();

        public static event Action OnInfoUpdate;

        public static void Init()
        {
            MelonCoroutines.Start(FetchingQueue());
        }

        private static IEnumerator FetchingQueue()
        {
            while (true)
            {
                yield return new WaitForSeconds(1);
                while (ObjectsToFetch.Count > 0)
                {
                    QueueData queueData = ObjectsToFetch.First();
                    ObjectsToFetch.RemoveFirst();
                    if (queueData.Type == typeof(APIUser))
                    {
                        API.Fetch<APIUser>(queueData.Id, new Action<ApiContainer>(container =>
                        {
                            if (!PersistentObjectCache.ContainsKey(queueData.Id))
                                PersistentObjectCache.Add(queueData.Id, container.Model);

                            ApiCache.Save(queueData.Id, container.Model.Cast<ApiCacheObject>());
                            queueData.Completed?.Invoke(container.Model);
                            OnInfoUpdate?.Invoke();
                        }), new Action<ApiContainer>(container =>
                        {
                            MelonLogger.Error("Could not fetch ApiUser with id " + queueData.Id);
                        }));
                    }
                    else if (queueData.Type == typeof(ApiWorld))
                    {
                        API.Fetch<ApiWorld>(queueData.Id, new Action<ApiContainer>(container =>
                        {
                            if (!PersistentObjectCache.ContainsKey(queueData.Id))
                                PersistentObjectCache.Add(queueData.Id, container.Model);

                            ApiCache.Save(queueData.Id, container.Model.Cast<ApiCacheObject>());
                            queueData.Completed?.Invoke(container.Model);
                            OnInfoUpdate?.Invoke();
                        }), new Action<ApiContainer>(container =>
                        {
                            MelonLogger.Error("Could not fetch ApiWorld with id " + queueData.Id);
                        }));
                    }
                    else if (queueData.Type == typeof(ApiWorldInstance))
                    {
                        API.Fetch<ApiWorldInstance>(queueData.Id, new Action<ApiContainer>(container =>
                        {
                            ApiCache.Save(queueData.Id, container.Model.Cast<ApiCacheObject>());
                            queueData.Completed?.Invoke(container.Model);
                            OnInfoUpdate?.Invoke();
                        }), new Action<ApiContainer>(container =>
                        {
                            MelonLogger.Error("Could not fetch ApiWorldInstance with id " + queueData.Id);
                        }));
                    }
                    OnInfoUpdate?.Invoke();
                    yield return new WaitForSeconds(Random.Next(2, 5));
                }
            }
        }

        public static APIUser FetchAPIUser(string id)
        {
            APIUser apiUser = FetchInternalAPIUser(id);
            if (apiUser != null)
            {
                if (PersistentObjectCache.ContainsKey(id))
                    PersistentObjectCache[id] = apiUser;
                else
                    PersistentObjectCache.Add(id, apiUser);
                OnInfoUpdate?.Invoke();
                return apiUser;
            }

            return PersistentObjectCache.TryGetValue(id, out ApiModel cachedObject) ? cachedObject.Cast<APIUser>() : null;
        }

        public static ApiWorld FetchAPIWorld(string id)
        {
            ApiWorld apiWorld = FetchInternalAPIWorld(id);
            if (apiWorld != null)
            {
                if (PersistentObjectCache.ContainsKey(id))
                    PersistentObjectCache[id] = apiWorld;
                else
                    PersistentObjectCache.Add(id, apiWorld);
                OnInfoUpdate?.Invoke();
                return apiWorld;
            }

            return PersistentObjectCache.TryGetValue(id, out ApiModel cachedObject) ? cachedObject.Cast<ApiWorld>() : null;
        }

        public static void FetchAPIWorldInstance(string id, Action<ApiModel> completed)
        {
            ApiWorldInstance apiWorldInstance = FetchInternalAPIWorldInstance(id);
            if (apiWorldInstance != null)
            {
                completed.Invoke(apiWorldInstance);
                return;
            }
            QueueFetch(id, typeof(ApiWorldInstance), completed, true);
        }

        private static APIUser FetchInternalAPIUser(string id)
        {
            if (!ApiCache.Contains<APIUser>(id))
                return null;

            ApiCache.CacheEntry cacheEntry = ApiCache.cache[Il2CppType.Of<APIUser>()][id];
            if (cacheEntry.obj == null)
                return null;

            return cacheEntry.obj.Cast<APIUser>();
        }

        private static ApiWorld FetchInternalAPIWorld(string id)
        {
            if (!ApiCache.Contains<ApiWorld>(id))
                return null;

            ApiCache.CacheEntry cacheEntry = ApiCache.cache[Il2CppType.Of<ApiWorld>()][id];
            if (cacheEntry.obj == null)
                return null;

            return cacheEntry.obj.Cast<ApiWorld>();
        }

        private static ApiWorldInstance FetchInternalAPIWorldInstance(string id)
        {
            if (!ApiCache.Contains<ApiWorldInstance>(id))
                return null;

            ApiCache.CacheEntry cacheEntry = ApiCache.cache[Il2CppType.Of<ApiWorldInstance>()][id];
            if (cacheEntry.obj == null)
                return null;

            return cacheEntry.obj.Cast<ApiWorldInstance>();
        }

        public static void QueueFetch(string id, Type type, Action<ApiModel> completed, bool priority = false)
        {
            if (priority)
                ObjectsToFetch.AddFirst(new QueueData() { Id = id, Type = type, Completed = completed });
            else
                ObjectsToFetch.AddLast(new QueueData() { Id = id, Type = type, Completed = completed });
            OnInfoUpdate?.Invoke();
        }

        public static IEnumerator LoadImage(string url, RawImage rawImage)
        {
            UnityWebRequest webRequest = UnityWebRequestTexture.GetTexture(url, true);
            UnityWebRequestAsyncOperation asyncOperation = webRequest.BeginWebRequest();
            while (!asyncOperation.isDone)
                yield return null;
            if (!webRequest.isNetworkError && !webRequest.isHttpError)
                rawImage.texture = DownloadHandlerTexture.GetContent(webRequest);
            else
                MelonLogger.Warning("Failed to load image: " + url);
        }

        public static int GetCurrentQueueCount()
        {
            return ObjectsToFetch.Count;
        }

        public static int GetCacheSize()
        {
            return PersistentObjectCache.Count;
        }

        private struct QueueData
        {
            public string Id;
            public Type Type;
            public Action<ApiModel> Completed;
        }
    }
}
