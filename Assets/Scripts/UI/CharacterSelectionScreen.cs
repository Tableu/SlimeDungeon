using System.Collections.Generic;
using Controller.Form;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterSelectionScreen : MonoBehaviour
{
    [SerializeField] private FormDataDictionary formDictionary;
    [SerializeField] private List<CharacterSelectionIcon> icons;
    [SerializeField] private SpellInfoGroup spellInfoGroup;
    [SerializeField] private List<FormData> characterDatas;
    [SerializeField] private GameObject startButton;
    [SerializeField] private GameObject characterModel;
    private FormData _selectedData;
    private void Start()
    {
        List<FormData>.Enumerator characterEnumerator = characterDatas.GetEnumerator();
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
        if (!formDictionary.Dictionary.ContainsKey(_selectedData.Name))
        {
            Debug.Log("Error - Form not in dictionary");
            return;
        }

        PlayerPrefs.SetString("Initial Form", _selectedData.Name);
        
        PlayerPrefs.Save();
        SceneManager.LoadScene("Scenes/DungeonGeneration");
    }

    public void OnIconClick(FormData formData)
    {
        if (formData == _selectedData)
            return;
        _selectedData = formData;
        spellInfoGroup.SetCharacter(formData);
        startButton.SetActive(true);

        if (characterModel.transform.childCount > 0)
        {
            Destroy(characterModel.transform.GetChild(0).gameObject);
        }

        Instantiate(formData.Model, characterModel.transform);
    }
}