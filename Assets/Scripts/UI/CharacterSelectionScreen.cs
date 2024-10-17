using System.Collections;
using System.Collections.Generic;
using Controller.Player;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class CharacterSelectionScreen : MonoBehaviour
{
    [FormerlySerializedAs("formDictionary")] [SerializeField] private CharacterDataDictionary characterDictionary;
    [SerializeField] private List<CharacterSelectionIcon> icons;
    [SerializeField] private SpellInfoGroup spellInfoGroup;
    [SerializeField] private List<PlayerCharacterData> characterDatas;
    [SerializeField] private GameObject startButton;
    [SerializeField] private GameObject characterModel;
    [SerializeField] private LoadingScreen loadingScreen;
    private PlayerCharacterData _selectedData;
    private void Start()
    {
        List<PlayerCharacterData>.Enumerator characterEnumerator = characterDatas.GetEnumerator();
        foreach (CharacterSelectionIcon icon in icons)
        {
            characterEnumerator.MoveNext();
            if (characterEnumerator.Current == null)
                break;
            icon.Initialize(this, characterEnumerator.Current);
        }
        
        spellInfoGroup.SetCharacter(characterDatas[0]);
        Instantiate(characterDatas[0].Model, characterModel.transform);
        _selectedData = characterDatas[0];
    }

    public void OnStartClick()
    {
        if (_selectedData == null)
            return;
        if (!characterDictionary.Dictionary.ContainsKey(_selectedData.Name))
        {
            Debug.Log("Error - Form not in dictionary");
            return;
        }

        PlayerPrefs.SetString("Initial Form", _selectedData.Name);
        
        PlayerPrefs.Save();
        StartCoroutine(LoadSceneAsync());
    }
    
    private IEnumerator LoadSceneAsync()
    {
        string scene = "Scenes/DungeonGeneration";
        if (PlayerPrefs.HasKey("SavedScene"))
            scene = PlayerPrefs.GetString("SavedScene");
        AsyncOperation loadSceneAsync = SceneManager.LoadSceneAsync(scene);
        loadingScreen.gameObject.SetActive(true);
        while (!loadSceneAsync.isDone)
        {
            loadingScreen.RotateIcon();
            yield return null;
        }
    }

    public void OnIconClick(PlayerCharacterData playerCharacterData)
    {
        if (playerCharacterData == _selectedData)
            return;
        _selectedData = playerCharacterData;
        spellInfoGroup.SetCharacter(playerCharacterData);
        startButton.SetActive(true);

        if (characterModel.transform.childCount > 0)
        {
            Destroy(characterModel.transform.GetChild(0).gameObject);
        }

        Instantiate(playerCharacterData.Model, characterModel.transform);
    }
}