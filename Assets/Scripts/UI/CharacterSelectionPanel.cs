using Controller.Form;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterSelectionPanel : MonoBehaviour
{
    [SerializeField] private FormDataDictionary formDictionary;
    [SerializeField] private CharacterSelectionIcon icon;
    [SerializeField] private SpellInfoGroup spellInfoGroup;
    [SerializeField] private FormData formData;

    private void Start()
    {
        icon.Initialize(formData.Model);
        spellInfoGroup.Initialize(formData);
    }

    public void OnContinueClick()
    {
        if (!formDictionary.Dictionary.ContainsKey(formData.Name))
        {
            Debug.Log("Error - Form not in dictionary");
            return;
        }

        PlayerPrefs.SetString("Initial Form", formData.Name);
        
        PlayerPrefs.Save();
        SceneManager.LoadScene("Scenes/DungeonGeneration");
    }
}