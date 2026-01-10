using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

namespace MC.Modules.Tabsystem
{
    [RequireComponent(typeof(Button))]
    public class TabBarButton : MonoBehaviour
    {
        [field: SerializeField] public bool Activated { get ; private set; }
        public TabGroup tabGroup;
        public TabGroup.Type tabPageType;
        public LayoutElement layoutElement;

        [SerializeField] Image selectedTabIconImage;
        [SerializeField] RectTransform selectedTabIcon;
        [SerializeField] Button button;
        [field: SerializeField] public RectTransform bubblePlugPoint { get; private set; }

        Vector3 firstIconLocalPosition;

        private void Awake()
        {
            firstIconLocalPosition = selectedTabIcon.localPosition;
        }

        public void AddAction(Action buttonAction)
        {
            button.onClick.AddListener(buttonAction.Invoke);
        }

        public void SetSelectedIcon(bool ON, float _newSize, float duration, Ease _ease)
        {
            selectedTabIconImage.DOFade(ON ? 1f : 0f, duration).SetEase(_ease);
            selectedTabIcon.DOScale(ON ? _newSize : 1f, duration).SetEase(_ease);
            float localNewY = ON ? firstIconLocalPosition.y + selectedTabIcon.sizeDelta.y * 0.25f : firstIconLocalPosition.y;
            selectedTabIcon.DOAnchorPos(Vector2.up * localNewY, duration).SetEase(_ease);
        }

        Graphic[] graphics;
        public void ActivateDeactivate(bool ON)
        {
            Activated = ON;
            if (graphics == null) graphics = selectedTabIcon.GetComponentsInChildren<Graphic>();
            foreach (var item in graphics)
            {
                if (item != selectedTabIconImage) item.color = new Color(1, 1, 1, ON ? 1f : 0.25f);
            }
        }
    }
}
