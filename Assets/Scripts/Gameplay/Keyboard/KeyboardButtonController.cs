using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using MC.Utility;

namespace MC.Modules.Keyboard
{
    public enum ButtonType
    {
        Consonant,
        Vowel,
        Pass,
        Backspace,
        Back,
        Guess
    }

    //[RequireComponent(typeof(Button))]
    public class KeyboardButtonController : MonoBehaviour
    {
        public delegate void KeyboardButtonControllerEvent(KeyboardButtonController buttonController);
        public event KeyboardButtonControllerEvent OnPressLetterEvent;
        public event KeyboardButtonControllerEvent OnEnableEvent;
        public event KeyboardButtonControllerEvent OnDisableEvent;

        [field: SerializeField] public ButtonType buttonType { get; private set; } = ButtonType.Consonant;
        [field: SerializeField] public LetterObj LetterObject { get; private set; }

        [SerializeField] Button _button;
        [SerializeField] Image _containerIcon;
        [SerializeField] TextMeshProUGUI _containerText;
        [SerializeField] Color _disabledLetterTextColor;
        
        Color defaultTextColor;

        private void Awake()
        {
            defaultTextColor = _containerText.color;
            _button.onClick.AddListener(OnPressLetter);
        }

        private void OnEnable()
        {
            OnEnableEvent?.Invoke(this);

            // refresh visuals
            bool On = GetComponent<Button>().interactable;
            UpdateVisuals(On);
        }

        public void ActivateLetter(bool ON, bool force = false)
        {            
            if (ON && !force) return;

            //Debug.Log($"EnableLetter: {letterObj.letter} newState: {ON} ShouldBeInactive: {ShouldBeInactive}");
            EnableLetter(true);
            GetComponent<Button>().interactable = ON;
            UpdateVisuals(ON);
        }

        public void EnableLetter(bool ON)
        {
            gameObject.SetActive(ON);
        }

        public void OnPressLetter()
        {
            OnPressLetterEvent?.Invoke(this);
        }

        void UpdateVisuals(bool ON)
        {
            _containerText.color = ON ? defaultTextColor : _disabledLetterTextColor;
        }

        private void OnDisable()
        {
            OnDisableEvent?.Invoke(this);
        }

#if UNITY_EDITOR
        [ContextMenu("Rename")]
        public void RenameObject()
        {
            LetterObject = new LetterObj(LetterObject.Letter);
            if (LetterObject.LetterType == LetterType.Vowel || LetterObject.LetterType == LetterType.Consonant
                    || LetterObject.LetterType == LetterType.Numeric || LetterObject.LetterType == LetterType.SpecialChar)
            {
                _containerText.SetText(LetterObject.UpperLetterChar.ToString());
                gameObject.name = "KeyboardLetter_" + _containerText.text;
                UnityEditor.EditorUtility.SetDirty(this);
            }
        }
#endif      
    }
}