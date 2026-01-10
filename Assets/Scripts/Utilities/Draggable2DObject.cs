using UnityEngine;
using UnityEngine.InputSystem;

public class Draggable2DObject : MonoBehaviour
{
    [Header("Drag Settings")]
    [SerializeField] Camera _cam;                   // If null, will use Camera.main
    [field: SerializeField] public bool Draggable { get; private set; } = true;   // Requires a Collider2D to hit-test
    [SerializeField] bool onlyDragWhenHit = true; // Only start dragging when pointer hits the object
    [SerializeField] bool keepZ = true;             // Keep original Z while dragging
    [SerializeField] Collider2D _selfCollider;  

    [Header("Drag Area (Reference Plane)")]
    [SerializeField] Collider2D _dragAreaCollider;   // Limits dragging within this area
    [SerializeField] bool _clampUsingObjectBounds = true; // Prevent overflow (not just pivot)

    [Header("Drag Area (Reference UI Recttransform)")] // If not null, will use this instead of dragAreaCollider
    [SerializeField] RectTransform _dragAreaRectTransform; // Limits dragging within this UI rect (converted to world space)
    [SerializeField] bool _clampUsingUIBounds = true; // Prevent overflow (not just pivot)

    private bool _dragging;
    private Vector3 _offsetWorld;
    private float _originalZ;
    private Vector3 _worldPosition, _targetPosition;

    
    private float _tempZ;

    private void Awake()
    {
        if (_cam == null) _cam = Camera.main;
        _originalZ = transform.position.z;

        // Optional: object collider used for bounds-based clamping
        _selfCollider ??= GetComponentInChildren<Collider2D>();
    }

    private void Update()
    {
        if (_cam == null || !Draggable) return;

        var pointer = Pointer.current;
        if (pointer == null) return;

        // Pointer pressed this frame
        if (pointer.press.wasPressedThisFrame)
        {
            _worldPosition = ScreenToWorld(pointer.position.ReadValue());

            if (!onlyDragWhenHit || HitThis(_worldPosition))
            {
                _dragging = true;
                _offsetWorld = transform.position - _worldPosition;
            }
        }

        // While pressed, move
        if (_dragging)
        {
            _worldPosition = ScreenToWorld(pointer.position.ReadValue());
            _targetPosition = _worldPosition + _offsetWorld;

            if (keepZ) _targetPosition.z = _originalZ;

            // Clamp to drag area so the object never overflows the plane
            _targetPosition = ClampToWorldDragArea(_targetPosition);
            _targetPosition = ClampToUIDragArea(_targetPosition);

            MoveToPosition(_targetPosition);
        }

        // Released this frame
        if (pointer.press.wasReleasedThisFrame)
        {
            _dragging = false;
        }
    }

    private Vector3 ScreenToWorld(Vector2 screenPos)
    {
        _tempZ = Mathf.Abs(_cam.transform.position.z - transform.position.z);
        return _cam.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, _tempZ));
    }

    private bool HitThis(Vector3 worldPos)
    {
        // If you want "onlyDragWhenHit", you need a collider on the draggable object.
        // (Your original code used dragAreaCollider here, but that makes "hit-test" equal to "inside the area".)
        if (!Draggable) return true;
        if (_selfCollider == null) return false;

        return _selfCollider.OverlapPoint(worldPos);
    }

    private Vector3 ClampToWorldDragArea(Vector3 desiredPos)
    {
        if (!_clampUsingObjectBounds || _selfCollider == null) return desiredPos;

        // Drag area AABB
        Bounds area = _dragAreaCollider.bounds;

        // Object extents (half-size) so the WHOLE object stays inside the area
        Bounds obj = _selfCollider.bounds;
        Vector3 ext = obj.extents;

        // Clamp position so: (pos - ext) >= area.min  AND  (pos + ext) <= area.max
        float minX = area.min.x + ext.x;
        float maxX = area.max.x - ext.x;
        float minY = area.min.y + ext.y;
        float maxY = area.max.y - ext.y;

        desiredPos.x = Mathf.Clamp(desiredPos.x, minX, maxX);
        desiredPos.y = Mathf.Clamp(desiredPos.y, minY, maxY);

        return desiredPos;
    }

    private Vector3 ClampToUIDragArea(Vector3 desiredPos)
    {
        if (!_clampUsingUIBounds || _dragAreaRectTransform == null) return desiredPos;

        Vector3[] worldCorners = new Vector3[4];
        _dragAreaRectTransform.GetWorldCorners(worldCorners);

        float minX = worldCorners[0].x;
        float maxX = worldCorners[2].x;
        float minY = worldCorners[0].y;
        float maxY = worldCorners[2].y;

        // Object extents (half-size) so the WHOLE object stays inside the area
        Bounds obj = _selfCollider.bounds;
        Vector3 ext = obj.extents;

        // Clamp position so: (pos - ext) >= area.min  AND  (pos + ext) <= area.max
        float clampedMinX = minX + ext.x;
        float clampedMaxX = maxX - ext.x;
        float clampedMinY = minY + ext.y;
        float clampedMaxY = maxY - ext.y;

        desiredPos.x = Mathf.Clamp(desiredPos.x, clampedMinX, clampedMaxX);
        desiredPos.y = Mathf.Clamp(desiredPos.y, clampedMinY, clampedMaxY);

        return desiredPos;
    }

    private void MoveToPosition(Vector3 targetPos)
    {
        if (keepZ) targetPos.z = _originalZ;
        transform.position = targetPos;
    }
}
