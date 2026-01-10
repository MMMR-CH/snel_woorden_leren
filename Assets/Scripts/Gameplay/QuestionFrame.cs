using MC.Modules.Keyboard;
using TMPro;
using UnityEngine;

namespace SWL
{
    public class QuestionFrame : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI questionText;
        
        LetterSet _letterSet;

        public void Init(LetterSet letterSet)
        {
            this._letterSet = letterSet;
        }
        
        public void SetQuestionText(string text)
        {
            questionText.text = text;
        }
    }
}
