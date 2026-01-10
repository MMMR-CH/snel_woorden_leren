using System;
using UnityEngine;

namespace MC.Modules.Tabsystem
{
    public class PageContainer : MonoBehaviour
    {
        public virtual void Appear(Action onFinish = null)
        {
            // This method should be overridden in derived classes to implement specific appearance logic.
            // The onFinish action can be invoked when the appearance is complete.
            onFinish?.Invoke();
        }

        public virtual void Disappear(Action onFinish = null)
        {   
            // This method should be overridden in derived classes to implement specific disappearance logic.
            // The onFinish action can be invoked when the disappearance is complete.
            onFinish?.Invoke();
        }
    }
}
