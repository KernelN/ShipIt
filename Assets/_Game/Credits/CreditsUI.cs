using TMPro;
using UnityEngine;

namespace ShipIt
{
    public class CreditsUI : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI creditsLabel;
        [SerializeField] ShopManager shopManager;

        void Start()
        {
            if (shopManager)
                shopManager.OnCreditsChanged += HandleCreditsChanged;
            else
                GameManager.inst.DataLoaded += RefreshCredits;

            RefreshCredits();
        }
        void OnDestroy()
        {
            if (shopManager)
                shopManager.OnCreditsChanged -= HandleCreditsChanged;
            else if (GameManager.inst.DataLoaded != null)
                GameManager.inst.DataLoaded -= RefreshCredits;
        }

        void HandleCreditsChanged(int _)
        {
            RefreshCredits();
        }

        void RefreshCredits()
        {
            GameManager manager = GameManager.inst;

            if (!creditsLabel || !manager || manager.Data == null) return;

            creditsLabel.text = manager.Data.credits.ToString("$0");
        }
    }
}
