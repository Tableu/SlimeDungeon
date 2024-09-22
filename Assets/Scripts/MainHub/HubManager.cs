using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Controller.Player;
using Systems.Save;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Windows;



public class HubManager : MonoBehaviour
{
    [SerializeField] private LoadingScreen loadingScreen;
    [SerializeField] private PartyController partyController;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private RandomCharacterData randomCharacterData;
    [SerializeField] private RandomizedChestLoot randomizedChestLoot;
    [SerializeField] private List<CharacterItem> characterItems;
    [SerializeField] private List<SpellItem> spellItems;
    [SerializeField] private List<HatItem> hatItems;
    [SerializeField] private SaveManager saveManager;
    [SerializeField] private MainHubData mainHubData;
    private List<PlayerCharacterData> _initialCharacterDatas;

    private void Awake()
    {
        playerController.Initialize();
        partyController.Initialize(playerController.PlayerInputActions);
        saveManager.Load();
    }

    private void Start()
    {
        if (!File.Exists(saveManager.savePath))
            return;
        _initialCharacterDatas = partyController.Characters.Select(character => character.Data).ToList();
        MainHubLevel levelData = mainHubData.GetLevelData(PlayerPrefs.GetInt("SlimesSaved"));

        for (int x = 0; x < levelData.CharacterCount; x++)
        {
            SetCharacterItem(characterItems[x]);
        }

        for (int x = 0; x < levelData.SpellCount; x++)
        {
            SetSpellItem(spellItems[x]);
        }
        
        for (int x = 0; x < levelData.HatCount; x++)
        {
            SetHatItem(hatItems[x]);
        }
    }

    private void SetCharacterItem(CharacterItem characterItem)
    {
        while (true)
        {
            PlayerCharacterData data = randomCharacterData.GetRandomElement();
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

    private void SetHatItem(HatItem hatItem)
    {
        EquipmentData data = randomizedChestLoot.GetHat();
        hatItem.gameObject.SetActive(true);
        hatItem.Initialize(data);
    }

    public void LoadDungeonLevel()
    {
        StartCoroutine(LoadSceneAsync());
    }
    
    private IEnumerator LoadSceneAsync()
    {
        saveManager.Save();
        AsyncOperation loadSceneAsync = SceneManager.LoadSceneAsync("Scenes/DungeonGeneration");
        
        loadingScreen.gameObject.SetActive(true);
        while (!loadSceneAsync.isDone)
        {
            loadingScreen.RotateIcon();
            yield return null;
        }
    }
}