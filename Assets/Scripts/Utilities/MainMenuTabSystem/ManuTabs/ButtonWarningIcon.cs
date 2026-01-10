using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace FW_Utilities
{
    public class ButtonWarningIcon : MonoBehaviour
    {
        [SerializeField] Image warningImage;
        [SerializeField] float vibrato = 20, duration = 0.5f, strength = 60;

        public void Activate(bool ON)
        {
            if (ON && !warningImage.enabled)
            {
                warningImage.enabled = true;
                var seq = DOTween.Sequence();
                seq.Append(transform.DOShakeRotation(duration, Vector3.forward * strength, (int)vibrato));
                seq.AppendInterval(2f);
                seq.SetLoops(-1);
            }
            else if (!ON && warningImage.enabled)
            {
                transform.DOKill();
                warningImage.enabled = false;
            }
        }


    }
}

