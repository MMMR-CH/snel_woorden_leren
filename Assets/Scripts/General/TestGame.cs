using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace SWL   
{
    public class TestGame : MonoBehaviour
    {
        [ContextMenu("TestSheets")]
        void TestSheets()
        {
            var data = GoogleSheetsReader.ReadSheetData("15nzyQJZu5FfhMMdUdF6Tok8k3YS1-kPZMqkw-MLNKYQ", "woorden");
            
            // log as json
            SWL_Debug.Log(JsonConvert.SerializeObject(data));
        }
    }
}
