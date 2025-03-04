using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class AsteroidSpawnpoint
{
    public Vector2 position;
    public float radius;
    public Vector2 direction;
    public float spawnRate;

    public AsteroidSpawnpoint(Vector2 position, float radius, Vector2 direction)
    {
        this.position = position;
        this.radius = radius;
        this.direction = direction;
    }
}
public class AsteroidSpawner : MonoBehaviour
{
    public GameObject spawnPointContainer;
    public GameObject[] asteroidPrefabs;
    [SerializeField]
    public AsteroidSpawnpoint[] spawnPoints;
    public float maxSpeed;
    public float minSpeed;
    float counter = 0;
    List<int> spawnedIndecies = new List<int>();

    public void Start()
    {
       
        for (int i = 0; i < spawnPointContainer.transform.childCount; i++)
        {
            Transform spawnPoint = spawnPointContainer.transform.GetChild(i);
            spawnPoints[i].position = spawnPoint.position;

            
        }
        StartCoroutine(SpawnCycle());
    }
    IEnumerator SpawnCycle()
    {
        if (spawnedIndecies.Count >= spawnPoints.Length)
        {
            spawnedIndecies.Clear();
            counter = 0;
        }
        counter += Time.deltaTime;
        foreach (var spawnPoint in spawnPoints)
        {
            if(spawnedIndecies.Contains(System.Array.IndexOf(spawnPoints, spawnPoint)))
            {
                continue;
            }
            if (counter > spawnPoint.spawnRate)
            {
                GameObject asteroid = Instantiate(asteroidPrefabs[Random.Range(0, asteroidPrefabs.Length)], spawnPoint.position + Random.insideUnitCircle * spawnPoint.radius, Quaternion.identity);
                asteroid.GetComponent<Asteroid>().direction = spawnPoint.direction;
                asteroid.GetComponent<Asteroid>().speed = Random.Range(minSpeed, maxSpeed);
                spawnedIndecies.Add(System.Array.IndexOf(spawnPoints, spawnPoint));
            }
        }
        yield return null;
        StartCoroutine(SpawnCycle());
    }
   
}
