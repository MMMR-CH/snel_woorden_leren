using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

namespace SWL
{
    public class LoadingSceneCanvas : MonoBehaviour
    {
        [SerializeField] Image loadingBarImage;
        [SerializeField] TextMeshProUGUI versionText;
        [SerializeField] TextMeshProUGUI loadingInfoText;

        private void Awake()
        {
            loadingBarImage.fillAmount = 0f;
        }

        public void SetLoadingInfoText(string text)
        {
            loadingInfoText.text = text;
        }

        public void SetLoadingBar(float progress)
        {
            loadingBarImage.fillAmount = progress;
        }

        public void SetVersionText(string version)
        {
            versionText.text = version;
        }
    }
}
