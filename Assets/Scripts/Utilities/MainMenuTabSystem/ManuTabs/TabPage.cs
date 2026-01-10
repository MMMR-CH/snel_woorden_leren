using System;
using UnityEngine;

namespace MC.Modules.Tabsystem
{
    public class TabPage : MonoBehaviour
    {
        public TabGroup.Type pageType;        
        public int lefttoRightOrder;

        [SerializeField] private PageContainer _pagePrefab; 
        [SerializeField] Canvas canvas;

        PageContainer _pageObject;

        public void Appear()
        {
            if (_pagePrefab == null) return;

            // instantiate the menu if it doesn't exist
            if (_pageObject == null)
            {
                _pageObject = Instantiate(_pagePrefab, transform);
                gameObject.SetActive(true);
                canvas.enabled = true;
                _pageObject.Appear();
                return;
            }
            else
            {
                gameObject.SetActive(true);
                canvas.enabled = true;
                _pageObject.Appear();
            }
            
        }

        public void Disappear()
        {
            if (_pagePrefab == null) return;

            Action action = () =>
            {
                gameObject.SetActive(false);
                canvas.enabled = false;
            };
            if (_pageObject != null) _pageObject.Disappear(action);
        }
    }
}
