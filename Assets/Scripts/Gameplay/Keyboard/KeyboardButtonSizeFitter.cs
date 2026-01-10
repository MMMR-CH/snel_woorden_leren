using System.Collections;
using UnityEngine;

namespace MC.Modules.Keyboard
{
    public class KeyboardButtonSizeFitter : MonoBehaviour
    {
        [SerializeField] RectTransform referenceRecttransform;
        [SerializeField] float multiplyWidth = 1;

        RectTransform rectTransform;

        public void AdjustSize()
        {
            if (referenceRecttransform == null) return;
            rectTransform = GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(referenceRecttransform.sizeDelta.x * multiplyWidth, rectTransform.sizeDelta.y);            
        }
    }
}

