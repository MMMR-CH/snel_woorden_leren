using DG.Tweening;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace MC.Modules.Tabsystem
{
    public class MainMenuPages : MonoBehaviour
    {
        public TabPage CurrentSnappablePage { get; private set; }
        public bool IsInitialized { get; private set; } = false;

        public UnityEvent<TabPage, TabPage> OnSelectionChangeStartEvent { get; private set; } = new();
        public UnityEvent<TabPage, TabPage> OnSelectionChangeEndEvent { get; private set; } = new();
        public UnityAction PlayButtonSoundAction { get; private set; }


        [SerializeField] TabPage[] pages;
        [SerializeField] float snapDuration = 0.5f;
        [SerializeField] Ease snapEase = Ease.InCubic;
        [SerializeField] TabGroup tabGroup;

        TabPage mainPageTab;


        public (float duration, Ease easeMethod) GetSnappingAttributes() => (snapDuration, snapEase);

        bool tabLock;

        public void Init(UnityAction playButtonSoundAction, UnityAction<TabPage, TabPage> onSelectionChangeStartEvent = null, UnityAction<TabPage, TabPage> onSelectionChangeEndEvent = null)
        {
            PlayButtonSoundAction = playButtonSoundAction;
            if (onSelectionChangeStartEvent != null) OnSelectionChangeStartEvent.AddListener(onSelectionChangeStartEvent);
            if (onSelectionChangeEndEvent != null) OnSelectionChangeEndEvent.AddListener(onSelectionChangeEndEvent);

            IsInitialized = true;
        }


        public void OpenPage(TabGroup.Type type)
        {
            if (!IsInitialized)
            {
                Debug.LogError("MainMenuPages is not initialized! Call Init() before using this method.");
                return;
            }

            TabPage newPage = pages.FirstOrDefault((x) => x.pageType == type);
            if (newPage == null)
            {
                Debug.LogException(new System.Exception($"TabPage with type {type} not found!"));
                return;
            }

            if (CurrentSnappablePage == null) mainPageTab = CurrentSnappablePage = pages.First((x) => x.pageType == TabGroup.Type.Home);

            // Just ensure that page is opened
            if (newPage == CurrentSnappablePage || tabLock)
            {
                newPage.Appear();
                newPage.transform.localPosition = Vector3.zero;
                return;
            }

            tabLock = true;
            var previousPage = CurrentSnappablePage;
            OnSelectionChangeStartEvent?.Invoke(previousPage, newPage);
            bool shiftLeft = newPage.lefttoRightOrder > CurrentSnappablePage.lefttoRightOrder;
            float width = mainPageTab.GetComponent<RectTransform>().rect.width;
            CurrentSnappablePage.transform.DOLocalMoveX((shiftLeft ? -1 : 1) * width, snapDuration).SetRelative().SetEase(snapEase)
                .OnComplete(() => CurrentSnappablePage.Disappear());
            newPage.transform.localPosition = new Vector3((shiftLeft ? 1 : -1) * width, newPage.transform.localPosition.y, newPage.transform.localPosition.z);
            newPage.Appear();
            newPage.transform.DOLocalMoveX(0, snapDuration).OnComplete(() => 
            {                
                CurrentSnappablePage = newPage;
                tabLock = false;
                OnSelectionChangeEndEvent?.Invoke(previousPage, newPage);
            }).SetEase(snapEase);
        }        
    }
}
