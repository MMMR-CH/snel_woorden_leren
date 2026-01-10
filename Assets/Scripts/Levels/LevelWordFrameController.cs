using System;
using System.Runtime.CompilerServices;
using SWL;
using UnityEngine;
using UnityEngine.UI;

namespace SnelWoordenLeren.Levels
{
    public class LevelWordFrameController : MonoBehaviour
    {
        [Header("WordFrameContents Settings")]
        [SerializeField] private WordFrameContents _wordFrameContents;
        [SerializeField] RectTransform _wordDragArea;
        [SerializeField] Canvas _canvas;
        [SerializeField] HorizontalScrollSnap _horizontalScrollSnap;
        [SerializeField] ScrollRect _scrollRect;

        [SpecialName, Header("Word Frame Settings")]
        [SerializeField] private float _verticalDragThreshold = 30f;


        public void Init(
            WoordData[] woordenData,
            Action<DraggableUIObject.DragContext> onDragDirection = null,
            Action<DraggableUIObject.DragContext> onWordDragCallback = null,
            Action<DraggableUIObject.DragContext> onWordDragBeginCallback = null,
            Action<DraggableUIObject.DragContext> onWordDragEndCallback = null)
        {
            // add drag angel listener to determine if the drag is more horizontal or vertical
            onDragDirection += OnDragDirection;
            onWordDragEndCallback += OnWordDragEndCallback;

            // Initialize WordFrameContents
            _wordFrameContents.Init(new WordFrameContents.InitData
            {
                WoordData = woordenData,
                WordDragArea = _wordDragArea,
                Canvas = _canvas,
                HorizontalScrollSnap = _horizontalScrollSnap,
                OnDraggedDirection = onDragDirection,
                OnDraggedListener = onWordDragCallback,
                OnDragStartedListener = onWordDragBeginCallback,
                OnDragEndedListener = onWordDragEndCallback,
                ParentDragHandler = new UnityEngine.EventSystems.IDragHandler[] { _scrollRect },
                ParentEndDragHandler = new UnityEngine.EventSystems.IEndDragHandler[] { _scrollRect, _horizontalScrollSnap },
                ParentBeginDragHandler = new UnityEngine.EventSystems.IBeginDragHandler[] { _scrollRect, _horizontalScrollSnap }
            });
        }

        [SerializeField] float angle;
        bool wordDragging;

        void OnDragDirection(DraggableUIObject.DragContext context)
        {
            if (wordDragging) return; // prevent re-entrance
            // Check the drag direction angel to determine if it's more horizontal or vertical
            angle = Vector2.Angle(context.Direction, Vector2.down); // Angle with respect to horizontal axis
            if (angle > _verticalDragThreshold) // Mostly horizontal drag
            {
                //Debug.Log("Horizontal Drag Detected: " + angle);
                // Enable scrolling
                _scrollRect.enabled = true;
                _horizontalScrollSnap.SetSnappable(true);

                // stop dragging the word and return it to its original position
                context.DraggableUIObject.SetDraggable(false);
                context.DraggableUIObject.ReturnToOriginalPosition();
            }
            else if (angle <= _verticalDragThreshold) // Mostly vertical drag
            {
                wordDragging = true;
                //Debug.Log("Vertical Drag Detected: " + angle);
                // Disable scrolling while dragging a word
                _scrollRect.enabled = false;
                _horizontalScrollSnap.SetSnappable(false);

                context.DraggableUIObject.SetDraggable(true);
            }
        }
        
        void OnWordDragEndCallback(DraggableUIObject.DragContext context)
        {
            // Re-enable scrolling after word drag ends
            _scrollRect.enabled = true;
            _horizontalScrollSnap.SetSnappable(true);
            wordDragging = false;
            context.DraggableUIObject.SetDraggable(true);
        }
    }
}
