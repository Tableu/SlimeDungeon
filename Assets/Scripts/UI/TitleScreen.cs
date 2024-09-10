using System.Collections;
using System.IO;
using System.Linq;
using Systems.Save;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class TitleScreen : MonoBehaviour
{
    [SerializeField] private GameObject continueButton;
    [FormerlySerializedAs("formDictionary")] [SerializeField] private CharacterDataDictionary characterDictionary;
    [SerializeField] private GameObject model;
    [SerializeField] private LoadingScreen loadingScreen;
    [SerializeField] private SaveManager saveManager;
    private bool _saveExists;
    private void Start()
    {
        Time.timeScale = 1;
        _saveExists = File.Exists(SaveManager.DefaultSavePath);
        continueButton.SetActive(_saveExists);
        System.Random rand = new System.Random();
        Instantiate(characterDictionary.Dictionary.ElementAt(rand.Next(0, characterDictionary.Dictionary.Count)).Value.Model, model.transform);
    }

    public void OnContinueClick()
    {
        StartCoroutine(LoadSceneAsync());
    }

    public void OnNewGameClick()
    {
        if(_saveExists)
            File.Delete(SaveManager.DefaultSavePath);
        saveManager.ClearPlayerPrefs();
        SceneManager.LoadScene("Scenes/Character Selection");
    }

    public void OnExitClick()
    {
        Application.Quit();
    }
    
    private IEnumerator LoadSceneAsync()
    {
        AsyncOperation loadSceneAsync = SceneManager.LoadSceneAsync("Scenes/MainHub");
        loadingScreen.gameObject.SetActive(true);
        while (!loadSceneAsync.isDone)
        {
            loadingScreen.RotateIcon();
            yield return null;
        }
    }
}
