using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.CloudSave;
using Unity.Services.CloudSave.Models;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.Events;

namespace Universal.FileManaging.Cloud
{
    public class CloudDataManager : Singleton<CloudDataManager>
    {
        internal override bool DoNotDestroyOnLoad => true;
        
        public enum Key { DeviceID, GameData, IdleData, _count }
        Dictionary<Key, object> data = new Dictionary<Key, object>();
        Dictionary<string, Item> loadedData;
        bool isInitialized = false;


        public bool hasDataConflict { get; private set; }
        public UnityEvent OnDataConflictFixed;
        public UnityEvent<Key, Item> OnDataLoaded;

        void Start()
        {
            data = new Dictionary<Key, object>();
            loadedData = new Dictionary<string, Item>();
            Universal.SocialNet.GameSocials.inst.SignedAndLinked += OnInitializedAndSignedIn;
        }
        void OnInitializedAndSignedIn()
        {
            isInitialized = true;
            LoadDataWithErrorHandling();
        }
        public void SaveKeyData(Key key, string data)
        {
            if(!this.data.TryAdd(key, data))
                this.data[key] = data;
        }
        
        public async Task SaveDataWithErrorHandling()
        {
            if(!isInitialized) return;
            
            try
            {
                var data = new Dictionary<string, object>();
                foreach (var item in this.data)
                    data.Add(item.Key.ToString(), item.Value);
                Debug.Log("Attempting to save data...");
                await CloudSaveService.Instance.Data.Player.SaveAsync(data);
                Debug.Log("Save data success!");
            }
            catch (ServicesInitializationException e)
            {
                // service not initialized
                Debug.LogError(e);
            }
            catch (CloudSaveValidationException e)
            {
                // validation error
                Debug.LogError(e);
            }
            catch (CloudSaveRateLimitedException e)
            {
                // rate limited
                Debug.LogError(e);
            }
            catch (CloudSaveException e)
            {
                Debug.LogError(e);
            }
        }

        async Task LoadDataWithErrorHandling()
        {
            if(!isInitialized) return;
            
            try
            {
                Debug.Log("Attempting to load data...");
                loadedData = await CloudSaveService.Instance.Data.Player.LoadAllAsync();
                Debug.Log("Load data success!");
                
                string deviceKey = nameof(Key.DeviceID);
                if (loadedData.TryGetValue(deviceKey, out var deviceIdItem))
                {
                    string deviceID = deviceIdItem.Value.GetAsString();
                    
                    
                    if (!PlayerPrefs.HasKey(deviceKey) ||
                        deviceID != PlayerPrefs.GetString(deviceKey))
                    {
                        if(!PlayerPrefs.HasKey(deviceKey))
                            Debug.LogWarning(deviceKey + " not found in PlayerPrefs.");
                        else
                        {
                            string dID = PlayerPrefs.GetString(deviceKey);
                            Debug.LogWarning(deviceKey + " in PlayerPrefs (" + dID +") does not match " +  deviceID);
                        }
                            
                        hasDataConflict = true;
                        return;
                    }
                }
                
                SaveDeviceKey();
                
                for (int i = 0; i < (int)Key._count; i++)
                    if(loadedData.TryGetValue(((Key)i).ToString(), out var value))
                        OnDataLoaded?.Invoke((Key)i, value);
            }
            catch (ServicesInitializationException e)
            {
                // service not initialized
                Debug.LogError(e);
            }
            catch (CloudSaveValidationException e)
            {
                // validation error
                Debug.LogError(e);
            }
            catch (CloudSaveRateLimitedException e)
            {
                // rate limited
                Debug.LogError(e);
            }
            catch (CloudSaveException e)
            {
                Debug.LogError(e);
            }
        }

        void SaveDeviceKey()
        {
            PlayerPrefs.SetString(nameof(Key.DeviceID), SystemInfo.deviceUniqueIdentifier);
            SaveKeyData(Key.DeviceID, SystemInfo.deviceUniqueIdentifier);
            Debug.Log(nameof(Key.DeviceID) + " "+ SystemInfo.deviceUniqueIdentifier +" saved to PlayerPrefs.");
        }

        public void ClearSaveData()
        {
            loadedData.Clear();
            CloudSaveService.Instance.Data.Player.DeleteAllAsync();
            hasDataConflict = false;
            SaveDeviceKey();
            OnDataConflictFixed?.Invoke();
        }

        public void KeepSaveData()
        {
            for (int i = 0; i < (int)Key._count; i++)
                if(loadedData.TryGetValue(((Key)i).ToString(), out var value))
                    OnDataLoaded?.Invoke((Key)i, value);
            hasDataConflict = false;
            SaveDeviceKey();
            OnDataConflictFixed?.Invoke();
        }

        public Item GetData(Key gameData) => loadedData.GetValueOrDefault(gameData.ToString());
    }
}