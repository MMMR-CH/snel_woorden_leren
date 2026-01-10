using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MC.Modules.Keyboard;
using MC.Utility;
using UnityEngine;
using UnityEngine.Networking;

namespace SWL
{
    [CreateAssetMenu(fileName = "WoordenScrObj", menuName = "Scriptable Objects/WoordenScrObj", order = 1)]
    public class WoordenScrObj : ScriptableObject
    {
        [SerializeField] string sheetId;
        [Space, Header("A2 Woorden Data")]
        public List<WoordData> WoordenA2;
        [SerializeField] int versionA2 = 1; // version of the woorden data, used to check if the data is up to date
        [SerializeField] string sheetNameA2;
        [Space, Header("B1 Woorden Data")]
        public List<WoordData> WoordenB1;
        [SerializeField] int versionB1 = 1; // version of the woorden data, used to check if the data is up to date
        [SerializeField] string sheetNameB1;
        [Space, Header("Letter Sets")]
        [SerializeField] SerializableDictionary<KeyboardLanguageEnum, LetterSet> LettersetsByLanguage;

        string jsonPathA2 => Path.Combine(Application.persistentDataPath, "WoordenA2.json");
        string jsonPathB1 => Path.Combine(Application.persistentDataPath, "WoordenB1.json");

        public async Task<bool> Init(Action<float> onSetLoadingBar = null)
        {
            onSetLoadingBar?.Invoke(0);
            await InitFromJson(jsonPathA2, WoordenA2);
            onSetLoadingBar?.Invoke(0.1f);
            await InitFromJson(jsonPathB1, WoordenB1);
            onSetLoadingBar?.Invoke(0.2f);
            await UpdateWoorden(jsonPathA2, sheetNameA2, versionA2, WoordenA2);
            onSetLoadingBar?.Invoke(0.6f);
            await UpdateWoorden(jsonPathB1, sheetNameB1, versionB1, WoordenB1);
            onSetLoadingBar?.Invoke(1);
            SWL_Debug.Log("WoordenScrObj initialized from JSON files.");
            return true;
        }

        async Task<bool> InitFromJson(string jsonPath, List<WoordData> woordList)
        {
            // read json file and follow as task            
            if (File.Exists(jsonPath))
            {
                var json = await File.ReadAllTextAsync(jsonPath);
                JsonUtility.FromJsonOverwrite(json, woordList);
            }
            else
            {
                SWL_Debug.LogWarning("File not found: " + jsonPath);
            }
            return true;
        }

        [InspectorButton]
        async void UpdateWoorden()
        {
            var taskA2 = UpdateWoorden(jsonPathA2, sheetNameA2, versionA2, WoordenA2);
            await taskA2;
            versionA2 = taskA2.Result.Item2;
            var taskB1 = UpdateWoorden(jsonPathB1, sheetNameB1, versionB1, WoordenB1);
            await taskB1;
            versionB1 = taskB1.Result.Item2;
        }
        
        async Task<(bool, int)> UpdateWoorden(string jsonPath, string _sheetName, int _version, List<WoordData> woordList)
        {
            SWL_Debug.Log("Updating woorden from Google Sheet...");

            // check internet connection
            if (!InternetConnectionChecker.InternetConnectionON)
            {
                SWL_Debug.LogError("No internet connection");
                return (false, _version);
            }

            // first try server connection ping
            // if ping fails, return false
            SWL_Debug.Log("Pinging server to check connection...");
            var request = new UnityWebRequest("https://www.google.com");
            await request.SendWebRequest();
            if (request.result != UnityWebRequest.Result.Success)
            {
                SWL_Debug.LogError("Failed to peek server connection");
                return (false, _version);
            }

            // read sheet data
            // first only read the first 2 rows to get the version data
            SWL_Debug.Log($"Reading sheet data from {_sheetName}...");
            if (string.IsNullOrEmpty(sheetId) || string.IsNullOrEmpty(_sheetName))
            {
                SWL_Debug.LogError("Sheet ID or Sheet Name is not set");
                return (false, _version);
            }
            SWL_Debug.Log($"Reading index data from {_sheetName}...");
            string range = $"{_sheetName}!B2:B2"; // Adjust the range as needed
            var indexData = await GoogleSheetsReader.ReadSheetData(spreadsheetId: sheetId, range: range);
            SWL_Debug.Log($"Index data read from Google Sheets: {_sheetName} => {indexData.Count} rows, indexData: {string.Join("/ ", indexData.Select(row => string.Join("- ", row)))} ");
            if (indexData != null && indexData.Count > 0)
            {
                // check if the first row has the correct version
                if (indexData.Count > 0 && indexData[0].Count > 0 && int.TryParse(indexData[0][0], out int currentVersionA2))
                {
                    if (currentVersionA2 <= _version)
                    {
                        SWL_Debug.Log($"Sheet {_sheetName} is up to date.");
                        return (false, _version); // no need to update if version is the same or lower
                    }
                    else
                    {
                        _version = currentVersionA2;
                    }
                }
                else
                {
                    SWL_Debug.LogError($"Failed to parse version from Sheet {_sheetName} index data");
                    return (false, _version);
                }
            }
            else
            {
                SWL_Debug.LogError($"Failed to parse version from Sheet {_sheetName} index data");
                return (false, _version);
            }

            // copy all the words. if read fails, return false
            SWL_Debug.Log($"Reading whole sheet data Sheet {_sheetName}");
            var data = await GoogleSheetsReader.ReadSheetData<WoordData>(sheetId, _sheetName);
            if (data[0].WOORD == "index") // check if the first row is the index row
            {
                data.RemoveAt(0); // remove the index row
            }
            if (data != null && data.Count > 0)
            {
                // copy data to woordList
                woordList.Clear();
                woordList.AddRange(data);
                SWL_Debug.Log($"Read {woordList.Count} words from Sheet {_sheetName}");
                // update word attributes
                await UpdateWordAttributes(woordList);

                // check if json path exists, if not create it
                if (!Directory.Exists(Application.persistentDataPath))
                {
                    Directory.CreateDirectory(Application.persistentDataPath);
                }

                // serialize "woorden" to json and save
                var json = JsonUtility.ToJson(woordList);
                File.WriteAllText(jsonPath, json);
                SWL_Debug.Log($"Successfully read {woordList.Count} words from {_sheetName} and saved to {jsonPath}");
            }
            else
            {
                SWL_Debug.LogError($"Failed to read Sheet {_sheetName} sheet data");
                return (false, _version);
            }

            // save scriptable object asset
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
            UnityEditor.AssetDatabase.SaveAssets();
#endif

            return (true, _version);
        }

        public void ResetWoordenTempData(List<WoordData> woordList)
        {
            if (woordList == null || woordList.Count == 0)
            {
                SWL_Debug.LogWarning("No words found to reset");
                return;
            }
            // reset word data
            foreach (var woord in woordList)
            {
                woord.ResetWordTempData();
            }
            SWL_Debug.Log("Reset all word data");
        }

        async Task<bool> UpdateWordAttributes(List<WoordData> woordList)
        {
            if (woordList == null || woordList.Count == 0)
            {
                SWL_Debug.LogError("No words found to update attributes");
                return false;
            }

            // update word attributes
            for (int i = 0; i < woordList.Count; i++)
            {
                var woord = woordList[i];
                woord.name = woord.WOORD; // set name to word
                woord.LevelOrderIdx = i; // set level order index
                // parse hashcode from WOORD HASHCODE string
                woord.HashCode = Int32.TryParse(woord.HASHCODE, out int hashCode) ? hashCode : -11; // set hashcode 
                
                // update VOORBEELD_ZIN_4_ANSWER regrading answer. make it with _______ chars
                try
                {
                    // first approach is this, this will be changed in the future
                    // create a '_' array in the amount of VOORBEELD_ZIN_4_ANSWER letters. put ' ' char instead of space
                    char[] underlineArray = new char[woord.VOORBEELD_ZIN_4_ANSWER.Length];
                    for (int j = 0; j < woord.VOORBEELD_ZIN_4_ANSWER.Length; j++)
                    {
                        underlineArray[j] = woord.VOORBEELD_ZIN_4_ANSWER[j] == ' ' ?  ' ' : '_';
                    }
                    woord.VOORBEELD_ZIN_4 = woord.VOORBEELD_ZIN_4.Replace("_", new string(underlineArray));
                    /*
                    string[] ansewrWordArray = woord.VOORBEELD_ZIN_4_ANSWER.Split(' ');
                    for (int j = 0; j < ansewrWordArray.Length; j++)
                    {
                        woord.VOORBEELD_ZIN_4 = woord.VOORBEELD_ZIN_4.Replace("_", ansewrWordArray[j]);
                    }
                    */
                }
                catch (Exception e)
                {
                    SWL_Debug.LogError($"Failed to update word attributes: {e.Message}");
                }
                
                
                SetWordLetters(woord);
            }

            return true;

            void SetWordLetters(WoordData woord)
            {
                if (string.IsNullOrEmpty(woord.WOORD))
                {
                    SWL_Debug.LogWarning($"Word is empty '{woord.name}'");
                    woord.WordLetters = null;
                    woord.VOORBEELD_ZIN_4_ANSWER_Letters = null;
                    return;
                }
                
                // Set WordLetters enum array
                woord.WordLetters = new Letter[woord.WOORD.Length];
                for (int j = 0; j < woord.WOORD.Length; j++)
                {
                    //SWL_Debug.Log($"Processing letter '{woord.WOORD[j]}' in word '{woord.WOORD}' at index {j}");                    
                    try
                    {
                        // Fix: Use Enum.Parse to convert the character to the corresponding Letter enum value
                        var letterChar = woord.WOORD[j];
                        System.Globalization.CultureInfo cultureInfo = new System.Globalization.CultureInfo("nl-NL");
                        var letter = LettersetsByLanguage[KeyboardLanguageEnum.Dutch].LetterObjectsList.Find((x) => (int)x.Letter == char.ToUpper(letterChar, cultureInfo)).Letter;
                        //SWL_Debug.Log($"Parsed letter '{letterChar}' to Letter enum: {letter}");
                        if (letter == Letter.Null)
                        {
                            Debug.LogWarning($"Invalid letter '{letterChar}' in word '{woord.WOORD}' at index {j}");
                            continue; // Skip invalid letters
                        }
                        else
                        {
                            woord.WordLetters[j] = letter;
                        }
                    }
                    catch (Exception e)
                    {
                        SWL_Debug.LogError($"Error parsing letter '{woord.WOORD[j]}' in word '{woord.WOORD}': {e.Message}");
                        woord.WordLetters[j] = Letter.Null; // Set to Null if parsing fails
                    }
                }
                
                // Set VOORBEELD_ZIN_4_ANSWER_Letters enum array
                if (string.IsNullOrEmpty(woord.VOORBEELD_ZIN_4_ANSWER))
                {
                    SWL_Debug.LogWarning($"VOORBEELD_ZIN_4_ANSWER_Letters is empty in '{woord.WOORD}'");
                    woord.VOORBEELD_ZIN_4_ANSWER_Letters = null;
                    return;
                }
                woord.VOORBEELD_ZIN_4_ANSWER_Letters = new Letter[woord.VOORBEELD_ZIN_4_ANSWER.Length];
                for (int j = 0; j < woord.VOORBEELD_ZIN_4_ANSWER.Length; j++)
                {
                    //SWL_Debug.Log($"Processing letter '{woord.WOORD[j]}' in word '{woord.WOORD}' at index {j}");                    
                    try
                    {
                        // Fix: Use Enum.Parse to convert the character to the corresponding Letter enum value
                        var letterChar = woord.VOORBEELD_ZIN_4_ANSWER[j];
                        System.Globalization.CultureInfo cultureInfo = new System.Globalization.CultureInfo("nl-NL");
                        // The game uses always upper case letters
                        var letter = LettersetsByLanguage[KeyboardLanguageEnum.Dutch].LetterObjectsList.Find((x) => (int)x.Letter == char.ToUpper(letterChar, cultureInfo)).Letter;
                        //SWL_Debug.Log($"Parsed letter '{letterChar}' to Letter enum: {letter}");
                        if (letter == Letter.Null)
                        {
                            Debug.LogWarning($"Invalid letter '{letterChar}' in word '{woord.VOORBEELD_ZIN_4_ANSWER}' at index {j}");
                            continue; // Skip invalid letters
                        }
                        else
                        {
                            woord.VOORBEELD_ZIN_4_ANSWER_Letters[j] = letter;
                        }
                    }
                    catch (Exception e)
                    {
                        SWL_Debug.LogError($"Error parsing letter '{woord.VOORBEELD_ZIN_4_ANSWER[j]}' in word '{woord.VOORBEELD_ZIN_4_ANSWER}': {e.Message}");
                        woord.VOORBEELD_ZIN_4_ANSWER_Letters[j] = Letter.Null; // Set to Null if parsing fails
                    }
                }

                //SWL_Debug.Log($"Updated word '{woord.WOORD}' with letters: {string.Join(", ", woord.WordLetters)}");
            }
        }

    }

    [Serializable]
    public class WoordData
    {
        public string name;

        [GoogleSheetColumn("HASHCODE")]
        [field: SerializeField] public string HASHCODE { get; set; }
        [GoogleSheetColumn("WOORD")]
        [field: SerializeField] public string WOORD { get; set; }
        [GoogleSheetColumn("ID")]
        [field: SerializeField] public string ID { get; set; }
        [GoogleSheetColumn("NIVEAU")]
        [field: SerializeField] public string NIVEAU { get; set; }
        [GoogleSheetColumn("CATEGORY")]
        [field: SerializeField] public string CATEGORY { get; set; }
        [GoogleSheetColumn("BETEKENIS")]
        [field: SerializeField] public string BETEKENIS { get; set; }
        [GoogleSheetColumn("MEANING")]
        [field: SerializeField] public string MEANING { get; set; }
        
        [GoogleSheetColumn("VOORBEELD_ZIN_1")][field: SerializeField] public string VOORBEELD_ZIN_1 { get; set; }
        [GoogleSheetColumn("VOORBEELD_ZIN_2")][field: SerializeField] public string VOORBEELD_ZIN_2 { get; set; }
        [GoogleSheetColumn("VOORBEELD_ZIN_3")][field: SerializeField] public string VOORBEELD_ZIN_3 { get; set; }
        [GoogleSheetColumn("VOORBEELD_ZIN_4")][field: SerializeField] public string VOORBEELD_ZIN_4 { get; set; }
        [GoogleSheetColumn("VOORBEELD_ZIN_4_ANSWER")][field: SerializeField] public string VOORBEELD_ZIN_4_ANSWER { get; set; }
        
        
        [GoogleSheetColumn("IMAGE_URL")]
        [field: SerializeField] public string IMAGE_URL { get; set; }


        public int HashCode = -1; // used for hashcode
        public int LevelOrderIdx = -1; // used for level order idxs
        public Letter[] WordLetters = null; // used for letter objects in the game
        public Letter[] VOORBEELD_ZIN_4_ANSWER_Letters = null;

        // TEMP inlevel data
        public int RevealedSampleSentenceCount { get; private set; } = 0; // used to track how many sample sentences are revealed
        public Letter[] InitialLetters { get; private set; } = null; // used to track initial letters of the word
        public SWL_LanguagesEnum CurrentDescriptionLanguage { get; private set; } = SWL_LanguagesEnum.Dutch;
        public bool IsWoordCompleted { get; private set; } = false; // used to track if the word is completed in the level

        public void ResetWordTempData()
        {
            RevealedSampleSentenceCount = 0;
            InitialLetters = null;
            CurrentDescriptionLanguage = SWL_LanguagesEnum.Dutch;
        }

        public string GetNextSampleSentence()
        {
            switch (RevealedSampleSentenceCount)
            {
                case 0: return VOORBEELD_ZIN_1;
                case 1: return VOORBEELD_ZIN_2;
                case 2: return VOORBEELD_ZIN_3;
                default: return string.Empty; // or throw an exception if index is out of bounds
            }
        }

        public void IncreaseSampleSentenceCount()
        {
            RevealedSampleSentenceCount = Mathf.Clamp(RevealedSampleSentenceCount + 1, 0, GameConstants.MAX_SAMPLE_SENTENCES);
        }

        public void SetInitialLetters(Letter[] initialLetters)
        {
            InitialLetters = initialLetters;
        }

        public void SetCurrentDescriptionLanguage(SWL_LanguagesEnum language)
        {
            CurrentDescriptionLanguage = language;
        }

        public void SetWoordCompleted(bool completed)
        {
            IsWoordCompleted = completed;
        }
    }

    [Serializable]
    public class WoordPaginaIndexData
    {
        [GoogleSheetColumn("WOORD")]
        [field: SerializeField] public string WOORD { get; set; }
        [GoogleSheetColumn("ID")]
        [field: SerializeField] public string ID { get; set; }
    }
}
