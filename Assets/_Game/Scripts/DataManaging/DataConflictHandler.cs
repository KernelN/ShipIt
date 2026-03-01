using Unity.Services.CloudSave.Models;
using UnityEngine;
using Universal.FileManaging.Cloud;

namespace ShipIt.CloudData
{
    public class DataConflictHandler : MonoBehaviour
    {
        [SerializeField] GameDataShower localShower;
        [SerializeField] GameDataShower cloudShower;
        [SerializeField] GameObject dataConflictPanel;
        CloudDataManager dataManager;
        
        public void Start()
        {
            dataManager = CloudDataManager.inst;
            if(!dataManager) return;
            if(!dataManager.hasDataConflict) return;
            
            dataConflictPanel.SetActive(true);
            localShower?.Show(GameManager.inst.Data);
            Item cloudData = dataManager.GetData(CloudDataManager.Key.GameData);
            string dataString = cloudData.Value.GetAs<string>();
            cloudShower?.Show(JsonUtility.FromJson<GameData>(dataString));
        }
        public void KeepLocal() => dataManager.ClearSaveData();
        public void KeepCloud() => dataManager.KeepSaveData();
    }
}
