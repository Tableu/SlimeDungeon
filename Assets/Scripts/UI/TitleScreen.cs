using System.IO;
using System.Linq;
using Systems.Save;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreen : MonoBehaviour
{
    [SerializeField] private GameObject continueButton;
    [SerializeField] private FormDataDictionary formDictionary;
    [SerializeField] private GameObject model;
    private bool _saveExists;
    private void Start()
    {
        _saveExists = File.Exists(SaveManager.DefaultSavePath);
        continueButton.SetActive(_saveExists);
        System.Random rand = new System.Random();
        Instantiate(formDictionary.Dictionary.ElementAt(rand.Next(0, formDictionary.Dictionary.Count)).Value.Model, model.transform);
    }

    public void OnContineuClick()
    {
        SceneManager.LoadScene("Scenes/DungeonGeneration");
    }

    public void OnNewGameClick()
    {
        if(_saveExists)
            File.Delete(SaveManager.DefaultSavePath);
        SceneManager.LoadScene("Scenes/Character Selection");
    }

    public void OnExitClick()
    {
        Application.Quit();
    }
}
