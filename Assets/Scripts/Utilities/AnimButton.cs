using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using MC.Utility;

namespace MC.Utility
{
    /// <summary>
    /// A standard button that sends an event when clicked.
    /// </summary>
    [AddComponentMenu("UI/AnimButton")]
    public class AnimButton : UnityEngine.UI.Button
    {
        public enum AnimType
        {
            Squeeze_Shake,
            Shrink,
            Expand,
        }

        public bool IsAnimating => animating;
        public bool CanAnimate { get; set; } = true;
        [SerializeField] AnimType animType = AnimType.Shrink;
        [SerializeField] bool waitForAnim = true, colorAnim = true;
        [SerializeField] Sprite pressedSprite;
        [SerializeField] Image targetImage;
        [SerializeField] RectTransform animationRootTransform;
        [SerializeField] List<Graphic> excludeOnColorAnimation = new List<Graphic>();

        [HideInInspector] public UnityEvent normalStateEvent;
        [HideInInspector] public UnityEvent highlightedStateEvent;
        [HideInInspector] public UnityEvent pressedStateEvent;
        [HideInInspector] public UnityEvent selectedStateEvent;
        [HideInInspector] public UnityEvent disabledStateEvent;

        Vector3? tempScale = null;
        SelectionState previousState;
        bool animating;
        Sprite normalSprite;

        protected override void Start()
        {
            base.Start();

            if (targetGraphic is Image img)
            {
                if (targetImage == null) targetImage = img;
                normalSprite = targetImage.sprite;
            }
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left) return;
            Press();
        }

        private void Press()
        {
            if (!IsActive() || !IsInteractable())
                return;

            UISystemProfilerApi.AddMarker("Button.onClick", this);
            onClick?.Invoke();
        }

        public void SetNormalState()
        {
            DoStateTransition(SelectionState.Normal, false);
        }

        public void SetSprite(Sprite _sprite)
        {
            if (targetImage != null)
            {
                targetImage.sprite = _sprite;
                normalSprite = _sprite;
            }
        }

        /// <summary>
        /// Transition the Selectable to the entered state.
        /// </summary>
        /// <param name="state">State to transition to</param>
        /// <param name="instant">Should the transition occur instantly.</param>
        protected override void DoStateTransition(SelectionState state, bool instant)
        {
            if (!gameObject.activeInHierarchy)
                return;

            UnityEvent transitionEvent;
            if (animationRootTransform == null) animationRootTransform = GetComponent<RectTransform>();
            if (tempScale == null) tempScale = animationRootTransform.localScale;
            if (tempScale.Value == Vector3.zero) tempScale = new Vector3(1f, 1f, 1f);

            if (animType == AnimType.Squeeze_Shake) animationRootTransform.pivot = new Vector2(0.5f, 0f);
            else if (animType == AnimType.Expand) animationRootTransform.pivot = new Vector2(0.5f, 0.5f);
            else if (animType == AnimType.Shrink) animationRootTransform.pivot = new Vector2(0.5f, 0.5f);

            switch (state)
            {
                case SelectionState.Normal:
                    transitionEvent = normalStateEvent;
                    NormalStateAnim();
                    break;
                case SelectionState.Highlighted:
                    transitionEvent = highlightedStateEvent;
                    break;
                case SelectionState.Pressed:
                    PressedAnim();
                    transitionEvent = pressedStateEvent;
                    break;
                case SelectionState.Selected:
                    SelectedAnim();
                    transitionEvent = selectedStateEvent;
                    break;
                case SelectionState.Disabled:
                    DisabledAnim();
                    transitionEvent = disabledStateEvent;
                    break;
                default:
                    transitionEvent = normalStateEvent;
                    break;
            }
            transitionEvent?.Invoke();
            previousState = state;
        }

        void NormalStateAnim()
        {
            if (CanAnimate)
            {
                animationRootTransform.DOKill();
                targetGraphic.DOKill();
                animating = true;
                ChangeGraphicsColor(colors.normalColor, colors.fadeDuration);
                animationRootTransform.DOScale(tempScale.Value, 0.15f).OnComplete(delegate
                {
                    animating = false;
                });
            }
            else
            {
                ChangeGraphicsColor(colors.normalColor, 0f);
                animationRootTransform.localScale = tempScale.Value;
            }
        }

        void PressedAnim()
        {
            if (CanAnimate)
            {
                animationRootTransform.DOKill();
                targetGraphic.DOKill();
                animating = true;
                ChangeGraphicsColor(colors.pressedColor, colors.fadeDuration);

                if (targetImage != null && pressedSprite != null && normalSprite == null) normalSprite = targetImage.sprite;
                if (targetImage != null && pressedSprite != null) targetImage.sprite = pressedSprite;

                Sequence seq = DOTween.Sequence();
                if (animType == AnimType.Squeeze_Shake)
                {
                    seq.Append(animationRootTransform.DOScale(new Vector3(tempScale.Value.x * 1.15f, tempScale.Value.y * 0.75f, tempScale.Value.z), 0.1f));
                }

                if (animType == AnimType.Expand)
                {
                    seq.Append(animationRootTransform.DOScale(new Vector3(tempScale.Value.x * 1.15f, tempScale.Value.y * 1.15f, tempScale.Value.z), 0.1f));
                }

                if (animType == AnimType.Shrink)
                {
                    seq.Append(animationRootTransform.DOScale(new Vector3(tempScale.Value.x * 0.85f, tempScale.Value.y * 0.85f, tempScale.Value.z), 0.1f));
                }
                seq.OnComplete(() =>
                {
                    animating = false;
                    if (targetImage != null && pressedSprite != null && normalSprite != null) targetImage.sprite = normalSprite;
                });
            }
            else
            {
                ChangeGraphicsColor(colors.pressedColor, 0f);
                if (targetImage != null && pressedSprite != null) targetImage.sprite = pressedSprite;
                animationRootTransform.localScale = new Vector3(tempScale.Value.x * 0.85f, tempScale.Value.y * 0.85f, tempScale.Value.z);
            }
        }

        void SelectedAnim()
        {
            if (CanAnimate)
            {
                if (previousState != SelectionState.Pressed) return;
                animationRootTransform.DOKill();
                targetGraphic.DOKill();
                animating = true;
                ChangeGraphicsColor(colors.selectedColor, colors.fadeDuration);
                if (targetImage != null && normalSprite != null) targetImage.sprite = normalSprite;

                if (animType == AnimType.Squeeze_Shake)
                {
                    Sequence seq = DOTween.Sequence();
                    seq.Append(animationRootTransform.DOScale(new Vector3(tempScale.Value.x * 0.85f, tempScale.Value.y * 1.1f, tempScale.Value.z), 0.1f));
                    seq.Append(animationRootTransform.DOScale(new Vector3(tempScale.Value.x * 1f, tempScale.Value.y * 0.85f, tempScale.Value.z), 0.1f));
                    seq.Append(animationRootTransform.DOScale(new Vector3(tempScale.Value.x * 1f, tempScale.Value.y * 1f, tempScale.Value.z), 0.1f));
                    seq.OnComplete(() => animating = false);
                }

                if (animType == AnimType.Expand)
                {
                    Sequence seq = DOTween.Sequence();
                    seq.Append(animationRootTransform.DOScale(tempScale.Value * 1f, 0.15f));
                    seq.OnComplete(() => animating = false);
                }

                if (animType == AnimType.Shrink)
                {
                    Sequence seq = DOTween.Sequence();
                    seq.Append(animationRootTransform.DOScale(tempScale.Value * 1f, 0.15f));
                    seq.OnComplete(() => animating = false);
                }
            }
            else
            {
                ChangeGraphicsColor(colors.selectedColor, 0f);
                if (targetImage != null && normalSprite != null) targetImage.sprite = normalSprite;
                animationRootTransform.localScale = tempScale.Value;
            }
        }

        void DisabledAnim()
        {
            ChangeGraphicsColor(colors.disabledColor, colors.fadeDuration);
            animationRootTransform.DOScale(Vector3.one, 0.1f);
        }

        void ChangeGraphicsColor(Color newColor, float duration)
        {
            if (!colorAnim) return;
            if (CanAnimate)
            {
                targetGraphic.DOKill();
                var graphs = animationRootTransform.GetComponentsInChildren<Graphic>();
                foreach (var item in graphs)
                {
                    bool match = false;
                    if (excludeOnColorAnimation != null)
                    {
                        for (int i = 0; i < excludeOnColorAnimation.Count; i++)
                        {
                            if (item == excludeOnColorAnimation[i])
                            {
                                match = true;
                                break;
                            }
                        }
                    }
                    if (match) continue;
                    item.DOColor(newColor, duration);
                }
            }
            else
            {
                var graphs = animationRootTransform.GetComponentsInChildren<Graphic>();
                foreach (var item in graphs)
                {
                    bool match = false;
                    if (excludeOnColorAnimation != null)
                    {
                        for (int i = 0; i < excludeOnColorAnimation.Count; i++)
                        {
                            if (item == excludeOnColorAnimation[i])
                            {
                                match = true;
                                break;
                            }
                        }
                    }
                    if (match) continue;
                    item.color = newColor;
                }
            }
        }

        [ContextMenu("Reset Serialization")]
        void ResSerial()
        {
            normalSprite = null;
            tempScale = null;
            animating = false;
            previousState = SelectionState.Normal;
        }
    }
}

#if UNITY_EDITOR
namespace UnityEditor.UI
{
    [CustomEditor(typeof(AnimButton), true)]
    [CanEditMultipleObjects]
    /// <summary>
    ///   Custom Editor for the Button Component.
    ///   Extend this class to write a custom editor for a component derived from Button.
    /// </summary>
    public class AnimButtonEditor : SelectableEditor
    {
        SerializedProperty m_Interactable;
        SerializedProperty m_AnimType;
        SerializedProperty m_WaitForAnim;
        SerializedProperty m_ColorAnim;
        SerializedProperty m_Colors;
        SerializedProperty m_PressedSprite;
        SerializedProperty m_TargetImage;
        SerializedProperty m_AnimationRoot;
        SerializedProperty m_OnClickProperty;
        SerializedProperty m_TargetGraphic;
        SerializedProperty m_ExcludeColorAnim;

        protected override void OnEnable()
        {
            //base.OnEnable();

            m_Interactable = serializedObject.FindProperty("m_Interactable");
            m_AnimType = serializedObject.FindProperty("animType");
            m_Colors = serializedObject.FindProperty("m_Colors");
            m_PressedSprite = serializedObject.FindProperty("pressedSprite");
            m_TargetImage = serializedObject.FindProperty("targetImage");
            m_WaitForAnim = serializedObject.FindProperty("waitForAnim");
            m_TargetGraphic = serializedObject.FindProperty("m_TargetGraphic");
            m_AnimationRoot = serializedObject.FindProperty("animationRootTransform");
            m_ExcludeColorAnim = serializedObject.FindProperty("excludeOnColorAnimation");
            m_OnClickProperty = serializedObject.FindProperty("m_OnClick");
            m_ColorAnim = serializedObject.FindProperty("colorAnim");
        }

        public override void OnInspectorGUI()
        {
            //base.OnInspectorGUI();
            serializedObject.Update();
            EditorGUILayout.PropertyField(m_Interactable);
            EditorGUILayout.PropertyField(m_AnimType);
            EditorGUILayout.PropertyField(m_WaitForAnim);
            EditorGUILayout.PropertyField(m_ColorAnim);
            EditorGUILayout.PropertyField(m_Colors);
            EditorGUILayout.PropertyField(m_PressedSprite);
            EditorGUILayout.PropertyField(m_TargetImage);
            EditorGUILayout.PropertyField(m_TargetGraphic);
            EditorGUILayout.PropertyField(m_AnimationRoot);
            EditorGUILayout.PropertyField(m_ExcludeColorAnim);
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();


            EditorGUILayout.PropertyField(m_OnClickProperty);
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif