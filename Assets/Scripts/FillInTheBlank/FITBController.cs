using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

namespace SWL
{
    public class FITBController : MonoBehaviour
    {
        public int TotalWordsPlayed
        {
            get => PlayerPrefs.GetInt($"TotalWordsPlayed_Betekenis_{BetekenisLevelMainBus.CurrentWoordDataType()}", 0);
            set => PlayerPrefs.SetInt($"TotalWordsPlayed_Betekenis_{BetekenisLevelMainBus.CurrentWoordDataType()}", value);
        }
        // Level bus
        FITBMainBus betekenisLevelMainBus = null;
        FITBMainBus BetekenisLevelMainBus
        {
            get
            {
                if (betekenisLevelMainBus == null)
                {
                    betekenisLevelMainBus = new FITBMainBus();
                    betekenisLevelMainBus.CurrentWoordDataType = GameManager.Instance.MainBus.CurrentWoordDataType;
                    betekenisLevelMainBus.CompleteWord = CompleteWord;
                    betekenisLevelMainBus.IncorrectAnswer = IncorrectAnswer;
                    betekenisLevelMainBus.GetPreviousWord = GetPreviousWord;
                    betekenisLevelMainBus.GetNextWord = GetNextWord;
                    betekenisLevelMainBus.GetCurrentWord = () => UnplayedWordsList.Count > 0 ? UnplayedWordsList[currentWordIndex] : null;
                    betekenisLevelMainBus.OnPlayButtonSound = GameManager.Instance.MainBus.OnPlayButtonSound;

                    // generate input manager
                    InputManager inputManager = new GameObject("InputManager").AddComponent<InputManager>();
                    inputManager.OnKeyboardInputReceived.AddListener((letter) => betekenisLevelMainBus.OnInputReceived(letter));
                }
                return betekenisLevelMainBus;
            }
        }

        // Woord hashcodes of played levels
        HashSet<int> playedLevelHashcodes = null;
        HashSet<int> PlayedLevelHashcodes
        {
            get
            {
                if (playedLevelHashcodes == null)
                {
                    if (!PlayerPrefs.HasKey("PlayedWords_Betekenis")) playedLevelHashcodes = new HashSet<int>();
                    else
                    {
                        //SWL_Debug.Log($"Loading played level hashcodes from PlayerPrefs: " + PlayerPrefs.GetString("PlayedWords_Betekenis", string.Empty));
                        var savedHashset = PlayerPrefs.GetString("PlayedWords_Betekenis", string.Empty);
                        if (string.IsNullOrEmpty(savedHashset))
                        {
                            playedLevelHashcodes = new HashSet<int>();
                        }
                        else
                        {
                            try
                            {
                                playedLevelHashcodes = JsonConvert.DeserializeObject<HashSet<int>>(savedHashset);
                            }
                            catch (System.Exception ex)
                            {
                                SWL_Debug.LogError($"Error deserializing played level hashcodes: {ex.Message}");
                                playedLevelHashcodes = new HashSet<int>();
                            }
                        }
                    }
                }
                return playedLevelHashcodes;
            }
        }
        void SavePlayedLevelIDs() => PlayerPrefs.SetString("PlayedWords_Betekenis", JsonConvert.SerializeObject(PlayedLevelHashcodes));

        // Unplayed woords pool
        HashSet<int> unplayedWords = null;
        List<WoordData> unplayedWordsList = null;
        List<WoordData> UnplayedWordsList
        {
            get
            {
                // Collect unplayed levels for the first time, set level order idxs
                if (unplayedWords == null)
                {
                    SWL_Debug.Log($"Arranging unplayed levels. {TotalWordsPlayed} levels played tot nu");
                    unplayedWords = new HashSet<int>();
                    unplayedWordsList = new List<WoordData>();
                    bool levelNotPlayedYet = true;
                    for (int i = 0; i < GameManager.Instance.MainBus.WoordDatas.Count; i++)
                    {
                        levelNotPlayedYet = !PlayedLevelHashcodes.Contains(GameManager.Instance.MainBus.WoordDatas[i].HashCode);
                        if (levelNotPlayedYet)
                        {
                            unplayedWords.Add(i);
                            unplayedWordsList.Add(GameManager.Instance.MainBus.WoordDatas[i]);
                        }
                    }

                    // Shuffle unplayed words list
                    if (unplayedWordsList.Count > 0) unplayedWordsList.Shuffle();
                }

                // All levels are played before, reset played levels data
                if (unplayedWordsList.Count == 0)
                {
                    unplayedWords = new HashSet<int>();
                    unplayedWordsList = new List<WoordData>();
                    PlayedLevelHashcodes.Clear();
                    SavePlayedLevelIDs();
                    for (int i = 0; i < GameManager.Instance.MainBus.WoordDatas.Count; i++)
                    {
                        unplayedWords.Add(i);
                        unplayedWordsList.Add(GameManager.Instance.MainBus.WoordDatas[i]);
                    }

                    // Shuffle unplayed words list
                    unplayedWordsList.Shuffle();
                }

                if (unplayedWordsList.Count == 0 || unplayedWordsList == null)
                {
                    SWL_Debug.LogError("No unplayed words found. This should not happen.");
                    return new List<WoordData>();
                }


                return unplayedWordsList;
            }
        }



        // sub modules
        [Space, Header("SUB MODULES")]
        [SerializeField] FITBCanvas fitbCanvas;


        int currentWordIndex = 0;





        private void Awake()
        {
            Init();
            StartGameplay();
        }

        void Init()
        {
            var currentLetterSet = GameManager.Instance.MainBus.GetCurrentLetterSetFunc();

            // reset temp word datas
            foreach (var item in GameManager.Instance.MainBus.WoordDatas)
            {
                item.ResetWordTempData();
            }

            // init with EN culture info
            fitbCanvas.Init(BetekenisLevelMainBus, currentLetterSet);

            BetekenisLevelMainBus.LevelInitialized = true;
        }

        void StartGameplay()
        {
            fitbCanvas.InitLevel(BetekenisLevelMainBus.GetCurrentWord());

            //update the progress bar
            BetekenisLevelMainBus.UpdateProgressBar?.Invoke(TotalWordsPlayed, GameManager.Instance.MainBus.WoordDatas.Count, true);
        }

        void CompleteWord(WoordData woord)
        {
            if (!BetekenisLevelMainBus.LevelInitialized) return;

            PlayedLevelHashcodes.Add(woord.HashCode);
            TotalWordsPlayed++;
            woord.SetWoordCompleted(true);
            SavePlayedLevelIDs();

            //update the progress bar
            BetekenisLevelMainBus.UpdateProgressBar?.Invoke(TotalWordsPlayed, GameManager.Instance.MainBus.WoordDatas.Count, true);
        }

        void IncorrectAnswer(WoordData woord)
        {
            woord.SetWoordCompleted(false);
            SWL_Debug.Log("BetekenisLevelController ==> Incorrect answer. No action taken.");
        }

        WoordData GetNextWord()
        {
            if (UnplayedWordsList.Count == 0)
            {
                SWL_Debug.LogError("No unplayed words available.");
                return null;
            }
            // Increment the current word index
            currentWordIndex++;
            if (currentWordIndex >= UnplayedWordsList.Count) currentWordIndex = UnplayedWordsList.Count - 1;
            // Return the next word
            return UnplayedWordsList[currentWordIndex];
        }

        WoordData GetPreviousWord()
        {
            if (UnplayedWordsList.Count == 0)
            {
                SWL_Debug.LogError("No unplayed words available.");
                return null;
            }
            // Decrement the current word index
            currentWordIndex--;
            if (currentWordIndex < 0) currentWordIndex = 0;
            // Return the previous word
            return UnplayedWordsList[currentWordIndex];
        }
    }
}

