using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class HorizontalScrollSnap : MonoBehaviour, IBeginDragHandler, IEndDragHandler
{
    public bool CanSnap { get; private set; } = false;
    [SerializeField] ScrollRect scrollRect;
    [SerializeField] RectTransform content;
    [SerializeField] RectTransform snapPoint;
    [SerializeField] List<RectTransform> items;

    [SerializeField] float snapSpeed = 10f;
    [SerializeField] float velocityThreshold = 50f;

    private bool isDragging;
    private Coroutine snapCoroutine;

    void Awake()
    {
        Reset();
    }

    void Update()
    {
        if (isDragging || !CanSnap)
            return;

        if (scrollRect.velocity.magnitude < velocityThreshold)
        {
            if (snapCoroutine == null)
                snapCoroutine = StartCoroutine(SnapToClosest());
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!CanSnap) return;
        //Debug.Log("Begin Drag");
        isDragging = true;

        if (snapCoroutine != null)
        {
            StopCoroutine(snapCoroutine);
            snapCoroutine = null;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;
    }

    public void SetSnappable(bool canSnap)
    {
        CanSnap = canSnap;
        if (!CanSnap && snapCoroutine != null)
        {
            StopCoroutine(snapCoroutine);
            snapCoroutine = null;
        }
    }

    public void AddChildItem(RectTransform child)
    {
        if (items == null)
            items = new List<RectTransform>();
        items.Add(child);
    }

    RectTransform _closest;
    IEnumerator SnapToClosest()
    {
        scrollRect.velocity = Vector2.zero;
        _closest = null;
        float minDistance = float.MaxValue;

        foreach (var item in items)
        {
            float dist = Mathf.Abs(GetItemCenterWorldPositionX(item) - snapPoint.position.x);
            if (dist < minDistance)
            {
                minDistance = dist;
                _closest = item;
            }
        }

        if (!_closest)
        {
            snapCoroutine = null;
            yield break;
        }

        Vector2 targetPos = GetTargetContentPosition(_closest);

        while (Vector2.Distance(content.anchoredPosition, targetPos) > 0.5f)
        {
            content.anchoredPosition = Vector2.Lerp(
                content.anchoredPosition,
                targetPos,
                Time.deltaTime * snapSpeed
            );
            yield return null;
        }

        content.anchoredPosition = targetPos;
        snapCoroutine = null;
    }

    float GetItemCenterWorldPositionX(RectTransform item)
    {        
        return item.TransformPoint(Vector2.left * item.pivot.x * item.rect.size.x).x;
    }

    float _deltaX;
    Vector2 _contentLocal, _targetLocal, _snapPointLocal;
    Vector2 GetTargetContentPosition(RectTransform target)
    {
        _snapPointLocal = scrollRect.viewport.InverseTransformPoint(snapPoint.position);
        _targetLocal = scrollRect.viewport.InverseTransformPoint(target.position);
        _deltaX = _snapPointLocal.x - _targetLocal.x;
        return content.anchoredPosition + Vector2.right * _deltaX;
    }

    void Reset()
    {
        if (!scrollRect)
            scrollRect = GetComponent<ScrollRect>();

        if (!content)
            content = scrollRect.content;

        if (items != null)
            items.Clear();
    }
}
