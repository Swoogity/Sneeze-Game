using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawnManager : MonoBehaviour
{
    public Vector3[] spawnPoints;
    public GameObject pickup;

    public float startDelay = 5.0f;
    public float spawnInterval;

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("SpawnPickup", startDelay, spawnInterval);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // spawns the thing
    void SpawnPickup()
    {
        // picks a random spawn location from the spawnPoints list
        int spawnRange = Random.Range(0, spawnPoints.Length);

        // instantiate at one of the chosen spawn locations
        Instantiate(pickup, spawnPoints[spawnRange], pickup.transform.rotation);

        spawnInterval = Random.Range(10.0f, 15.0f);
        Invoke("SpawnPickup", spawnInterval);

    }
}
