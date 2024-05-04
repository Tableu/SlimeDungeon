using System.Collections.Generic;
using System.Linq;
using Controller.Form;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private Generator2D generator2D;
    [SerializeField] private RandomGameObjects randomEnemyGroups;
    [SerializeField] private RandomFormData randomCapturedCharacters;
    
    private List<RoomController> _roomScripts;
    void Start()
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
        
        //Generate random indexes for placing the random characters
        System.Random rnd = new System.Random();
        int[] myRndNos = Enumerable.Range(0, _roomScripts.Count).OrderBy(i => rnd.Next()).Take(capturedCharacters.Count).ToArray();
        int i = 0;
        using List<FormData>.Enumerator characterEnumerator = capturedCharacters.GetEnumerator();
        foreach (RoomController spawner in _roomScripts)
        {
            if (spawner != spawnRoom)
            {
                spawner.SpawnEnemies(randomEnemyGroups.GetRandomGroup());
                foreach (int r in myRndNos)
                {
                    if (i == r)
                    {
                        characterEnumerator.MoveNext();
                        spawner.SpawnCapturedCharacter(characterEnumerator.Current);
                        break;
                    }
                }
            }
            i++;
        }

        
    }
}
