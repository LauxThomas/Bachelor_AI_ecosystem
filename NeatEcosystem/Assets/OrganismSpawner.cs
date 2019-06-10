using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrganismSpawner : MonoBehaviour
{
    public GameObject[] organismPrefabs;
    public GameObject[] foodPrefabs;
    public int borderrange = 100;

    public float spawnTimer = 5.0f;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        spawnTimer -= Time.deltaTime;
        if (spawnTimer <= 0)
        {
            spawnOrganism();
            spawnTimer = Random.Range(2, 4);
        }
    }

    private void spawnOrganism()
    {
        int randomSpawn = Random.Range(0, organismPrefabs.Length);
        Vector3 position = new Vector3(Random.Range(-borderrange, borderrange), Random.Range(-borderrange, borderrange),
            0);
        Instantiate(organismPrefabs[randomSpawn], position, Quaternion.identity);
    }
}