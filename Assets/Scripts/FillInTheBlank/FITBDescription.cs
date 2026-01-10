using TMPro;
using UnityEngine;

namespace SWL
{
    public class FITBDescription : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI descriptionText;

        float tempSize = 1.0f;


        public void Init(FITBMainBus fitbMainBus)
        {
            tempSize = descriptionText.fontSize;
            fitbMainBus.AddDescriptionText = AddDescriptionText;
            fitbMainBus.ClearDescriptionText = ClearDescriptionText;
            fitbMainBus.AddSampleSentence = AddSampleSentence;
            fitbMainBus.ClearDescriptionText();
        }

        public void AddDescriptionText(string text, float sizePercentage = 1f)
        {
            if (string.IsNullOrEmpty(text))
            {
                SWL_Debug.LogWarning("Attempted to add empty description text.");
                return;
            }
            //SWL_Debug.Log($"BetekenisLevelDescription => Adding description text: {text}");
            if (descriptionText != null)
            {
                if (sizePercentage != 1f) text = $"<size={tempSize * sizePercentage}>{text}</size>";
                if (string.IsNullOrEmpty(descriptionText.text)) descriptionText.text = text; 
                else descriptionText.text += "\n" + text;
            }
            else
            {
                Debug.LogError("Description text is not assigned in the BetekenisLevelCanvas.");
            }
        }

        public void ClearDescriptionText()
        {
            descriptionText.text = string.Empty;
            //SWL_Debug.Log("BetekenisLevelDescription => Cleared description text.");
        }

        public void AddSampleSentence(int sampleIdx, string sampleText)
        {
            string textClr = sampleIdx switch
            {
                1 => "yellow",
                2 => "orange",
                3 => "green",
                _ => "white"
            };
            string text = $"<color={textClr}>{sampleIdx}. {sampleText}</color>";
            AddDescriptionText(text, 0.6f);
        }
    }
}
