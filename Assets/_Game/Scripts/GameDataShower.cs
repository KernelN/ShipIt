using TMPro;
using UnityEngine;

namespace ShipIt
{
    public class GameDataShower : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI text;
        
        public void Show(GameData data)
        {
            string dataString;
            dataString = "Credits: " + data.credits;
            dataString += "\nFuel: " + data.fuel;
            dataString += "\nHighest Level Completed: " + data.highestLevelCompleted;
            dataString += "\nSkins Bought: " + data.items.Count;
            text.text = dataString;
        }
    }
}
