using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace MC.Utility.BackPressSystem
{
    public class BackPressableObject //: MonoBehaviour, IBackPressable
    {
        //[SerializeField] private Button closeButton;
        //[SerializeField] UnityEvent onBack;
        //private bool _isApplicationQuitting;

        //private void OnEnable()
        //{
        //    AddToBackPressStack();
        //}

        //private void OnDisable()
        //{
        //    if (!_isApplicationQuitting)
        //    {
        //        BackPressManager.Instance.PopDesiredItemIfPossible(this);
        //    }
        //}

        //private void Start()
        //{
        //    if (closeButton)
        //    {
        //        closeButton.onClick.AddListener(() => BackPressManager.Instance.PopDesiredItemIfPossible(this));
        //    }
        //}

        //public void AddToBackPressStack()
        //{
        //    BackPressManager.Instance.AddToBackPressStack(this);
        //}

        //public void OnBackPress()
        //{
        //    onBack?.Invoke();
        //}

        //private void OnApplicationQuit()
        //{
        //    _isApplicationQuitting = true;
        //}
    }
}