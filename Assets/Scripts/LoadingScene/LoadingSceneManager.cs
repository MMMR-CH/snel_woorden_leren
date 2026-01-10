using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SWL
{
    public class LoadingSceneManager : MonoBehaviour
    {
        [SerializeField] WoordenScrObj woordenScrObj;
        [SerializeField] LoadingSceneCanvas canvas;

        // Start is called before the first frame update
        async void Start()
        {
            // set version text
            var versionText = "v" + Application.version;
            canvas.SetVersionText(versionText);

            // update woorden data and follow it in loading bar
            await UpdateWoordenData();
           LevelManager.LoadScene(LevelManager.LevelType.MainMenu);
        }

        async Task UpdateWoordenData()
        {
            // Initialize from JSON
            await woordenScrObj.Init(canvas.SetLoadingBar);            
        }
    }
}
