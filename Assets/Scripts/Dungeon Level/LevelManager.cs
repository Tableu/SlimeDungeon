using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using FischlWorks_FogWar;
using Newtonsoft.Json.Linq;
using Pathfinding;
using Systems.Save;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class LevelManager : MonoBehaviour, ISavable
{
    [FormerlySerializedAs("dungeonLevelData")] [SerializeField] private DungeonGenerationData dungeonGenerationData;
    [SerializeField] private GameObject exitPrefab;
    [SerializeField] private SaveManager saveManager;
    [SerializeField] private GameObject levelCenter;
    [SerializeField] private csFogWar fogOfWar;
    [SerializeField] private LoadingScreen loadingScreen;
    [SerializeField] private GameObject endPopup;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private PartyController partyController;
    [SerializeField] private LevelPlacer levelPlacer;
    [SerializeField] private AstarPath pathfinder;
    
    private List<Transform> _roomColliders = new List<Transform>();
    private int _tileSize;
    private List<Generator2D.LevelData> _dungeonData;
    private bool _saveDataLoaded;
    private int _currentLevel;
    private Generator2D _generator2D;
    private RoomController _spawnRoom;
    private RoomController _exitRoom;
    private Vector2Int  _floorSize;
    private Generator2D.LevelData LevelData => _dungeonData[_currentLevel];

    public string id { get; } = "LevelManager";

    private void Awake()
    {
        _generator2D = new Generator2D();
        playerController.Initialize();
        partyController.Initialize(playerController.PlayerInputActions);
        saveManager.Load();
        
        if (!_saveDataLoaded) //If there is no save data, the player is on a new save and the level manager should generate a new set of levels
        {
            _dungeonData = new List<Generator2D.LevelData>();
            int seed = (int) System.DateTime.Now.Ticks;
            for (int x = 0; x < dungeonGenerationData.Floors.Count; x++)
            {
                _dungeonData.Add(_generator2D.Generate(seed+x, dungeonGenerationData.Floors[x], x));
            }

            _currentLevel = 0;
        }

        Random.InitState(LevelData.RandomSeed);
        _tileSize = dungeonGenerationData.TileSize;

        bool isBossLevel = false;
        LevelPlacer.Results result = new LevelPlacer.Results();
        foreach (BossLevel bossLevel in dungeonGenerationData.BossLevels)
        {
            if (_currentLevel == bossLevel.Index)
            {
                isBossLevel = true;
                GameObject level = Instantiate(bossLevel.Prefab, transform);
                BossLevelInfo info = level.GetComponent<BossLevelInfo>();
                result = new LevelPlacer.Results
                {
                    Colliders = info.Colliders,
                    FloorSize = info.FloorSize,
                    StartRoom = info.StartRoom,
                    ExitRoom = info.ExitRoom
                };
                break;
            }
        }

        if (!isBossLevel)
        {
            result =
                levelPlacer.Run(_tileSize, LevelData, dungeonGenerationData.Floors[_currentLevel]);
        }
        
        _roomColliders = result.Colliders;
        _floorSize = result.FloorSize;
        Vector2Int paddedSize = _floorSize + new Vector2Int(_tileSize, _tileSize);
        levelCenter.transform.position = new Vector3(
            ((float)_floorSize.x * _tileSize)/2, levelCenter.transform.position.y, ((float)_floorSize.y * _tileSize)/2);
        fogOfWar.Initialize(paddedSize*2, _tileSize/2);

        GridGraph gridGraph = pathfinder.data.AddGraph(typeof(GridGraph)) as GridGraph;
        if (gridGraph != null)
        {
            gridGraph.SetDimensions(_floorSize.x * _tileSize, _floorSize.y * _tileSize, 1);
            gridGraph.collision.type = ColliderType.Capsule;
            gridGraph.collision.mask = LayerMask.GetMask("Walls", "Obstacles");
            gridGraph.center = levelCenter.transform.position + Vector3.down*0.1f;
            gridGraph.collision.heightMask = LayerMask.GetMask("Floor");
            gridGraph.collision.unwalkableWhenNoGround = false;
            gridGraph.Scan();
        }
        
        result.StartRoom.SetAsSpawnRoom();
        _spawnRoom = result.StartRoom;
        _exitRoom = result.ExitRoom;

        GameObject floorCollider = new GameObject("Floor Collider");
        floorCollider.layer = LayerMask.NameToLayer("Floor");
        BoxCollider fc = floorCollider.AddComponent<BoxCollider>();
        fc.size = new Vector3((paddedSize.x-2)*_tileSize, 0.001f, (paddedSize.y-2)*_tileSize);
        floorCollider.transform.position = new Vector3(
            ((float)_floorSize.x * _tileSize)/2, levelCenter.transform.position.y, ((float)_floorSize.y * _tileSize)/2);
    }

    private void Start()
    {
        
        GlobalReferences.Instance.Player.transform.position = _spawnRoom.transform.position + _spawnRoom.GetRandomPositionInBounds();
        _exitRoom.SpawnExit(exitPrefab, this);
    }

    public void ExitLevel()
    {
        _currentLevel++;
        if (_currentLevel < dungeonGenerationData.Floors.Count)
        {
            saveManager.Save();
            PlayerPrefs.Save();
            StartCoroutine(LoadSceneAsync("Scenes/MainHub"));
        }
        else
        {
            saveManager.ClearPlayerPrefs();
            File.Delete(SaveManager.DefaultSavePath);
            endPopup.SetActive(true);
            Time.timeScale = 0;
            playerController.enabled = false;
        }
    }

    public void GoToTitleScreen()
    {
        StartCoroutine(LoadSceneAsync("Scenes/Title Screen"));
    }

    public void HandlePlayerDeath()
    {
        StartCoroutine(EndGame());
        File.Delete(SaveManager.DefaultSavePath);
        endPopup.SetActive(true);
        TextMeshProUGUI text = endPopup.GetComponentInChildren<TextMeshProUGUI>();
        text.text = "You Died!";
        WindowManager.Instance.CloseWindow();
    }

    private IEnumerator EndGame()
    {
        yield return new WaitForSeconds(0.2f);
        Time.timeScale = 0;
    }

    private IEnumerator LoadSceneAsync(string scene)
    {
        AsyncOperation loadSceneAsync = SceneManager.LoadSceneAsync(scene);
        PlayerPrefs.SetString("SavedScene", scene);
        PlayerPrefs.Save();
        loadingScreen.gameObject.SetActive(true);
        while (!loadSceneAsync.isDone)
        {
            loadingScreen.RotateIcon();
            yield return null;
        }
    }

    #region Save Logic

    public object SaveState()
    {
        return new SaveData()
        {
            LevelDatas = _dungeonData,
            CurrentLevel = _currentLevel
        };
    }

    public void LoadState(JObject state)
    {
        var saveData = state.ToObject<SaveData>();
        if (saveData.LevelDatas.Count > 0)
        {
            _saveDataLoaded = true;
            _dungeonData = saveData.LevelDatas;
            _currentLevel = saveData.CurrentLevel;
        }
    }

    [Serializable]
    public struct SaveData
    {
        public List<Generator2D.LevelData> LevelDatas;
        public int CurrentLevel;
    }

    #endregion
}
