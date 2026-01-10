using DG.Tweening;
using MC.Utility;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace SWL
{
    public class BetekenisLevelBoosterController : MonoBehaviour
    {
        [SerializeField] RectTransform boosterPanel;
        [SerializeField] Button hintBoosterButton;
        [SerializeField] Button answerBoosterButton;
        [SerializeField] Button sampleBoosterButton;
        [SerializeField] Button englishButton;
        [SerializeField] Button nextButton;

        BetekenisLevelMainBus _levelMainBus;

        public void Init(BetekenisLevelMainBus levelMainBus)
        {
            _levelMainBus = levelMainBus;
            _levelMainBus.ShowNextButton = ShowNextButton;

            // add listeners to buttons
            hintBoosterButton.onClick.AddListener(OnHintBoosterPressed);
            answerBoosterButton.onClick.AddListener(OnAnswerBoosterPressed);
            sampleBoosterButton.onClick.AddListener(OnSampleBoosterPressed);
            englishButton.onClick.AddListener(OnEnglishButtonPressed);
            nextButton.onClick.AddListener(OnNextButtonPressed);
        }

        void ShowNextButton(bool ON)
        {       
            SWL_Debug.Log($"BetekenisLevelBoosterController ==> ShowNextButton: {ON}");     
            if (ON && !nextButton.gameObject.activeSelf)
            {
                nextButton.interactable = true;
                nextButton.transform.DOKill();
                boosterPanel.transform.DOKill();
                nextButton.gameObject.SetActive(true);
                boosterPanel.gameObject.SetActive(false);
                nextButton.transform.localScale = Vector3.zero;
                nextButton.transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack);
            }
            else if (!ON && !boosterPanel.gameObject.activeSelf)
            {
                nextButton.interactable = false;
                nextButton.transform.DOKill();
                boosterPanel.transform.DOKill();
                boosterPanel.gameObject.SetActive(true);
                boosterPanel.transform.localScale = Vector3.zero;
                nextButton.gameObject.SetActive(false);
                boosterPanel.transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack);
            }
        }

        public void OnHintBoosterPressed()
        {
            _levelMainBus.OnPlayButtonSound?.Invoke();

            // Find a random letter in the current word
            _levelMainBus.RevealRandomLetter();

            _levelMainBus.SaveCurrentWoordTempData.Invoke();
        }

        public void OnAnswerBoosterPressed()
        {
            _levelMainBus.OnPlayButtonSound?.Invoke();

            // Reveal the full answer
            _levelMainBus.RevealFullAnswer();
        }

        public void OnSampleBoosterPressed()
        {
            _levelMainBus.OnPlayButtonSound?.Invoke();

            // Check if the sample sentence is already revealed
            if (_levelMainBus.GetCurrentWord().RevealedSampleSentenceCount >= GameConstants.MAX_SAMPLE_SENTENCES)
            {
                SWL_Debug.Log("BetekenisLevelBoosterController ==> All Sample sentences are already revealed.");
                return;
            }

            // Add a sample sentence to description            
            string text = _levelMainBus.GetCurrentWord().GetNextSampleSentence();
            _levelMainBus.AddSampleSentence(_levelMainBus.GetCurrentWord().RevealedSampleSentenceCount + 1, text);
            _levelMainBus.GetCurrentWord().IncreaseSampleSentenceCount();
        }

        public void OnEnglishButtonPressed()
        {
            _levelMainBus.OnPlayButtonSound?.Invoke();
            // Toggle the English meaning
            _levelMainBus.ToggleEnglishMeaning.Invoke(_levelMainBus.GetCurrentWord());
        }

        public void OnNextButtonPressed()
        {
            _levelMainBus.MoveToNextWord?.Invoke();
        }
    }
}
