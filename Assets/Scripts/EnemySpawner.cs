using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public struct GroupRandomRange {

    /// <summary>
    ///  Should groups be used? If false all calls to generate value return 1
    /// </summary>
    [Tooltip("Should groups be used? If false all calls to generate value return 1")]
    public bool useGroups;

    /// <summary>
    ///  Are groups spawned close together?
    /// </summary>
    [Tooltip("Are groups spaced close together?")]
    public bool clusters;

    /// <summary>
    ///  The RandomRangeInt object that represents max group sizes
    /// </summary>
    public RandomRangeInt groupSizeRange;

    public int generateValue() {
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
    public RandomRangeFloat spawnTime;

    /// <summary>
    ///  How many enemies will spawn at one time
    /// </summary>
    [Tooltip("The size of groups")]
    public GroupRandomRange groupSize;
    
    /// <summary>
    ///  The types of enemies to spawn
    /// </summary>
    [Tooltip("The types of enemies to spawn")]
    public EnemyDescription[] enemies;

    public GameObject _enemyPrefab;

    // Start is called before the first frame update
    void Start()
    {
        // TODO: Load Enemy Prefab
        StartCoroutine(SpawnLoop());
    }

    IEnumerator SpawnLoop() {
        yield return new WaitForSeconds(spawnTime.generateValue());

        SpawnEvent();
    }
    
    private Vector3 randomXZPoint() {
        Vector3 scale = transform.lossyScale;
        return new Vector3(
                Random.Range(-scale.x, scale.x),
                0,
                Random.Range(-scale.z, scale.z));
    }

    private void SpawnEvent() {
        int newGroupSize = groupSize.generateValue();

        Vector3 clusterOrigin = transform.position;
        if (groupSize.clusters)
            clusterOrigin = randomXZPoint();

        for (int i = 0; i < newGroupSize; i++)
        {
            /*
                Calculate the new enemies starting position by finding a random position
                in the box bounding the spawner and raycasting downward, failing if no ray found
            */
            Vector3 randomPosition = clusterOrigin;
            randomPosition += randomXZPoint();
            randomPosition.y = 100;

            Physics.Raycast(randomPosition, Vector3.down, out RaycastHit hit);

            GameObject newEnemy = Instantiate(
                _enemyPrefab,
                hit.point,
                Quaternion.identity,
                transform.root);
        }
    }
}
