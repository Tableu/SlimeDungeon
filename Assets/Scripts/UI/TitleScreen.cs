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
    private bool _saveExists;
    private void Start()
    {
        _saveExists = File.Exists(SaveManager.DefaultSavePath);
        continueButton.SetActive(_saveExists);
        System.Random rand = new System.Random();
        Instantiate(characterDictionary.Dictionary.ElementAt(rand.Next(0, characterDictionary.Dictionary.Count)).Value.Model, model.transform);
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
