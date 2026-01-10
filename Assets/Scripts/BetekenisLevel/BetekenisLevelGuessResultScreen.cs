using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SWL
{
    /// <summary>
    /// Screen for displaying the result of a guess in the Betekenis level.
    /// </summary>
    /// <remarks>
    /// This class is responsible for managing the UI elements related to the guess result.
    /// </remarks>


    public class BetekenisLevelGuessResultScreen : MonoBehaviour
    {
        [SerializeField] Image correctResultImage;
        [SerializeField] Image incorrectResultImage;

        Coroutine disableCoroutine;

        public void ShowCorrectResult(bool skip = false, UnityAction animFinishAction = null)
        {
            if (disableCoroutine != null)
            {
                StopCoroutine(disableCoroutine);
                incorrectResultImage.gameObject.SetActive(false);
                correctResultImage.gameObject.SetActive(false);
            }
            else incorrectResultImage.gameObject.SetActive(false);

            correctResultImage.transform.localScale = Vector3.zero; // Reset scale to default
            correctResultImage.gameObject.SetActive(true);
            correctResultImage.transform.DOScale(Vector3.one, 0.25f).SetEase(Ease.OutBack);
            if(skip) disableCoroutine = StartCoroutine(SkipAfter(1f));

            IEnumerator SkipAfter(float delay)
            {
                yield return new WaitForSeconds(delay);
                correctResultImage.transform.DOScale(Vector3.zero, 0.25f).SetEase(Ease.InBack).OnComplete(() =>
                {
                    SWL_Debug.Log("BetekenisLevelGuessResultScreen ==> Result screen disabled.");
                    correctResultImage.gameObject.SetActive(false);
                    animFinishAction?.Invoke();
                });
            }
        }

        public void ShowIncorrectResult(bool disableAfter = true, UnityAction animFinishAction = null)
        {
            if (disableCoroutine != null)
            {
                StopCoroutine(disableCoroutine);
                incorrectResultImage.gameObject.SetActive(false);
                correctResultImage.gameObject.SetActive(false);
            }
            else correctResultImage.gameObject.SetActive(false);

            incorrectResultImage.transform.localScale = Vector3.zero; // Reset scale to default
            incorrectResultImage.gameObject.SetActive(true);
            incorrectResultImage.transform.DOScale(Vector3.one, 0.25f).SetEase(Ease.OutBack);
            if (disableAfter) disableCoroutine = StartCoroutine(DisableAfter(1f)); // Disable after 2 seconds

            IEnumerator DisableAfter(float delay)
            {
                yield return new WaitForSeconds(delay);
                incorrectResultImage.transform.DOScale(Vector3.zero, 0.25f).SetEase(Ease.InBack).OnComplete(() =>
                {
                    SWL_Debug.Log("BetekenisLevelGuessResultScreen ==> Result screen disabled.");
                    incorrectResultImage.gameObject.SetActive(false);
                    animFinishAction?.Invoke();
                });
            }
        }

        public void ResetResultScreen()
        {
            if (disableCoroutine != null)
            {
                StopCoroutine(disableCoroutine);
                disableCoroutine = null;
            }

            if (correctResultImage.gameObject.activeSelf)
                correctResultImage.transform.DOScale(Vector3.zero, 0.25f).SetEase(Ease.InBack).OnComplete(() =>
                {
                    correctResultImage.gameObject.SetActive(false);
                });

            if (incorrectResultImage.gameObject.activeSelf)
                incorrectResultImage.transform.DOScale(Vector3.zero, 0.25f).SetEase(Ease.InBack).OnComplete(() =>
                {
                    incorrectResultImage.gameObject.SetActive(false);
                });
        }
    }
}

