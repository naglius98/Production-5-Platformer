using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

public class Spawner : MonoBehaviour
{
   public enum ObjectType
   {
        Gem,
        Enemy,
   }

   public Tilemap tilemap;
   public GameObject[] objectPrefabs;
   public float EnemyProbability = 0.2f;
   public int MaxObjects = 8;
   public int MaxEnemies = 4;
   public float GemTime = 5.0f;
   public float SpawnInterval = 0.5f;

   private List<Vector3> ValidSpawnPositions = new List<Vector3>();
   private List<Vector3> OriginalSpawnPositions = new List<Vector3>();
   private List<GameObject> spawnedObjects = new List<GameObject>();
   private bool isSpawning = false;
   private Coroutine spawningCoroutine;

   void Start()

   {
        // Find the valid spawn positions on the tilemap
        FindValidSpawnPositions();
        // Store original positions for reset
        OriginalSpawnPositions = new List<Vector3>(ValidSpawnPositions);
        spawningCoroutine = StartCoroutine(SpawnObjectsWhenNeeded());
   }

   public void ResetSpawner()
   {
        Debug.Log("ResetSpawner called, destroying " + spawnedObjects.Count + " objects");
        
        // Stop all coroutines
        StopAllCoroutines();
        
        // Destroy all spawned objects
        for (int i = spawnedObjects.Count - 1; i >= 0; i--)
        {
            if (spawnedObjects[i] != null)
            {
                Debug.Log("Destroying object: " + spawnedObjects[i].name);
                Destroy(spawnedObjects[i]);
            }
        }
        spawnedObjects.Clear();
        
        // Reset spawn positions
        ValidSpawnPositions = new List<Vector3>(OriginalSpawnPositions);
        isSpawning = false;
        
        // Resume spawning after delay
        StartCoroutine(ResetAndSpawn());
   }
   
   private IEnumerator ResetAndSpawn()
   {
        // Wait one frame to ensure objects are destroyed
        yield return null;
        
        // Now start spawning
        Debug.Log("Starting new spawning cycle");
        spawningCoroutine = StartCoroutine(SpawnObjectsWhenNeeded());
   }

   void Update()
   {
        if (!isSpawning && ActiveObjects() < MaxObjects)
        {
            spawningCoroutine = StartCoroutine(SpawnObjectsWhenNeeded());
        }
   }

   // Find all valid spawn positions
   private void FindValidSpawnPositions()
   {
        ValidSpawnPositions.Clear();
        BoundsInt Bounds = tilemap.cellBounds;

        TileBase[] AllTiles = tilemap.GetTilesBlock(Bounds);
        Vector3 start = tilemap.CellToWorld(new Vector3Int(Bounds.xMin, Bounds.yMin, 0));

        // Loop through all tiles
        for (int x = 0; x < Bounds.size.x; x++)
        {
            for (int y = 0; y < Bounds.size.y; y++)
            {
                TileBase tile = AllTiles[x + y * Bounds.size.x];
                if (tile != null)
                {
                   Vector3 place = start + new Vector3(x + 0.5f, y + 2f, 0); // the object will float above the tile
                   ValidSpawnPositions.Add(place);
                }
            }
        }
   }

    private int ActiveObjects()
    {
        spawnedObjects.RemoveAll(item => item == null);
        return spawnedObjects.Count;
    }

    private int ActiveEnemies()
    {
        int enemyCount = 0;
        foreach (GameObject obj in spawnedObjects)
        {
            if (obj != null && obj.GetComponent<EnemyBehaviour>() != null)
            {
                enemyCount++;
            }
        }
        return enemyCount;
    }

    private bool PositionHasObject(Vector3 PositionToCheck)
    {
        return spawnedObjects.Any(checkObj => checkObj && Vector3.Distance(checkObj.transform.position, PositionToCheck) < 1.0f); // Make sure the object is not too close to the other objects
    }

    private ObjectType RandomObjectType()
    {
        float RandomChoice = Random.value;

        if (RandomChoice <= EnemyProbability)
        {
            return ObjectType.Enemy;
        }
        else
        {
            return ObjectType.Gem;
        }
    }

    private void SpawnObject()
    {
        if (ValidSpawnPositions.Count == 0) return;

        Vector3 SpawnPosition = Vector3.zero;
        bool ValidPositionFound = false;

        while (!ValidPositionFound && ValidSpawnPositions.Count > 0)
        {
            int RandomIndex = Random.Range(0, ValidSpawnPositions.Count);
            Vector3 PotentialPosition = ValidSpawnPositions[RandomIndex];
            Vector3 LeftPosition = PotentialPosition + Vector3.left;
            Vector3 RightPosition = PotentialPosition + Vector3.right;

            if (!PositionHasObject(LeftPosition) && !PositionHasObject(RightPosition))
            {
                SpawnPosition = PotentialPosition;
                ValidPositionFound = true;
            }

            ValidSpawnPositions.RemoveAt(RandomIndex);
        }

        if (ValidPositionFound)
        {
            ObjectType objectType = RandomObjectType();
            
            // Check if we spawned the maximum amount of enemies
            if (objectType == ObjectType.Enemy && ActiveEnemies() >= MaxEnemies)
            {
                // Spawn a gem instead
                objectType = ObjectType.Gem;
            }
            
            GameObject gameObject = Instantiate(objectPrefabs[(int)objectType], SpawnPosition, Quaternion.identity);
            spawnedObjects.Add(gameObject);

            // Destroy gems after a certain time
            if (objectType != ObjectType.Enemy)
            {
                StartCoroutine(DestroyObjectfterTime(gameObject, GemTime));
            }
        }
    }
   private IEnumerator SpawnObjectsWhenNeeded() 
   {
        isSpawning = true; 
        
        while (ActiveObjects() < MaxObjects)
        {
            SpawnObject();
            yield return new WaitForSeconds(SpawnInterval);
        }
        isSpawning = false;
   }

   private IEnumerator DestroyObjectfterTime(GameObject gameObject, float time)
   {
        yield return new WaitForSeconds(time);

        if (gameObject)
        {
            spawnedObjects.Remove(gameObject);
            ValidSpawnPositions.Add(gameObject.transform.position);
            Destroy(gameObject);
        }
   }
}

