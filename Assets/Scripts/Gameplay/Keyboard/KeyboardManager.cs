using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace MC.Modules.Keyboard
{
    public class KeyboardManager : MonoBehaviour
    {
        [field: SerializeField] public KeyboardLanguageEnum KeyboardLanguage { get; private set; }
        [field: SerializeField] public List<Letter> ConsonantLetters { get; private set; } = new();
        [field: SerializeField] public List<Letter> VowelLetters { get; private set; } = new();
        [field: SerializeField] public KeyboardButtonSizeFitter[] KeyboardButtonSizeFitters { get; private set; }
        [field: SerializeField] public KeyboardButtonController[] KeyboardButtonControllers { get; private set; }

        Action _playButtonSound = null;


        private void OnValidate()
        {
            GetButtons();
        }



        public void Init(Action<KeyboardButtonController> buttonOnClickAction, Action playButtonSound, Letter[] lettersToShow = null)
        {
            _playButtonSound = playButtonSound;
            StartCoroutine(InitCor());

            IEnumerator InitCor()
            {
                yield return new WaitForEndOfFrame();
                for (int i = 0; i < KeyboardButtonSizeFitters.Length; i++)
                {
                    KeyboardButtonSizeFitters[i].AdjustSize();
                }
                for (int i = 0; i < KeyboardButtonControllers.Length; i++)
                {
                    KeyboardButtonControllers[i].OnPressLetterEvent += buttonOnClickAction.Invoke;
                }
                ShowLetters(lettersToShow);
            }
        }

        public void ShowLetters(Letter[] lettersToBeShown = null)
        {
            // if no letters are not provided, show all letters. otherwise, show only the specified letters
            if (lettersToBeShown == null || lettersToBeShown.Length == 0)
            {
                Array.ForEach(KeyboardButtonControllers, button => button.gameObject.SetActive(true));
                return;
            }

            // Set the letters of the buttons to the given letters
            foreach (var item in KeyboardButtonControllers)
            {
                if (Array.Exists(lettersToBeShown, letter => letter == item.LetterObject.Letter))
                {
                    item.gameObject.SetActive(true);
                }
                else
                {
                    item.gameObject.SetActive(false);
                }
            }
        }

        void GetButtons()
        {
            KeyboardButtonControllers = transform.GetComponentsInChildren<KeyboardButtonController>(includeInactive: true);
            KeyboardButtonSizeFitters = transform.GetComponentsInChildren<KeyboardButtonSizeFitter>(includeInactive: true);
            // collect consonant and vowel letters from button controllers
            ConsonantLetters.Clear();
            VowelLetters.Clear();
            foreach (var item in KeyboardButtonControllers)
            {
                if (item.LetterObject.LetterType == LetterType.Consonant) ConsonantLetters.Add(item.LetterObject.Letter);
                if (item.LetterObject.LetterType == LetterType.Vowel) VowelLetters.Add(item.LetterObject.Letter);
            }
        }
    }
}
