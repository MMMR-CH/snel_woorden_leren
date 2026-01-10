using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace MC.Modules.Tabsystem
{
    public class TabGroup : MonoBehaviour
    {
        public enum Type
        {
            Home = 0,
            Betekenis = 1,
            Settings = 2,
            Shop = 3,
            Invullen = 4,
            Kruiswoordraadsel = 5
        }

        readonly Dictionary<Type, string> localizedBubbleWarningTexts;

        public static bool TabLock { get; private set; }

        [SerializeField] List<TabBarButton> tabButtons;
        [SerializeField] MainMenuPages snapper;
        [SerializeField] RectTransform tabScrollBarPanel;
        [SerializeField] float selectedButtonWidthMultiplier = 1.25f;
        [SerializeField] float selectedButtonIconScaleMultiplier = 1.25f;
        [SerializeField] Canvas canvas;
        [SerializeField] GameObject buttonLockedWarning;
        [SerializeField] TextMeshProUGUI buttonLockedWarningText;

        float shiftDuration => snapper.GetSnappingAttributes().duration;
        Ease shiftEaseMethod => snapper.GetSnappingAttributes().easeMethod;
        Type? activeBubbleButton;


        private void Awake()
        {
            snapper.OnSelectionChangeStartEvent.AddListener((previousPage, newPage) => TabLock = true);
            snapper.OnSelectionChangeEndEvent.AddListener((previousPage, newPage) => TabLock = false);
        }

        void OnEnable()
        {
            // reset button lock window
            buttonLockedWarning.gameObject.SetActive(false);
            activeBubbleButton = null;
        }

        public void Start()
        {
            // Open as default page
            _OpenTab(Type.Home);

            // Assign main buttons
            foreach (var tab in tabButtons) { tab.AddAction(() => OnPressTabBarButton(tab.tabPageType)); }
        }

        public void OnPressTabBarButton(Type type)
        {
            if (TabLock) return;
            snapper.PlayButtonSoundAction?.Invoke();
            _OpenTab(type);
        }

        void _OpenTab(Type type)
        {
            // findthe button with the type
            TabBarButton tabButton = tabButtons.Find((x) => x.tabPageType == type);

            // Check if the tab is locked, if so, show the warning window
            if (!tabButton.Activated)
            {
                ShowButtonLockedWindow(type);
                return;
            }

            // Open the tab
            snapper.OpenPage(type);

            // Shift the scroll bar
            ShiftScrollBar(tabButton);

            // Set the selected icon
            for (int i = 0; i < tabButtons.Count; i++)
            {
                tabButtons[i].SetSelectedIcon(tabButton == tabButtons[i], selectedButtonIconScaleMultiplier, shiftDuration, shiftEaseMethod);
            }
        }

        void ShiftScrollBar(TabBarButton _targetButton)
        {
            for (int i = 0; i < tabButtons.Count; i++)
            {
                if (_targetButton == tabButtons[i])
                {
                    _targetButton.layoutElement.DOKill();
                    _targetButton.layoutElement.DOFlexibleSize(new Vector2(selectedButtonWidthMultiplier, 1f), shiftDuration);
                }
                else
                {
                    tabButtons[i].layoutElement.DOKill();
                    tabButtons[i].layoutElement.DOFlexibleSize(Vector2.one, shiftDuration);
                }
            }

            tabScrollBarPanel.SetParent(_targetButton.transform);
            tabScrollBarPanel.SetAsFirstSibling();
            StartCoroutine(ShiftBarCor());

            IEnumerator ShiftBarCor()
            {
                float percentage = 0f;
                float lerpFactor = 0.2f;
                while (percentage < 0.97f)
                {
                    tabScrollBarPanel.offsetMax = Vector2.Lerp(tabScrollBarPanel.offsetMax, Vector2.zero, lerpFactor);
                    tabScrollBarPanel.offsetMin = Vector2.Lerp(tabScrollBarPanel.offsetMin, Vector2.zero, lerpFactor);
                    percentage = percentage + ((1 - percentage) * lerpFactor);
                    yield return null;
                }
                tabScrollBarPanel.offsetMax = Vector2.zero;
                tabScrollBarPanel.offsetMin = Vector2.zero;
            }
        }

        void EnableDisableBottomBar(bool ON)
        {
            canvas.enabled = ON;
            gameObject.SetActive(ON);
        }


        Coroutine buttonLockedWindowCoroutine;
        void ShowButtonLockedWindow(Type type)
        {
            if (activeBubbleButton == type) return;
            activeBubbleButton = type;
            if(localizedBubbleWarningTexts != null && localizedBubbleWarningTexts.ContainsKey(type)) buttonLockedWarningText.text = localizedBubbleWarningTexts[type];
            if (buttonLockedWindowCoroutine != null) StopCoroutine(buttonLockedWindowCoroutine);
            buttonLockedWindowCoroutine = StartCoroutine(ShowButtonLockedWindowCor());
            IEnumerator ShowButtonLockedWindowCor()
            {
                TabBarButton tabButton = tabButtons.Find((x) => x.tabPageType == type);
                buttonLockedWarning.transform.position = tabButton.bubblePlugPoint.position;
                buttonLockedWarning.gameObject.SetActive(true);
                buttonLockedWarning.transform.localScale = Vector3.zero;
                buttonLockedWarning.transform.DOScale(Vector3.one, 0.35f).SetEase(Ease.OutBack);
                yield return new WaitForSeconds(2.5f);
                buttonLockedWarning.transform.DOScale(Vector3.zero, 0.15f).OnComplete(() =>
                {
                    buttonLockedWarning.gameObject.SetActive(false);
                    activeBubbleButton = null;
                });
            }
        }
    }
}
