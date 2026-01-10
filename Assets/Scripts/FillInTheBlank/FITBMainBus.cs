using MC.Modules.Keyboard;
using System;

namespace SWL
{
    public class FITBMainBus
    {
        // LevelManager
        public Func<WoordDataType> CurrentWoordDataType { get; set; } = null;
        public bool LevelInitialized { get; set; } = false;
        public Action<Letter> OnInputReceived { get; set; } = null;

        // AnswerFrame
        public Action<Letter> AddAnswerChar { get; set; } = null;
        public Action BackspaceAnswer { get; set; } = null;
        public Func<bool> CheckAnswer { get; set; } = null;
        public Action RevealRandomLetter { get; set; } = null;
        public Action RevealFullAnswer { get; set; } = null;
        public Action ClearCurrentAnswer { get; set; } = null;

        // description
        public Action<string, float> AddDescriptionText { get; set; } = null;
        public Action ClearDescriptionText { get; set; } = null;
        public Action<int, string> AddSampleSentence { get; set; } = null;

        // BetekenisLevelController
        public Action<WoordData> CompleteWord { get; set; } = null;
        public Action<WoordData> IncorrectAnswer { get; set; } = null;
        public Func<WoordData> GetPreviousWord { get; set; } = null;
        public Func<WoordData> GetNextWord { get; set; } = null;
        public Func<WoordData> GetCurrentWord { get; set; } = null;

        // betekelenis level canvas
        public Action<WoordData> ToggleEnglishMeaning { get; set; } = null;
        public Action SaveCurrentWoordTempData { get; set; } = null;
        public Action MoveToNextWord { get; set; } = null;
        public Action MoveToPreviousWord { get; set; } = null;

        // audio
        public Action OnPlayButtonSound { get; set; } = null;
        public Action OnPlayCorrectSound { get; set; } = null;

        // progress bar
        public Action<float, float, bool> UpdateProgressBar { get; set; } = null;

        // booster panel
        public Action<bool> ShowNextButton { get; set; } = null;
    }
}
