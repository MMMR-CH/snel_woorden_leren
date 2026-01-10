using MC.Modules.Keyboard;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SWL
{
    public class AnswerFrame : MonoBehaviour
    {
        [field: SerializeField] public string CurrentAnswer { get; private set; }
        [field: SerializeField] public List<Letter> InitialAnswerLetters { get; private set; }
        [field: SerializeField] public List<Letter> CurrentAnswerLetters { get; private set; }
        [field: SerializeField] public bool Locked { get; private set; } = false;


        [SerializeField] TextMeshProUGUI answerText;
        [SerializeField] Image bgImage;


        Letter[] _requiredAnswer = null;
        LetterSet _letterSet;
        int currentWritingIndex = 0;
        Color? defaultBGColor;

        public void SetLevel(Letter[] requiredAnswer, Letter[] initialAnswer, LetterSet letterSet)
        {
            SWL_Debug.Log($"AnswerFrame => Initializing AnswerFrame with required answer: {string.Join("", requiredAnswer)} and initial answer: {string.Join("", initialAnswer)}");
            defaultBGColor ??= bgImage.color; // Store the default background color if not already set
            currentWritingIndex = 0;
            CurrentAnswer = string.Empty;
            CurrentAnswerLetters = initialAnswer.ToList();
            InitialAnswerLetters = initialAnswer.ToList();
            this._requiredAnswer = requiredAnswer;
            this._letterSet = letterSet;
            UpdateCurrentAnswer();
        }

        public void AddLetter(Letter letter)
        {
            SWL_Debug.Log($"AnswerFrame => Adding letter: {letter} to answer frame. Current writing index: {currentWritingIndex}");
            // Check if the answer frame is locked
            if (Locked)
            {
                SWL_Debug.Log("AnswerFrame => Cannot add letter, answer frame is locked.");
                return;
            }

            if (CurrentAnswerLetters == null)
            {
                currentWritingIndex = 0;
                CurrentAnswerLetters = new List<Letter>();
            }

            // Check if the current writing index is out of bounds 
            if (currentWritingIndex >= _requiredAnswer.Length)
            {
                UpdateCurrentAnswer();
                return;
            }

            // if the next required letter is space, shift next letter index
            while (InitialAnswerLetters[currentWritingIndex] != Letter.Underline)
            {
                CurrentAnswerLetters[currentWritingIndex] = InitialAnswerLetters[currentWritingIndex];
                currentWritingIndex++;
                AddLetter(letter);
                return;
            }

            CurrentAnswerLetters[currentWritingIndex] = letter;
            currentWritingIndex++;
            UpdateCurrentAnswer();
        }

        public void Backspace()
        {
            SWL_Debug.Log($"AnswerFrame => Backspacing. Current writing index: {currentWritingIndex}");
            // Check if the answer frame is locked
            if (Locked)
            {
                SWL_Debug.Log("AnswerFrame => Cannot add letter, answer frame is locked.");
                return;
            }

            if (CurrentAnswerLetters == null)
            {
                currentWritingIndex = 0;
                CurrentAnswerLetters = new List<Letter>();
            }

            currentWritingIndex--;

            // Check if the current writing index is out of bounds
            if (currentWritingIndex < 0)
            {
                currentWritingIndex = 0;
                UpdateCurrentAnswer();
                return;
            }

            // if the next required letter is space, shift next letter index
            while (InitialAnswerLetters[currentWritingIndex] != Letter.Underline)
            {
                CurrentAnswerLetters[currentWritingIndex] = InitialAnswerLetters[currentWritingIndex];
                Backspace();
                return;
            }

            CurrentAnswerLetters[currentWritingIndex] = Letter.Underline;
            UpdateCurrentAnswer();
        }

        public bool CheckAnswer()
        {
            SWL_Debug.Log($"AnswerFrame ==> CheckAnswer ==> CurrentAnswer: {CurrentAnswer}, RequiredAnswer: {string.Join("", _requiredAnswer)}");
            if (CurrentAnswerLetters == null || _requiredAnswer == null || CurrentAnswerLetters.Count != _requiredAnswer.Length)
            {
                SWL_Debug.Log("AnswerFrame ==> CheckAnswer ==> Current answer letters or required answer is null or lengths do not match.");
                return false;
            }
            // Check if the answer frame is locked
            if (Locked)
            {
                SWL_Debug.Log("AnswerFrame => Cannot add letter, answer frame is locked.");
                return false;
            }

            // Check if the current answer matches the required answer
            for (int i = 0; i < _requiredAnswer.Length; i++)
            {
                //SWL_Debug.Log($"AnswerFrame ==> CheckAnswer ==> Required: {_requiredAnswer[i]}, Current: {CurrentAnswerLetters[i]}");
                if (_requiredAnswer[i] != CurrentAnswerLetters[i])
                {
                    SWL_Debug.Log($"AnswerFrame ==> CheckAnswer ==> Answer does not match at index {i}. Required: {_requiredAnswer[i]}, Current: {CurrentAnswerLetters[i]}");
                    return false;
                }
            }
            return true;
        }

        public bool IsAnswerComplete()
        {
            SWL_Debug.Log($"AnswerFrame ==> IsAnswerComplete ==> CurrentAnswerLetters.Count: {CurrentAnswerLetters?.Count}, RequiredAnswer.Length: {_requiredAnswer?.Length}");
            // Check if the current answer letters count matches the required answer length
            return CurrentAnswerLetters != null && CurrentAnswerLetters.Count >= _requiredAnswer.Length;
        }

        public void SetLocked(bool locked)
        {
            SWL_Debug.Log($"AnswerFrame => Setting locked state to: {locked}");
            Locked = locked;
            if (bgImage != null)
            {
                bgImage.color = (locked && ColorUtility.TryParseHtmlString("#8CC924", out Color color)) ? color : defaultBGColor.Value; // Change color based on locked state
            }
            else
            {
                Debug.LogWarning("AnswerFrame => Background Image is not assigned in the AnswerFrame component.");
            }
        }

        public void RevealRandomLetter()
        {
            // find a random letter in the InitialAnswerLetters that is underline
            SWL_Debug.Log("AnswerFrame => Revealing a random letter.");
            if (CurrentAnswerLetters == null || CurrentAnswerLetters.Count == 0)
            {
                SWL_Debug.LogWarning("AnswerFrame => InitialAnswerLetters is null or empty.");
                return;
            }

            List<int> underlineIndices = new List<int>();
            for (int i = 0; i < CurrentAnswerLetters.Count; i++)
            {
                if (CurrentAnswerLetters[i] == Letter.Underline)
                {
                    underlineIndices.Add(i);
                }
            }
            if (underlineIndices.Count == 0)
            {
                SWL_Debug.LogWarning("AnswerFrame => No underline letters found in InitialAnswerLetters.");
                return;
            }
            int randomIndex = UnityEngine.Random.Range(0, underlineIndices.Count);
            int letterIndex = underlineIndices[randomIndex];
            InitialAnswerLetters[letterIndex] = _requiredAnswer[letterIndex];
            CurrentAnswerLetters[letterIndex] = _requiredAnswer[letterIndex];
            UpdateCurrentAnswer();
            SWL_Debug.Log($"AnswerFrame => Revealed letter: {_requiredAnswer[letterIndex]} at index {letterIndex}");
        }

        public void RevealFullAnswer()
        {
            SWL_Debug.Log("AnswerFrame => Revealing all letters.");
            if (InitialAnswerLetters == null || InitialAnswerLetters.Count == 0)
            {
                SWL_Debug.LogWarning("AnswerFrame => InitialAnswerLetters is null or empty.");
                return;
            }
            for (int i = 0; i < InitialAnswerLetters.Count; i++)
            {
                if (InitialAnswerLetters[i] == Letter.Underline)
                {
                    InitialAnswerLetters[i] = _requiredAnswer[i];
                    CurrentAnswerLetters[i] = _requiredAnswer[i];
                }
            }
            UpdateCurrentAnswer();
        }

        public void ClearCurrentAnswer()
        {
            currentWritingIndex = 0;
            CurrentAnswerLetters = InitialAnswerLetters.ToList();
            UpdateCurrentAnswer();
        }




        void UpdateCurrentAnswer()
        {
            if (CurrentAnswerLetters != null && CurrentAnswerLetters.Count > 0)
            {
                char[] currentText = new char[CurrentAnswerLetters.Count];
                for (int i = 0; i < CurrentAnswerLetters.Count; i++)
                {
                    //SWL_Debug.Log($"AnswerFrame ==> UpdateCurrentAnswer ==> currentText[{i}]: {_letterSet.LetterObjectsDictionary[CurrentAnswerLetters[i]]}");
                    currentText[i] = _letterSet.LetterObjectsDictionary[CurrentAnswerLetters[i]].UpperLetterChar;
                }
                CurrentAnswer = new string(currentText);
            }
            else CurrentAnswer = string.Empty;
            answerText.SetText(CurrentAnswer);
        }
    }
}
