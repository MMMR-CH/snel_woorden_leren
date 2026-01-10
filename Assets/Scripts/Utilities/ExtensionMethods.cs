using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins;
using DG.Tweening.Plugins.Core.PathCore;
using DG.Tweening.Plugins.Options;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Color = UnityEngine.Color;

public static class ExtensionMethods
{
    /// <summary>Tweens a Rigidbody's position through the given path waypoints, using the chosen path algorithm.
    /// Also stores the Rigidbody as the tween's target so it can be used for filtered operations.
    /// <para>NOTE: to tween a rigidbody correctly it should be set to kinematic at least while being tweened.</para>
    /// <para>BEWARE: doesn't work on Windows Phone store (waiting for Unity to fix their own bug).
    /// If you plan to publish there you should use a regular transform.DOPath.</para></summary>
    /// <param name="path">The waypoints to go through</param>
    /// <param name="duration">The duration of the tween</param>
    /// <param name="pathType">The type of path: Linear (straight path) or CatmullRom (curved CatmullRom path)</param>
    /// <param name="pathMode">The path mode: 3D, side-scroller 2D, top-down 2D</param>
    /// <param name="resolution">The resolution of the path (useless in case of Linear paths): higher resolutions make for more detailed curved paths but are more expensive.
    /// Defaults to 10, but a value of 5 is usually enough if you don't have dramatic long curves between waypoints</param>
    /// <param name="gizmoColor">The color of the path (shown when gizmos are active in the Play panel and the tween is running)</param>
    public static TweenerCore<Vector3, Path, PathOptions> DOAnchorPosPath(
        this RectTransform target, Vector3[] path, float duration, PathType pathType = PathType.Linear,
        PathMode pathMode = PathMode.Full3D, int resolution = 10, Color? gizmoColor = null
    )
    {
        if (resolution < 1) resolution = 1;
        TweenerCore<Vector3, Path, PathOptions> t = DOTween.To(PathPlugin.Get(), () => target.anchoredPosition, v3 => target.anchoredPosition = v3, new Path(pathType, path, resolution, gizmoColor), duration)
            .SetTarget(target);

        t.plugOptions.mode = pathMode;
        return t;
    }

    public static float Map(float baseCurrent, float baseMin, float baseMax, float targetMin, float targetMax)
    {
        return (baseCurrent - baseMin) / (baseMax - baseMin) * (targetMax - targetMin) + targetMin;
    }
    public static float MapClamped(float baseCurrent, float baseMin, float baseMax, float targetMin, float targetMax)
    {
        return Mathf.Clamp((baseCurrent - baseMin) / (baseMax - baseMin) * (targetMax - targetMin) + targetMin, targetMin, targetMax);
    }

    public static float MapInverse(float baseCurrent, float baseMin, float baseMax, float targetMin, float targetMax)
    {
        return (1f - (baseCurrent - baseMin) / (baseMax - baseMin)) * (targetMax - targetMin) + targetMin;
    }
    public static float MapClampedInverse(float baseCurrent, float baseMin, float baseMax, float targetMin, float targetMax)
    {
        return Mathf.Clamp((1f - (baseCurrent - baseMin) / (baseMax - baseMin)) * (targetMax - targetMin) + targetMin, targetMin, targetMax);
    }
    public static Coroutine WaitAndDo(this MonoBehaviour mono, float waitTime, Action action)
    {
        return mono.StartCoroutine(WaitAndDoEnumerator(waitTime, action));
    }
    private static IEnumerator WaitAndDoEnumerator(float waitTime, Action action)
    {
        yield return new WaitForSeconds(waitTime);
        action();
    }
    public static Coroutine WaitAndDoAfter(this MonoBehaviour mono, Func<bool> predicate, Action action)
    {
        return mono.StartCoroutine(WaitAndDoAfterEnumerator(predicate, action));
    }
    private static IEnumerator WaitAndDoAfterEnumerator(Func<bool> predicate, Action action)
    {
        yield return new WaitUntil(predicate);
        action();
    }
    public static Coroutine WaitAndDoRT(this MonoBehaviour mono, float waitTime, Action action)
    {
        return mono.StartCoroutine(WaitAndDoRTEnumerator(waitTime, action));
    }
    private static IEnumerator WaitAndDoRTEnumerator(float waitTime, Action action)
    {
        yield return new WaitForSecondsRealtime(waitTime);
        action();
    }
    public static Color Set(this Color c, float? r = null, float? g = null, float? b = null, float? a = null)
    {
        float R = r ?? c.r;
        float G = g ?? c.g;
        float B = b ?? c.b;
        float A = a ?? c.a;
        return new Color(R, G, B, A);
    }
    public static Vector2 Set(this Vector2 c, float? x = null, float? y = null)
    {
        float X = x ?? c.x;
        float Y = y ?? c.y;
        return new Vector2(X, Y);
    }
    public static Vector3 SetNoDepth(this Vector3 c, float? x = null, float? y = null)
    {
        float X = x ?? c.x;
        float Y = y ?? c.y;
        return new Vector3(X, Y, 0);
    }
    public static Vector3 Set(this Vector3 c, float? x = null, float? y = null, float? z = null)
    {
        float X = x ?? c.x;
        float Y = y ?? c.y;
        float Z = z ?? c.z;
        return new Vector3(X, Y, Z);
    }
    public static Vector2 Rotate(this Vector2 v, float degrees)
    {
        float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
        float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);

        float tx = v.x;
        float ty = v.y;
        v.x = (cos * tx) - (sin * ty);
        v.y = (sin * tx) + (cos * ty);
        return v;
    }

    /*
    public static void Play(this AudioClip audioClip, bool stopOnSceneChange = false)
    {
        AudioManager.instance.Play(audioClip, stopOnSceneChange);
    }

    public static void Play(this AudioClip audioClip, float delay)
    {
        AudioManager.instance.WaitAndDo(delay, () => AudioManager.instance.Play(audioClip));
    }
    public static void Stop(this AudioClip audioClip)
    {
        AudioManager.instance.Stop(audioClip);
    }
    */

    public static Vector2 RadianToVector2(float radian)
    {
        return new Vector2(Mathf.Cos(radian), Mathf.Sin(radian));
    }

    public static Vector2 DegreeToVector2(float degree)
    {
        return RadianToVector2(degree * Mathf.Deg2Rad);
    }

    public static void ReplaceWith(this GameObject go, GameObject other)
    {
        if (!go || !other) return;
        go.SetActive(false);
        other.SetActive(true);
    }

    public static void AddListenerOnce(this UnityEngine.Events.UnityEvent ev, UnityEngine.Events.UnityAction action)
    {
        ev.AddListener(action);
        ev.AddListener(() => ev.RemoveListener(action));
    }
    public static void AddListenerOnce<T>(this UnityEngine.Events.UnityEvent<T> ev, UnityEngine.Events.UnityAction<T> action)
    {
        ev.AddListener(action);
        ev.AddListener(val => ev.RemoveListener(action));
    }

    public static Vector2 ScreenToCanvasPoint(RectTransform canvas, Vector2 screenPoint, bool isScreenSpace = false)
    {
        //if (isScreenSpace) return screenPoint - canvas.sizeDelta / 2;
        if (isScreenSpace) return screenPoint - new Vector2(Screen.width / 2f, Screen.height / 2f);
        return (screenPoint - canvas.anchoredPosition) / canvas.localScale.x;
    }

    public static float ClampAngle0360(float angle)
    {
        while (angle < 0f) angle += 360f;
        while (angle > 360f) angle -= 360f;
        return angle;
    }
    public static float ClampAngle_180(float angle)
    {
        while (angle < -180f) angle += 360f;
        while (angle > 180f) angle -= 360f;
        return angle;
    }

    public static Task WaitForClick(this Button button)
    {
        TaskCompletionSource<bool> promise = new TaskCompletionSource<bool>();
        button.onClick.AddListenerOnce(() =>
        {
            promise.SetResult(true);
        });
        return promise.Task;
    }

    public static Task WaitAsync(this MonoBehaviour mono, float seconds)
    {
        TaskCompletionSource<bool> promise = new TaskCompletionSource<bool>();
        if (mono == null) return Task.Delay(0);
        mono.StartCoroutine(WaitAndDoEnumerator(seconds, () => 
        {
            promise.SetResult(true);
        }));
        return promise.Task;
    }
    public static void SetSpriteNativeSize(this Image im, Sprite s)
    {
        im.sprite = s;
        im.SetNativeSize();
    }
    public static void SetSpriteNativeSize(this Image _image)
    {
        if (_image.TryGetComponent(out RectTransform rectTr))
        {
            rectTr.sizeDelta = new Vector2(_image.sprite.rect.width, _image.sprite.rect.height);
        }
    }
    public static void SetListener(this Button.ButtonClickedEvent evt, UnityAction action)
    {
        evt.RemoveAllListeners();
        evt.AddListener(action);
    }

    public static void PopUpAnim(this RectTransform rectTransform, float ascensionTime, float topScale, float decensionTime, float lasScale, Action afterPopUpCallback = null)
    {
        rectTransform.localScale = Vector3.zero;
        rectTransform.gameObject.SetActive(true);
        rectTransform.DOScale(Vector3.one * topScale, ascensionTime).OnComplete(() =>
        {
            rectTransform.DOScale(Vector3.one * lasScale, decensionTime).OnComplete(() =>
            {
                afterPopUpCallback?.Invoke();
            });
        });
    }

    public static T Random<T>(this IEnumerable<T> source)
    {
        //DONE: do not forget to validate public methods' arguments
        if (source is null)
            throw new ArgumentNullException(nameof(source));

        // If enumerable is a collection (array, list) we can address items explictitly
        if (source is ICollection<T> collection)
        {
            if (collection.Count <= 0)
                throw new ArgumentOutOfRangeException(nameof(source),
                  $"Empty {nameof(source)} is not supported.");

            return collection.ElementAt(UnityEngine.Random.Range(0, collection.Count));
        }

        // In general case we have to materialize the enumeration
        var list = source.ToList();

        if (list.Count <= 0)
            throw new ArgumentOutOfRangeException(nameof(source),
              $"Empty {nameof(source)} is not supported.");

        return list[UnityEngine.Random.Range(0, list.Count)];
    }

    public static UnityEngine.Color HexadecimalToColor(this string hexa)
    {
        UnityEngine.Color color = UnityEngine.Color.white;
        ColorUtility.TryParseHtmlString("hexa", out color);
        return color;
    }

    public static T RandomEnum<T>() where T : struct, IConvertible
    {
        if (!typeof(T).IsEnum) { throw new Exception("random enum variable is not an enum"); }

        var random = new System.Random();
        var values = Enum.GetValues(typeof(T));
        return (T)values.GetValue(random.Next(values.Length));
    }

    public static TaskAwaiter GetAwaiter(this AsyncOperation asyncOp)
    {
        var tcs = new TaskCompletionSource<AsyncOperation>();
        asyncOp.completed += operation => { tcs.SetResult(operation); };
        return ((Task)tcs.Task).GetAwaiter();
    }

    public static string KiloFormat(this long num)
    {
        //if (num >= 100000000)
        //    return (num / 1000000).ToString("#.0M");

        if (num >= 1000000000)
            return (num / 1000000).ToString("0.#") + "M";

        //if (num >= 100000)
        //    return (num / 1000).ToString("#.0K");

        if (num >= 1000000)
            return (num / 1000).ToString("0.#") + "K";

        return num.ToString();
    }

    public static string KiloFormat(this int num)
    {
        return KiloFormat((long)num);
    }
}

public static class ScrollRectExtensions
{
    public static void SnapPositionToBringChildIntoView(this ScrollRect instance, RectTransform child, float yOffset = 0)
    {
        instance.content.anchoredPosition = instance.GetSnapToPositionToBringChildIntoView(child, yOffset);
    }

    public static Vector2 GetSnapToPositionToBringChildIntoView(this ScrollRect instance, RectTransform child, float yOffset = 0)
    {
        Canvas.ForceUpdateCanvases();
        var contentPos = (Vector2)instance.transform.InverseTransformPoint(instance.content.position);
        var childPos = (Vector2)instance.transform.InverseTransformPoint(child.position);
        var endPos = contentPos - childPos;
        // If no horizontal scroll, then don't change contentPos.x
        if (!instance.horizontal)
        {
            endPos.x = contentPos.x;
            if(instance.content.anchorMin.y == 0)
                endPos.y += Screen.height / 2 + yOffset;
        }
        // If no vertical scroll, then don't change contentPos.y
        if (!instance.vertical) endPos.y = contentPos.y;

        return endPos;
    }
}

public static class RendererExtensions
{
    /// <summary>
    /// Counts the bounding box corners of the given RectTransform that are visible from the given Camera in screen space.
    /// </summary>
    /// <returns>The amount of bounding box corners that are visible from the Camera.</returns>
    /// <param name="rectTransform">Rect transform.</param>
    /// <param name="camera">Camera.</param>
    private static int CountCornersVisibleFrom(this RectTransform rectTransform, Camera camera)
    {
        Rect screenBounds = new Rect(0f, 0f, Screen.width, Screen.height); // Screen space bounds (assumes camera renders across the entire screen)
        Vector3[] objectCorners = new Vector3[4];
        rectTransform.GetWorldCorners(objectCorners);

        int visibleCorners = 0;
        Vector3 tempScreenSpaceCorner; // Cached
        for (var i = 0; i < objectCorners.Length; i++) // For each corner in rectTransform
        {
            tempScreenSpaceCorner = camera.WorldToScreenPoint(objectCorners[i]); // Transform world space position of corner to screen space
            if (screenBounds.Contains(tempScreenSpaceCorner)) // If the corner is inside the screen
            {
                visibleCorners++;
            }
        }
        return visibleCorners;
    }

    /// <summary>
    /// Determines if this RectTransform is fully visible from the specified camera.
    /// Works by checking if each bounding box corner of this RectTransform is inside the cameras screen space view frustrum.
    /// </summary>
    /// <returns><c>true</c> if is fully visible from the specified camera; otherwise, <c>false</c>.</returns>
    /// <param name="rectTransform">Rect transform.</param>
    /// <param name="camera">Camera.</param>
    public static bool IsFullyVisibleFrom(this RectTransform rectTransform, Camera camera)
    {
        return CountCornersVisibleFrom(rectTransform, camera) == 4; // True if all 4 corners are visible
    }

    /// <summary>
    /// Determines if this RectTransform is at least partially visible from the specified camera.
    /// Works by checking if any bounding box corner of this RectTransform is inside the cameras screen space view frustrum.
    /// </summary>
    /// <returns><c>true</c> if is at least partially visible from the specified camera; otherwise, <c>false</c>.</returns>
    /// <param name="rectTransform">Rect transform.</param>
    /// <param name="camera">Camera.</param>
    public static bool IsVisibleFrom(this RectTransform rectTransform, Camera camera)
    {
        return CountCornersVisibleFrom(rectTransform, camera) > 0; // True if any corners are visible
    }
}

public static class CoTaskHelper
{


    public static async Task ExecuteCoroutineAsync(this MonoBehaviour monoBehavior, IEnumerator coroutine)
    {
        var tcs = new System.Threading.Tasks.TaskCompletionSource<object>();

        monoBehavior.StartCoroutine( tempCoroutine( coroutine, () => tcs.TrySetResult(null)));

        await tcs.Task;
    }



    static IEnumerator tempCoroutine(IEnumerator coro, System.Action afterExecutionCallback)
    {
        yield return coro;
        afterExecutionCallback();
    }



    public static IEnumerator WaitAsCoroutine(this Task task)
    {
        yield return new WaitUntil(() => task.IsCompleted || task.IsFaulted || task.IsCanceled);
    }

}

public static class AnimatorExtensions
{
    public static AnimationClip FindAnimation(this Animator animator, string name)
    {
        foreach (AnimationClip clip in animator.runtimeAnimatorController.animationClips)
        {
            if (clip.name == name)
            {
                return clip;
            }
        }

        return null;
    }
}

public static class DateTimeExtensions
{
    public static string GetTimeText(int seconds)
    {
        var ts = TimeSpan.FromSeconds(seconds);
        if (ts.Days > 0) return $"{ts.Days}d {ts.Hours}h {ts.Minutes}m";
        else
        {
            if (ts.Hours > 0) return $"{ts.Hours}h {ts.Minutes}m {ts.Seconds}s";
            else
            {
                if (ts.Minutes > 0) return $"{ts.Minutes}m {ts.Seconds}s";
                else return $"{ts.Seconds}s";
            }
        }
    }
}

public static class ListExtensions
{
    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = UnityEngine.Random.Range(0, n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}