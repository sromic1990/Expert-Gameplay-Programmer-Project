using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public GameObject[] groundEnemies;
    public GameObject[] airEnemies;

    private Enemy[][] groundPool;
    private Enemy[][] airPool;

    void Awake()
    {
        groundPool = FillPool(groundEnemies);
        airPool = FillPool(airEnemies);
    }

    /// <summary>
    /// Populates a double array pool of Enemies.
    /// </summary>
    /// <param name="prefabs">Enemy prefab array to populate from</param>
    /// <returns>Two-dimensional Enemy array</returns>
    private Enemy[][] FillPool(GameObject[] prefabs)
    {
        Enemy[][] pool = new Enemy[prefabs.Length][];

        for (int i = 0; i < prefabs.Length; i++)
        {
            pool[i] = new Enemy[GameplayConstants.ENEMY_POOL_SIZE];
            GameObject poolParent = new GameObject(prefabs[i].name + "_Pool");
            poolParent.transform.position = Vector3.zero;

            for (int j = 0; j < GameplayConstants.ENEMY_POOL_SIZE; j++)
            {
                GameObject clone = Instantiate(prefabs[i]);
                clone.transform.parent = poolParent.transform;
                clone.SetActive(false);
                clone.name = prefabs[i].name + "_" + i.ToString();
                pool[i][j] = clone.GetComponent<Enemy>();
            }
        }

        return pool;
    }

    /// <summary>
    /// Spawns a random ground enemy at the passed position, if a
    /// ground enemy is available.
    /// </summary>
    /// <param name="position">World coordinates to spawn at</param>
    public void SpawnGroundEnemy(Vector3 position)
    {
        SpawnFromPool(position, groundPool);
    }

    /// <summary>
    /// Spawns a random air enemy at the passed position, if an
    /// air enemy is available.
    /// </summary>
    /// <param name="position">World coordinates to spawn at</param>
    public void SpawnAirEnemy(Vector3 position)
    {
        SpawnFromPool(position, airPool);
    }

    /// <summary>
    /// Grabs the next available enemy from a random set from the passed
    /// pool and spawns it at the given position. If no enemy from that
    /// random pool is available, it won't spawn and logs a warning.
    /// </summary>
    /// <param name="position">World coordinates to spawn at</param>
    /// <param name="pool">Enemy pool to draw from</param>
    /// <returns>success of spawning.</returns>
    private bool SpawnFromPool(Vector3 position, Enemy[][] pool)
    {
        int randomIndex = Random.Range(0, pool.Length);

        for (int i = 0; i < GameplayConstants.ENEMY_POOL_SIZE; i++)
        {
            GameObject enemy = pool[randomIndex][i].gameObject;
            if (!enemy.activeInHierarchy)
            {
                enemy.SetActive(true);
                pool[randomIndex][i].Spawn(position);
                return true;
            }
        }

        Debug.LogWarning("No available enemy of type " + pool[randomIndex][0].name);
        ClearCache(pool[randomIndex]);
        return false;
    }

    private void ClearCache(Enemy[] pool)
    {
        float nearest = float.MinValue;

        for (int i = 0; i < GameplayConstants.ENEMY_POOL_SIZE; i++)
        {
            float horizontalPosition = pool[i].transform.position.x;
            nearest = Mathf.Max(nearest, horizontalPosition);
        }
        for (int i = 0; i < GameplayConstants.ENEMY_POOL_SIZE; i++)
        {
            GameObject enemy = pool[i].gameObject;
            if (enemy.transform.position.x < nearest)
            {
                enemy.SetActive(false);
            }
        }
    }
}
