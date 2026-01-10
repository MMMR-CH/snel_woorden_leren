using System;
using SWL;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SnelWoordenLeren.Levels
{
    public class WordUIObject : MonoBehaviour, DraggableUIObject.IDragSource
    {
        public struct InitData
        {
            public WoordData WoordData;
            public Canvas Canvas;
            public RectTransform DragAreaTransform;
            public Action<DraggableUIObject.DragContext> OnDragDirection;
            public Action<DraggableUIObject.DragContext> OnDraggedListener;
            public Action<DraggableUIObject.DragContext> OnDragStartedListener;
            public Action<DraggableUIObject.DragContext> OnDragEndedListener;
            public IBeginDragHandler[] ParentBeginDragHandler;
            public IDragHandler[] ParentDragHandler;
            public IEndDragHandler[] ParentEndDragHandler;
        }
        [SerializeField] DraggableUIObject _draggableUIObject;
        [SerializeField] private TMPro.TextMeshProUGUI _wordText;

        public RectTransform RectTransform => GetComponent<RectTransform>();
        public object Owner => gameObject;
        public DraggableUIObject DraggableUIObject => _draggableUIObject;

        public void Init(InitData initData)
        {
            WoordData woordData = initData.WoordData;
            Canvas canvas = initData.Canvas;
            RectTransform dragAreaTransform = initData.DragAreaTransform;

            // Initialize DraggableUIObject
            DraggableUIObject.InitData draggableInitData = new DraggableUIObject.InitData(
                isDraggable: true,
                canvas: canvas,
                dragAreaTransform: dragAreaTransform,
                source: this,
                calculateDragDirection: true,
                onDragDirection: initData.OnDragDirection,
                onDraggedListener: initData.OnDraggedListener,
                onDragStartedListener: initData.OnDragStartedListener,
                onDragEndedListener: initData.OnDragEndedListener,
                parentBeginDragHandler: initData.ParentBeginDragHandler,
                parentDragHandler: initData.ParentDragHandler,
                parentEndDragHandler: initData.ParentEndDragHandler);
            _draggableUIObject.Init(draggableInitData);

            // Set word text
            _wordText.text = woordData.WOORD;
        }
    }
}
