using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleSpawner : MonoBehaviour
{
    public GameObject circleBluePrefab; // Circle blue Prefab
    public GameObject circleRedPrefab;  // Circle red Prefab
    public GameObject circleYellowPrefab; // Circle yellow Prefab

    private List<GameObject> circlesToSpawn;
    private int maxCircles = 1;
    private float spawnInterval = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        circlesToSpawn = new List<GameObject>();
        PrepareCircleList();
        StartCoroutine(SpawnCircles());
    }

    void PrepareCircleList()
    {
        if (circleBluePrefab == null || circleRedPrefab == null || circleYellowPrefab == null)
        {
            Debug.LogError("One or more Circle Prefabs are not assigned in CircleSpawner!");
            return;
        }

        for (int i = 0; i < maxCircles; i++)
        {
            float randomValue = Random.value;
            GameObject circleToAdd;

            if (randomValue < 0.60f) // 60% Circle blue
            {
                circleToAdd = circleBluePrefab;
                Debug.Log("Selected Circle blue (60% chance) for spawn list");
            }
            else if (randomValue < 0.90f) // 30% Circle red
            {
                circleToAdd = circleRedPrefab;
                Debug.Log("Selected Circle red (30% chance) for spawn list");
            }
            else // 10% Circle yellow
            {
                circleToAdd = circleYellowPrefab;
                Debug.Log("Selected Circle yellow (10% chance) for spawn list");
            }

            circlesToSpawn.Add(circleToAdd);
        }

        ShuffleList(circlesToSpawn);
    }

    void ShuffleList<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            T temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    IEnumerator SpawnCircles()
    {
        foreach (GameObject circlePrefab in circlesToSpawn)
        {
            GameObject spawnedCircle = Instantiate(circlePrefab, transform.position, Quaternion.identity);

            Rigidbody2D rb = spawnedCircle.GetComponent<Rigidbody2D>();
            if (rb == null)
            {
                Debug.LogError($"Spawned Circle {circlePrefab.name} is missing Rigidbody2D!");
                continue;
            }

            CircleBoundary circleScript = spawnedCircle.GetComponent<CircleBoundary>();
            if (circleScript != null)
            {
                circleScript.SetShootingState(true);
            }

            Debug.Log($"Spawned {circlePrefab.name} at position: {transform.position}, dropping downward");

            yield return new WaitForSeconds(spawnInterval);
        }
    }
}