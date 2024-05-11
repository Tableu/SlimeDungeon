using System.Collections.Generic;
using System.Linq;
using Controller.Form;
using Systems.Save;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private Generator2D generator2D;
    [SerializeField] private RandomGameObjects randomEnemyGroups;
    [SerializeField] private RandomGameObjects randomTreasureChests;
    [SerializeField] private RandomFormData randomCapturedCharacters;
    [SerializeField] private GameObject exitPrefab;
    [SerializeField] private SaveManager saveManager;
    
    private List<RoomController> _roomScripts;

    private void Awake()
    {
        saveManager.Load();
    }

    private void Start()
    {
        _roomScripts = generator2D.Generate();
        RoomController spawnRoom = _roomScripts[Random.Range(0, _roomScripts.Count)];
        GlobalReferences.Instance.Player.transform.position = spawnRoom.transform.position;

        //Build and initialize navmesh surfaces
        List<NavMeshBuildSource> sources = new List<NavMeshBuildSource>();
        Vector3 size = new Vector3(generator2D.Size.x * generator2D.TileSize, 10,
            generator2D.Size.y * generator2D.TileSize);
        Vector3 center = new Vector3(size.x / 2, 0, size.z / 2);
        Bounds bounds = new Bounds(center, size);
        NavMeshBuilder.CollectSources(bounds, 
            LayerMask.GetMask("Floor", "Walls"),
            NavMeshCollectGeometry.PhysicsColliders, 0, new List<NavMeshBuildMarkup>(), sources);
        NavMeshData data = NavMeshBuilder.BuildNavMeshData(new NavMeshBuildSettings(), sources, bounds, Vector3.zero,
            Quaternion.identity);
        NavMesh.AddNavMeshData(data);
        
        List<FormData> capturedCharacters = randomCapturedCharacters.GetRandomGroup();
        List<GameObject> treasureChests = randomTreasureChests.GetRandomGroup();
        
        //Generate random indexes for placing the random characters
        List<int> capturedCharacterIndexes = GetUniqueRandomIndexes(_roomScripts.Count, capturedCharacters.Count);
        List<int> treasureChestIndexes = GetUniqueRandomIndexes(_roomScripts.Count, treasureChests.Count);
        int i = 0;
        using List<FormData>.Enumerator characterEnumerator = capturedCharacters.GetEnumerator();
        using List<GameObject>.Enumerator treasureEnumerator = treasureChests.GetEnumerator();
        foreach (RoomController spawner in _roomScripts)
        {
            if (spawner != spawnRoom)
            {
                spawner.SpawnEnemies(randomEnemyGroups.GetRandomGroup());
                if (capturedCharacterIndexes.Contains(i))
                {
                    characterEnumerator.MoveNext();
                    spawner.SpawnCapturedCharacter(characterEnumerator.Current);
                }

                if (treasureChestIndexes.Contains(i))
                {
                    treasureEnumerator.MoveNext();
                    spawner.SpawnTreasureChest(treasureEnumerator.Current);
                }
            }
            i++;
        }

        
        RoomController exitRoom = _roomScripts[Random.Range(0, _roomScripts.Count)];
        while(exitRoom == spawnRoom) {exitRoom = _roomScripts[Random.Range(0, _roomScripts.Count)];}
        exitRoom.SpawnExit(exitPrefab, this);
    }

    public void ExitLevel()
    {
        saveManager.Save();
        SceneManager.LoadScene("Scenes/DungeonGeneration");
    }
    
    private List<int> GetUniqueRandomIndexes(int indexRange, int randomIndexCount)
    {
        System.Random rnd = new System.Random();
        return Enumerable.Range(0, indexRange)
            .OrderBy(i => rnd.Next()).Take(randomIndexCount).ToList();
    }
}
