using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Controller.Player;
using Systems.Save;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;



public class HubManager : MonoBehaviour
{
    [SerializeField] private LoadingScreen loadingScreen;
    [SerializeField] private PartyController partyController;
    [SerializeField] private RandomCharacterData randomCharacterData;
    [SerializeField] private RandomizedChestLoot randomizedChestLoot;
    [SerializeField] private List<CharacterItem> characterItems;
    [SerializeField] private List<SpellItem> spellItems;
    [SerializeField] private SaveManager saveManager;
    private List<CharacterData> _initialCharacterDatas;

    private void Awake()
    {
        saveManager.Load();
    }

    private void Start()
    {
        _initialCharacterDatas = partyController.Characters.Select(character => character.Data).ToList();
        int characterItemCount = Random.Range(3, 4);
        for (int x = 0; x < characterItemCount; x++)
        {
            SetCharacterItem(characterItems[x]);
        }

        int spellItemCount = Random.Range(1, 3);
        for (int x = 0; x < spellItemCount; x++)
        {
            SetSpellItem(spellItems[x]);
        }
    }

    private void SetCharacterItem(CharacterItem characterItem)
    {
        while (true)
        {
            CharacterData data = randomCharacterData.GetRandomElement();
            if (!_initialCharacterDatas.Contains(data))
            {
                characterItem.gameObject.SetActive(true);
                characterItem.Initialize(data);
                break;
            }
        }
    }

    private void SetSpellItem(SpellItem spellItem)
    {
        AttackData data = randomizedChestLoot.GetSpell();
        spellItem.gameObject.SetActive(true);
        spellItem.Initialize(data);
    }

    public void LoadDungeonLevel()
    {
        StartCoroutine(LoadSceneAsync());
    }
    
    private IEnumerator LoadSceneAsync()
    {
        AsyncOperation loadSceneAsync = SceneManager.LoadSceneAsync("Scenes/DungeonGeneration");
        
        loadingScreen.gameObject.SetActive(true);
        while (!loadSceneAsync.isDone)
        {
            loadingScreen.RotateIcon();
            yield return null;
        }
    }
}