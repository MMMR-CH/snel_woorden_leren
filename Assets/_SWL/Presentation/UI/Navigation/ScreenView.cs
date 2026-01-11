using UnityEngine;

namespace SWL.Presentation.UI.Navigation
{
    public abstract class ScreenView : MonoBehaviour, IScreen
    {
        [SerializeField] private GameObject root; // root object of the screen

        protected virtual void Reset()
        {
            root = gameObject;
        }

        public virtual void Show()
        {
            if (root != null) root.SetActive(true);
            else gameObject.SetActive(true);
        }

        public virtual void Hide()
        {
            if (root != null) root.SetActive(false);
            else gameObject.SetActive(false);
        }
    }
}
