using DG.Tweening;
using MC.Modules.Keyboard;
using MC.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SWL
{
    /// <summary>
    /// Canvas for the Betekenis level.
    /// </summary>
    /// <remarks>
    /// This class is responsible for managing the Betekenis level UI elements.
    /// </remarks>
    [RequireComponent(typeof(Canvas))]
    public class FITBCanvas : MonoBehaviour
    {
        [SerializeField] SerializableDictionary<KeyboardLanguageEnum, KeyboardManager> KeyboardPrefabs;
        [SerializeField] RectTransform keyboardContainer;
        [SerializeField] AnswerFrame answerFrame;
        [SerializeField] FITBDescription fitbDescription;
        [SerializeField] private QuestionFrame questionFrame;
        [SerializeField] Button previousButton;
        [SerializeField] Button nextButton;
        [SerializeField] Button homeButton;
        [SerializeField] FITBGuessResultScreen fitbGuessResultScreen;
        [SerializeField] FITBBoosterController fitbBoosterController;
        [SerializeField] ProgressBar progressBar;

        KeyboardManager _keyboardManagerInstance;
        FITBMainBus _levelBus = null;
        LetterSet _currentLetterSet;
        WoordData _currentWoordData;
        KeyboardButtonController _guessButton;

        public void Init(FITBMainBus levelBus, LetterSet letterSet)
        {
            SWL_Debug.Log($"BetekenisLevelCanvas ==> Init");

            _levelBus = levelBus;
            _currentLetterSet = letterSet;

            _levelBus.SaveCurrentWoordTempData = SaveCurrentWoordTempData;
            _levelBus.MoveToNextWord = MoveToNextWord;
            _levelBus.MoveToPreviousWord = MoveToPreviousWord;

            // Initialize keyboard
            // Convert CultureInfo to KeyboardLanguageEnum
            // Check if the keyboard prefab exists for the given language
            if (KeyboardPrefabs.TryGetValue(letterSet.KeyboardLanguage, out KeyboardManager keyboardPrefab))
            {
                // Instantiate the keyboard prefab and set it as a child of the container
                _keyboardManagerInstance = Instantiate(keyboardPrefab, keyboardContainer);
                _keyboardManagerInstance.Init(OnKeyboardButtonClicked, levelBus.OnPlayButtonSound, lettersToShow: null);
            }
            else Debug.LogError($"No keyboard prefab found for language: {letterSet.KeyboardLanguage}"); 

            // subscribe to input manager event
            _levelBus.OnInputReceived += OnInputReceived;

            // init answer frame
            _levelBus.BackspaceAnswer = answerFrame.Backspace;
            _levelBus.AddAnswerChar = answerFrame.AddLetter;
            _levelBus.CheckAnswer = answerFrame.CheckAnswer;
            _levelBus.RevealRandomLetter = answerFrame.RevealRandomLetter;
            _levelBus.RevealFullAnswer = answerFrame.RevealFullAnswer;
            _levelBus.ClearCurrentAnswer = answerFrame.ClearCurrentAnswer;

            // init level control buttons
            previousButton.onClick.AddListener(_levelBus.MoveToPreviousWord.Invoke);
            nextButton.onClick.AddListener(_levelBus.MoveToNextWord.Invoke);
            homeButton.onClick.AddListener(() =>
            {
                SWL_Debug.Log("BetekenisLevelCanvas ==> homeButton clicked");
                _levelBus.OnPlayButtonSound();
                LevelManager.LoadScene(LevelManager.LevelType.MainMenu);
            });

            // description text
            fitbDescription.Init(_levelBus);
            _levelBus.ToggleEnglishMeaning = ToggleMeaning;

            // init booster frame
            fitbBoosterController.Init(_levelBus);

            // progress bar
            _levelBus.UpdateProgressBar = progressBar.UpdateBar;
        }

        WoordData _lastWoord;
        public void InitLevel(WoordData woord)
        {
            // Initialize the question frame
            questionFrame.SetQuestionText(woord.VOORBEELD_ZIN_4);
            
            // Initialize the answer frame
            _currentWoordData = woord;
            var answer = woord.VOORBEELD_ZIN_4_ANSWER_Letters;
            
            // If the word is already played or completed, show the answer directly
            Letter[] _initialAnswer = new Letter[answer.Length];
            if (woord.IsWoordCompleted)
            {
                _initialAnswer = answer; 
                woord.SetInitialLetters(_initialAnswer);
            }
            // else display fully '_' chars
            else if (woord.InitialLetters == null)
            {
                for (int i = 0; i < answer.Length; i++)
                {
                    if (answer[i] == Letter.Space) _initialAnswer[i] = Letter.Space;
                    else _initialAnswer[i] = Letter.Underline;
                }
                woord.SetInitialLetters(_initialAnswer);
            }
            
            // set finally the letters on answer text
            if (_lastWoord != woord)
            {
                _lastWoord = woord;
                answerFrame.SetLevel(initialAnswer: woord.InitialLetters, requiredAnswer: answer, letterSet: _currentLetterSet);
                answerFrame.SetLocked(woord.IsWoordCompleted); // Lock the answer frame if the word is completed
            }

            // Initialize the description text
            InitDescriptionText(woord);

            // init booster panel
            _levelBus.ShowNextButton(woord.IsWoordCompleted);
        }

        void InitDescriptionText(WoordData woord)
        {
            _levelBus.ClearDescriptionText();
            _levelBus.AddDescriptionText(woord.BETEKENIS, 1);
            if (woord.IsWoordCompleted) // show all answers if the woord is completed
            {
                for (int i = 0; i < GameConstants.MAX_SAMPLE_SENTENCES; i++)
                {
                    woord.IncreaseSampleSentenceCount();
                }
            }
            if (woord.RevealedSampleSentenceCount > 0) _levelBus.AddSampleSentence(1, woord.VOORBEELD_ZIN_1);
            if (woord.RevealedSampleSentenceCount > 1) _levelBus.AddSampleSentence(2, woord.VOORBEELD_ZIN_2);
            if (woord.RevealedSampleSentenceCount > 2) _levelBus.AddSampleSentence(3, woord.VOORBEELD_ZIN_3);

            // init result screen
            if (woord.IsWoordCompleted) fitbGuessResultScreen.ShowCorrectResult();
            else fitbGuessResultScreen.ResetResultScreen();
        }

        void OnKeyboardButtonClicked(KeyboardButtonController controller)
        {
            //SWL_Debug.Log($"BetekenisLevelCanvas ==> OnKeyboardButtonClicked: {controller.LetterObject.UpperLetterChar}");

            // if the button is not interactable, do nothing
            if (!controller.GetComponent<Button>().interactable) return;

            // Play button sound
            _levelBus.OnPlayButtonSound?.Invoke();

            // button animation
            controller.transform.DOKill();
            controller.transform.DOScale(Vector3.one * 1.1f, 0.1f).OnComplete(() =>
            {
                controller.transform.DOScale(Vector3.one, 0.1f);
            });

            // Null check for _keyboardManagerInstance
            if (_keyboardManagerInstance == null)
            {
                Debug.LogError("KeyboardManager instance is null.");
                return;
            }

            // Handle the button click based on its type
            switch (controller.buttonType)
            {
                case ButtonType.Consonant:
                case ButtonType.Vowel:
                    _levelBus.AddAnswerChar(controller.LetterObject.Letter);
                    EnableGuessButtonIfAnswerIsFull();
                    break;
                case ButtonType.Backspace:
                    _levelBus.BackspaceAnswer();
                    EnableGuessButtonIfAnswerIsFull();
                    break;
                case ButtonType.Guess:
                    bool correct = _levelBus.CheckAnswer();
                    if (correct)
                    {
                        SWL_Debug.Log("BetekenisLevelCanvas ==> Guess is correct!");
                        _levelBus.CompleteWord(_currentWoordData);
                        _levelBus.ShowNextButton(true);
                        fitbGuessResultScreen.ShowCorrectResult(skip: true, null);
                        InitDescriptionText(_currentWoordData);
                        answerFrame.SetLocked(true); // Lock the answer frame after a correct guess
                        
                        // Create the full answer string and set question frame
                        char[] _initialAnswer = new  char[_currentWoordData.VOORBEELD_ZIN_4_ANSWER_Letters.Length];
                        for (int i = 0; i < _currentWoordData.VOORBEELD_ZIN_4_ANSWER_Letters.Length; i++)
                        {
                            if (_currentWoordData.VOORBEELD_ZIN_4_ANSWER_Letters[i] == Letter.Space)
                                _initialAnswer[i] = _currentLetterSet.LetterObjectsDictionary[Letter.Space]
                                    .UpperLetterChar;
                            else
                                _initialAnswer[i] = _currentLetterSet.LetterObjectsDictionary[Letter.Underline]
                                    .UpperLetterChar;
                        }
                        string answer = _currentWoordData.VOORBEELD_ZIN_4;
                        answer = answer.Replace(new string(_initialAnswer),  _currentWoordData.VOORBEELD_ZIN_4_ANSWER);
                        questionFrame.SetQuestionText(answer);
                    }
                    else
                    {
                        SWL_Debug.Log("BetekenisLevelCanvas ==> Guess is incorrect.");
                        fitbGuessResultScreen.ShowIncorrectResult();
                        _levelBus.ClearCurrentAnswer();
                    }
                    break;
                case ButtonType.Pass:
                    // Handle pass logic if needed
                    break;
                case ButtonType.Back:
                    // Handle back logic if needed
                    break;
                default:
                    Debug.LogWarning($"Unhandled button type: {controller.buttonType}");
                    break;
            }

            void EnableGuessButtonIfAnswerIsFull()
            {
                if (_keyboardManagerInstance == null)
                {
                    Debug.LogError("KeyboardManager instance is null in EnableGuessButtonIfAnswerIsFull.");
                    return;
                }
                _guessButton ??= _keyboardManagerInstance.KeyboardButtonControllers.FirstOrDefault(b => b.buttonType == ButtonType.Guess);
                if (_guessButton != null)
                {
                    _guessButton.ActivateLetter(answerFrame.IsAnswerComplete());
                }
                else
                {
                    Debug.LogWarning("Guess button not found in KeyboardButtonControllers.");
                }
            }
        }

        void OnInputReceived(Letter letter)
        {
            // Handle input received from InputManager if needed

            if (letter == Letter.Space)
            {
                // Space is not used in Betekenis levels
                return;
            }
            if (letter == Letter.Null)
            {
                _levelBus.BackspaceAnswer();
                return;
            }
            if (letter == Letter._1)
            {
                // Use first booster
                fitbBoosterController.OnHintBoosterPressed();
                return;
            }
            if (letter == Letter._2)
            {
                // Use second booster
                fitbBoosterController.OnAnswerBoosterPressed();
                return;
            }
            if (letter == Letter._3)
            {
                // Use third booster
                fitbBoosterController.OnSampleBoosterPressed();
                return;
            }
            if (letter == Letter._4)
            {
                // Toggle English meaning
                fitbBoosterController.OnEnglishButtonPressed();
                return;
            }
            if (letter == Letter._0 || letter == Letter.NewLine)
            {
                // if the answer is correct, move to next word. otherwise, check the answer
                if (_currentWoordData.IsWoordCompleted)
                    fitbBoosterController.OnNextButtonPressed();
                else
                {
                    // Check answer or move to next word
                    var guessButton = _keyboardManagerInstance.KeyboardButtonControllers.FirstOrDefault(b => b.buttonType == ButtonType.Guess);
                    if (guessButton != null) OnKeyboardButtonClicked(guessButton);
                }
                return;
            }            

            // Find the keyboard button corresponding to the letter and simulate a click
            if (_keyboardManagerInstance == null)
            {
                Debug.LogError("KeyboardManager instance is null in OnInputReceived.");
                return;
            }

            //Debug.Log($"BetekenisLevelCanvas ==> OnInputReceived: {letter}");

            // Find the keyboard button corresponding to the letter and simulate a click
            var button = _keyboardManagerInstance.KeyboardButtonControllers.FirstOrDefault(b => b.LetterObject.Letter == letter);
            if (button != null) OnKeyboardButtonClicked(button);
            Debug.LogWarning($"No keyboard button found for letter: {letter}");
        }

        void ToggleMeaning(WoordData woord)
        {            
            _levelBus.ClearDescriptionText();
            if (woord.CurrentDescriptionLanguage == SWL_LanguagesEnum.Dutch)
            {
                woord.SetCurrentDescriptionLanguage(SWL_LanguagesEnum.English);
                _levelBus.AddDescriptionText(woord.MEANING, 1);
            }
            else
            {
                woord.SetCurrentDescriptionLanguage(SWL_LanguagesEnum.Dutch);
                _levelBus.AddDescriptionText(woord.BETEKENIS, 1);
            }
            
            if (woord.RevealedSampleSentenceCount > 0) _levelBus.AddSampleSentence(1, woord.VOORBEELD_ZIN_1);
            if (woord.RevealedSampleSentenceCount > 1) _levelBus.AddSampleSentence(2, woord.VOORBEELD_ZIN_2);
            if (woord.RevealedSampleSentenceCount > 2) _levelBus.AddSampleSentence(3, woord.VOORBEELD_ZIN_3);
        }

        void SaveCurrentWoordTempData()
        {
            // Save the current word's temporary data
            _currentWoordData.SetInitialLetters(answerFrame.InitialAnswerLetters.ToArray());
        }

        void MoveToNextWord()
        {
            //SWL_Debug.Log("BetekenisLevelCanvas ==> Next button clicked");
            _levelBus.OnPlayButtonSound();
            _currentWoordData = _levelBus.GetNextWord();
            InitLevel(_currentWoordData);
        }

        void MoveToPreviousWord()
        {
            //SWL_Debug.Log("BetekenisLevelCanvas ==> Previous button clicked");
            _levelBus.OnPlayButtonSound();
            _currentWoordData = _levelBus.GetPreviousWord();
            InitLevel(_currentWoordData);
        }
    }


    public static class CultureInfoExtensions
    {
        /// <summary>
        /// Converts a CultureInfo object to a KeyboardLanguageEnum.
        /// </summary>
        /// <param name="cultureInfo">The CultureInfo to convert.</param>
        /// <returns>The corresponding KeyboardLanguageEnum.</returns>
        public static KeyboardLanguageEnum ToKeyboardLanguageEnum(this CultureInfo cultureInfo)
        {
            // Map CultureInfo to KeyboardLanguageEnum
            return cultureInfo.TwoLetterISOLanguageName switch
            {
                "nl" => KeyboardLanguageEnum.Dutch,
                "en" => KeyboardLanguageEnum.English,
                "de" => KeyboardLanguageEnum.German,
                "fr" => KeyboardLanguageEnum.French,
                "es" => KeyboardLanguageEnum.Spanish,
                _ => throw new System.NotSupportedException($"Culture '{cultureInfo.Name}' is not supported.")
            };
        }

        public static KeyboardLanguageEnum ToKeyboardLanguageEnum(this SWL_LanguagesEnum lang)
        {
            // Map CultureInfo to KeyboardLanguageEnum
            return lang switch
            {
                SWL_LanguagesEnum.Dutch => KeyboardLanguageEnum.Dutch,
                SWL_LanguagesEnum.English => KeyboardLanguageEnum.English,
                SWL_LanguagesEnum.German => KeyboardLanguageEnum.German,
                SWL_LanguagesEnum.French => KeyboardLanguageEnum.French,
                SWL_LanguagesEnum.Spanish => KeyboardLanguageEnum.Spanish,
                SWL_LanguagesEnum.Turkish => KeyboardLanguageEnum.Turkish,
                _ => throw new System.NotSupportedException($"SWL_LanguagesEnum '{lang}' is not supported.")
            };
        }
    }


}
