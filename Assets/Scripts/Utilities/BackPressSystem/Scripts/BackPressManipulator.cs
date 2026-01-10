using UnityEngine;

namespace MC.Utility.BackPressSystem
{
    public class BackPressManipulator : MonoBehaviour
    {
        [SerializeField] private bool blockBackPressOnEnable = true;
        [SerializeField] private bool allowBackPressOnDisable = true;
        [SerializeField] private bool isSuperManipulator;
        private bool _isApplicationQuitting;

        public bool IsSuperManipulator => isSuperManipulator;

        private void OnEnable()
        {
            if (blockBackPressOnEnable)
            {
                BackPressManager.Instance.AddManipulator(this);
            }
        }

        private void OnDisable()
        {
            if (!_isApplicationQuitting && allowBackPressOnDisable)
            {
                BackPressManager.Instance.RemoveManipulator(this);
            }
        }

        private void OnApplicationQuit()
        {
            _isApplicationQuitting = true;
        }
    }
}