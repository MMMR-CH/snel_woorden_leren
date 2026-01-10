using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SWL
{
    public class ProgressBar : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI progressText;
        [SerializeField] Image fillerImage;

        public void UpdateBar(float current, float max, bool showDesc = true)
        {
            progressText.text = showDesc ? $"{current}/{max}" : string.Empty;
            fillerImage.fillAmount = max > 0 ? current / max : 0f;
        }
    }
}
