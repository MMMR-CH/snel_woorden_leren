using UnityEngine;
using UnityEngine.UI;

namespace SWL.Presentation.UI.Navigation
{
    [RequireComponent(typeof(Button))]
    public sealed class TabButton : MonoBehaviour
    {
        [SerializeField] private TabRouter router;
        [SerializeField] private TabId tabId;
        [SerializeField] private Button button;

        private void Awake()
        {
            Reset();
            button.onClick.AddListener(Click);
        }

        private void OnEnable()
        {
            button ??= GetComponent<Button>();
            button.onClick.AddListener(Click);
        }

        private void OnDisable()
        {
            if (button != null) button.onClick.RemoveListener(Click);
        }

        void Reset()
        {
            button ??= GetComponent<Button>();
            router ??= GetComponentInParent<TabRouter>(true);
        }


        private void Click()
        {
            if (router == null) return;
            router.Show(tabId);
        }
    }
}
