using UnityEngine;
using UnityEngine.EventSystems;
using System;
using UnityEngine.UI;

/// <summary>
/// Makes a UI object (RectTransform) draggable inside a reference RectTransform area.
/// - Drag starts when you press on the UI element and continues until release.
/// - The dragged rect is clamped so it never leaves the area (not just pivot).
/// - On release, it returns to its original anchored position.
/// </summary>
[DisallowMultipleComponent]
[RequireComponent(typeof(RectTransform))]
public class DraggableUIObject : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public interface IDragSource
    {
        RectTransform RectTransform { get; }
        object Owner { get; } // who owns this draggable (can be GameObject, data object, etc.)
    }

    public readonly struct DragContext
    {
        public readonly IDragSource Source;   // who drags
        public readonly DraggableUIObject DraggableUIObject;
        public readonly Vector2 Delta;    
        public readonly Vector2 Direction;          
        public readonly Vector2 AnchoredPos;        // current pos
        public readonly bool IsClamped;          
        public readonly int PointerId;              // touch/mouse

        public DragContext(
            IDragSource source,
            DraggableUIObject draggableUIObject,
            Vector2 delta,
            Vector2 direction,
            Vector2 anchoredPos,
            bool isClamped = false,
            int pointerId = -1)
        {
            Source = source;
            DraggableUIObject = draggableUIObject;
            Delta = delta;
            Direction = direction;
            AnchoredPos = anchoredPos;
            IsClamped = isClamped;
            PointerId = pointerId;
        }
    }

    public readonly struct InitData
    {
        public readonly bool IsDraggable;
        public readonly IDragSource Source;
        public readonly Canvas Canvas;
        public readonly RectTransform DragAreaTransform;
        public readonly bool CalculateDragDirection;
        public readonly Action<DragContext> OnDragDirection;
        public readonly Action<DragContext> OnDraggedListener;
        public readonly Action<DragContext> OnDragStartedListener;
        public readonly Action<DragContext> OnDragEndedListener;
        public readonly IBeginDragHandler[] ParentBeginDragHandler;
        public readonly IEndDragHandler[] ParentEndDragHandler;
        public readonly IDragHandler[] ParentDragHandler;


        public InitData(
            bool isDraggable,
            Canvas canvas,
            RectTransform dragAreaTransform,
            IDragSource source,
            bool calculateDragDirection = false,
            Action<DragContext> onDragDirection = null,
            Action<DragContext> onDraggedListener = null,
            Action<DragContext> onDragStartedListener = null,
            Action<DragContext> onDragEndedListener = null,
            IBeginDragHandler[] parentBeginDragHandler = null,
            IDragHandler[] parentDragHandler = null,
            IEndDragHandler[] parentEndDragHandler = null
            )
        {
            Source = source;
            IsDraggable = isDraggable;
            Canvas = canvas;
            DragAreaTransform = dragAreaTransform;
            CalculateDragDirection = calculateDragDirection;
            OnDragDirection = onDragDirection;
            OnDraggedListener = onDraggedListener;
            OnDragStartedListener = onDragStartedListener;
            OnDragEndedListener = onDragEndedListener;
            ParentBeginDragHandler = parentBeginDragHandler;
            ParentEndDragHandler = parentEndDragHandler;
            ParentDragHandler = parentDragHandler;
        }
    }
    #region ----------------------------------------------------------------------- EVENTS
    public event Action<DragContext> DragDirection;
    public event Action<DragContext> Dragged;   // (delta, directionNormalized)
    public event Action<DragContext> DragStarted;
    public event Action<DragContext> DragEnded;
    #endregion
    #region ----------------------------------------------------------------------- PROPERTIES
    [field: SerializeField] public bool IsDraggable { get; private set; } = true;
    [field: SerializeField] public bool IsDragging { get; private set; } = false;
    public Vector2? CurrentDragDelta { get; private set; } = null;
    public Vector2? CurrentDragDirection { get; private set; } = null;
    #endregion
    #region ----------------------------------------------------------------------- FIELDS
    private Vector2 _lastAnchoredPos;
    private Vector2 _lastPointerPos;
    private Vector2 _desiredAnchoredPos;
    private const float DIR_EPSILON = 50f; // min delta to consider for direction calculation
    private RectTransform _dragArea; // The reference area the object must stay within (fully)
    private Canvas _canvas;
    private RectTransform _self;
    private RectTransform _parentRect;
    private IDragSource _dragSource;
    private Vector2? _originalAnchoredPos;    
    private Vector2 _pointerOffset; // Offset between pointer position (in parent local space) and the rect's anchoredPosition
    private Camera _camera;
    private bool _calculateDragDirection;
    private IBeginDragHandler[] _parentBeginDragHandler;
    private IDragHandler[] _parentDragHandler;
    private IEndDragHandler[] _parentEndDragHandler;
    #endregion

    private void Awake()
    {
        _self = (RectTransform)transform;
        _parentRect = _self.parent as RectTransform;
        if (_canvas == null) _canvas = GetComponentInParent<Canvas>();
    }

    public void Init(InitData initData)
    {
        // reset listeners
        Dragged = null;
        DragStarted = null;
        DragEnded = null;

        _canvas = initData.Canvas;
        _dragArea = initData.DragAreaTransform;
        _calculateDragDirection = initData.CalculateDragDirection;

        if (initData.OnDragDirection != null)
            DragDirection += initData.OnDragDirection;

        if (initData.OnDraggedListener != null)
            Dragged += initData.OnDraggedListener;

        if (initData.OnDragStartedListener != null)
            DragStarted += initData.OnDragStartedListener;

        if (initData.OnDragEndedListener != null)
            DragEnded += initData.OnDragEndedListener;

        _dragSource = initData.Source;
        IsDraggable = initData.IsDraggable;
        _parentBeginDragHandler = initData.ParentBeginDragHandler;
        _parentDragHandler = initData.ParentDragHandler;
        _parentEndDragHandler = initData.ParentEndDragHandler;
    }


    public void OnBeginDrag(PointerEventData eventData)
    {
        _lastPointerPos = eventData.position;

        if (_parentBeginDragHandler != null && _parentBeginDragHandler.Length > 0) foreach (var handler in _parentBeginDragHandler) handler.OnBeginDrag(eventData);
        if (_parentRect == null) _parentRect = _self.parent as RectTransform;
        if (_parentRect == null) return;
        if (_dragArea == null) Debug.LogWarning($"{nameof(DraggableUIObject)}: Drag Area is not assigned. Drag will be unconstrained.", this);
        if (_originalAnchoredPos == null) _originalAnchoredPos = _self.anchoredPosition; // save original pos

        // Save original position
        _lastAnchoredPos = _self.anchoredPosition;
        CurrentDragDelta = Vector2.zero;
        CurrentDragDirection = Vector2.zero;

        IsDragging = IsDraggable;

        // Calculate pointer offset in parent-local space
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _parentRect,
                eventData.position,
                GetEventCamera(),
                out var localPointer))
        {
            _pointerOffset = _self.anchoredPosition - localPointer;
        }

        DragStarted?.Invoke(new DragContext(
            _dragSource,
            this,
            Vector2.zero,
            Vector2.zero,
            _self.anchoredPosition,
            isClamped: _dragArea != null,
            pointerId: eventData.pointerId));
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Calculate desired position
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _parentRect,
                eventData.position,
                GetEventCamera(),
                out var localPointer))
        {
            return;
        }
        _desiredAnchoredPos = localPointer + _pointerOffset;
        
        if (_calculateDragDirection)
        {
            // delta (parent local)
            CurrentDragDelta = eventData.position - _lastPointerPos;

            // direction
            if (CurrentDragDelta?.sqrMagnitude > DIR_EPSILON)
            {
                CurrentDragDirection = CurrentDragDelta?.normalized;
                _lastPointerPos = eventData.position;
                DragDirection?.Invoke(new DragContext(
                    _dragSource,
                    this,
                    CurrentDragDelta ?? Vector2.zero,
                    CurrentDragDirection ?? Vector2.zero,
                    _self.anchoredPosition,
                    isClamped: _dragArea != null,
                    pointerId: eventData.pointerId));
                //Debug.Log($"CurrentDragDirection: {CurrentDragDirection}");
            }            
        }
    
        if (!IsDraggable || !IsDragging || _parentRect == null || CurrentDragDirection == null)
        {
            if (_parentDragHandler != null && _parentDragHandler.Length > 0) foreach (var handler in _parentDragHandler) handler.OnDrag(eventData); // forward to parent if any
            return;
        }

        // Clamp to drag area
        if (_dragArea != null) _desiredAnchoredPos = ClampAnchoredPosToArea(_desiredAnchoredPos);
        // apply
        _self.anchoredPosition = _desiredAnchoredPos;
        _lastAnchoredPos = _desiredAnchoredPos;

        // notify others
        Dragged?.Invoke(new DragContext(
            _dragSource,
            this,
            CurrentDragDelta ?? Vector2.zero,
            CurrentDragDirection ?? Vector2.zero,
            _self.anchoredPosition,
            isClamped: _dragArea != null,
            pointerId: eventData.pointerId));
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (_parentEndDragHandler != null && _parentEndDragHandler.Length > 0) foreach (var handler in _parentEndDragHandler) handler.OnEndDrag(eventData); // forward to parent if any
        if (!IsDragging) return;
        IsDragging = false;
        CurrentDragDelta = null;
        CurrentDragDirection = null;
        _lastAnchoredPos = _self.anchoredPosition;
        _lastPointerPos = eventData.position;             

        ReturnToOriginalPosition();
        DragEnded?.Invoke(new DragContext(
            _dragSource,
            this,
            Vector2.zero,
            Vector2.zero,
            _self.anchoredPosition,
            isClamped: _dragArea != null,
            pointerId: eventData.pointerId));
    }

    public void SetDraggable(bool isDraggable)
    {
        IsDraggable = isDraggable;
    }

    public void ReturnToOriginalPosition()
    {
        //Debug.Log($"Returning to original position: {_originalAnchoredPos}");
        _self.anchoredPosition = _originalAnchoredPos ?? _self.anchoredPosition;
    }

    private Camera GetEventCamera()
    {
        if (_camera != null) return _camera;
        if (_canvas == null) return null;

        // Screen Space - Overlay => camera must be null
        if (_canvas.renderMode == RenderMode.ScreenSpaceOverlay) return null;

        // Screen Space - Camera or World Space => use canvas.worldCamera if available
        _camera = _canvas.worldCamera != null ? _canvas.worldCamera : Camera.main;
        return _camera;
    }

    /// <summary>
    /// Clamp the draggable rect so ALL its corners remain within the dragArea rect.
    /// Works even if draggable is scaled/rotated, by clamping based on desired world corners.
    /// </summary>
    private Vector2 ClampAnchoredPosToArea(Vector2 desiredAnchoredPos)
    {
        // Convert desired anchoredPosition (parent local) to world position
        Vector3 desiredWorldPos = _parentRect.TransformPoint(desiredAnchoredPos);

        // Compute deltaWorld from current to desired
        Vector3 currentWorldPos = _parentRect.TransformPoint(_self.anchoredPosition);
        Vector3 deltaWorld = desiredWorldPos - currentWorldPos;

        // Get current world corners, offset them to where they would be at desired position
        Vector3[] corners = new Vector3[4];
        _self.GetWorldCorners(corners);
        for (int i = 0; i < 4; i++) corners[i] += deltaWorld;

        // Convert desired corners into dragArea local space, find AABB min/max there
        Vector2 min = new Vector2(float.PositiveInfinity, float.PositiveInfinity);
        Vector2 max = new Vector2(float.NegativeInfinity, float.NegativeInfinity);

        for (int i = 0; i < 4; i++)
        {
            Vector2 c = (Vector2)_dragArea.InverseTransformPoint(corners[i]);
            min = Vector2.Min(min, c);
            max = Vector2.Max(max, c);
        }

        Rect areaRect = _dragArea.rect;

        float dx = 0f;
        float dy = 0f;

        // If any part goes outside, push it back in area local space
        if (min.x < areaRect.xMin) dx = areaRect.xMin - min.x;
        else if (max.x > areaRect.xMax) dx = areaRect.xMax - max.x;

        if (min.y < areaRect.yMin) dy = areaRect.yMin - min.y;
        else if (max.y > areaRect.yMax) dy = areaRect.yMax - max.y;

        if (dx == 0f && dy == 0f)
            return desiredAnchoredPos;

        // Apply correction in dragArea local space at the desired position’s center
        // Start from the desired center (convert desiredWorldPos to dragArea local)
        Vector2 desiredCenterInArea = (Vector2)_dragArea.InverseTransformPoint(desiredWorldPos);
        desiredCenterInArea += new Vector2(dx, dy);

        // Convert corrected center back to world, then to parent local (anchoredPosition)
        Vector3 correctedWorld = _dragArea.TransformPoint(desiredCenterInArea);
        Vector2 correctedInParent = (Vector2)_parentRect.InverseTransformPoint(correctedWorld);

        return correctedInParent;
    }
}
