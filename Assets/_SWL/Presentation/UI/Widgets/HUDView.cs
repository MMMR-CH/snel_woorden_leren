using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SWL.Presentation.UI.Widgets
{
    public sealed class HUDView : MonoBehaviour
    {
        [Header("Optional")]
        [SerializeField] private Image avatarImage;

        [Header("Texts")]
        [SerializeField] private TMP_Text lifeText;
        [SerializeField] private TMP_Text lifeTimerText;
        [SerializeField] private TMP_Text coinText;
        [SerializeField] private TMP_Text gemText;

        private void Awake()
        {
            if (lifeText == null) Debug.LogError("HUDView: lifeText is not set", this);
            if (coinText == null) Debug.LogError("HUDView: coinText is not set", this);
            if (gemText == null) Debug.LogError("HUDView: gemText is not set", this);
        }

        public void SetLife(int life) => lifeText.text = life.ToString();
        public void SetCoins(int coins) => coinText.text = coins.ToString();
        public void SetGems(int gems) => gemText.text = gems.ToString();        

        public void SetLifeTimer(string text)
        {
            if (lifeTimerText != null)
                lifeTimerText.text = text;
        }
    }
}
