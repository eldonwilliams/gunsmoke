using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public struct GroupRandomRange
{

    /// <summary>
    ///  Should groups be used? If false all calls to generate value return 1
    /// </summary>
    [Tooltip("Should groups be used? If false all calls to generate value return 1")]
    public bool useGroups;

    /// <summary>
    ///  The RandomRangeInt object that represents max group sizes
    /// </summary>
    public RandomRangeInt groupSizeRange;

    public int generateValue()
    {
        if (!useGroups) return 1;
        return groupSizeRange.generateValue();
    }
}

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Configuration")]

    /// <summary>
    ///  The time between spawns
    /// </summary>
    [Tooltip("The time between spawns, used as the debounce")]
    public RandomRangeFloat SpawnTime;

    /// <summary>
    ///  The max number of enemies that can be spawned at once
    /// </summary>
    [Tooltip("The max number of enemies that can be spawned at once")]
    public int MaxSpawns = 10;

    /// <summary>
    ///  How many enemies will spawn at one time
    /// </summary>
    [Tooltip("The size of groups")]
    public GroupRandomRange GroupSize;

    /// <summary>
    ///  The types of enemies to spawn
    /// </summary>
    [Tooltip("The types of enemies to spawn")]
    public EnemyDescription[] Enemies;

    /// <summary>
    ///  A reference to the prefab of the enemy
    /// </summary>
    private GameObject _enemyPrefab;

    /// <summary>
    ///  The number of spawned enemies
    /// </summary>
    private int _spawned;

    // Start is called before the first frame update
    void Start()
    {
        _enemyPrefab = Resources.Load<GameObject>("Prefab/Enemy");

        if (SpawnTime.min == 0 && SpawnTime.max == 0)
        {
            Debug.Log("EnemySpawner infinite loop, min max 0, accident?");
            return;
        }

        StartCoroutine(SpawnLoop());
    }

    IEnumerator SpawnLoop()
    {
        for (; ; )
        {
            yield return new WaitForSeconds(SpawnTime.generateValue());

            SpawnEvent();
        }
    }

    private Vector3 randomXZPoint()
    {
        Vector3 size = transform.lossyScale / 2;
        return new Vector3(
            Random.Range(-size.x, size.x),
            0,
            Random.Range(-size.z, size.z));
    }

    private void SpawnEvent()
    {
        int newGroupSize = GroupSize.generateValue();

        for (int i = 0; i < newGroupSize; i++)
        {
            /*
                Calculate the new enemies starting position by finding a random position
                in the box bounding the spawner and raycasting downward, failing if no ray found
            */
            if (_spawned >= MaxSpawns) return;

            Vector3 randomPosition = transform.position;
            randomPosition += randomXZPoint();
            randomPosition.y = 100;

            Physics.Raycast(randomPosition, Vector3.down, out RaycastHit hit);

            GameObject newEnemy = Instantiate(
                _enemyPrefab,
                hit.point,
                Quaternion.identity,
                null);
            
            _spawned++;
            
            newEnemy.GetComponent<EntityEnemy>().OnDeath += () => {
                _spawned--;
            };
        }
    }
}
