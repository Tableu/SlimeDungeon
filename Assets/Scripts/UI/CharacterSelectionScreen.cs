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
    [SerializeField] private List<CharacterData> characterDatas;
    [SerializeField] private GameObject startButton;
    [SerializeField] private GameObject characterModel;
    private CharacterData _selectedData;
    private void Start()
    {
        List<CharacterData>.Enumerator characterEnumerator = characterDatas.GetEnumerator();
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
        SceneManager.LoadScene("Scenes/DungeonGeneration");
    }

    public void OnIconClick(CharacterData characterData)
    {
        if (characterData == _selectedData)
            return;
        _selectedData = characterData;
        spellInfoGroup.SetCharacter(characterData);
        startButton.SetActive(true);

        if (characterModel.transform.childCount > 0)
        {
            Destroy(characterModel.transform.GetChild(0).gameObject);
        }

        Instantiate(characterData.Model, characterModel.transform);
    }
}