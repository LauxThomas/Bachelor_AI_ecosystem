using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class vehicleSpawner : MonoBehaviour
{
    public GameObject[] prefabs;
    private List<GameObject> vehicles;
    [Range(0.5f, 5.0f)] public float spawnRate = 2.5f;
    [Range(5, 25)] public int initialRate = 10;
    private GameManager gm;

    private void Start()
    {
        vehicles = new List<GameObject>();
        gm = FindObjectOfType<GameManager>();
        for (int i = 0; i < initialRate; i++)
        {
            spawnOrganism();
        }

        InvokeRepeating("spawnOrganism", 1.0f, spawnRate);
    }

    void spawnOrganism()
    {
        Vector3 spawnPosition = new Vector3(Random.Range(-gm.windowWidth, gm.windowWidth),
            Random.Range(-gm.windowHeight, gm.windowHeight), 0);
        GameObject newVehicle =
            Instantiate(prefabs[Random.Range(0, prefabs.Length - 1)], spawnPosition, Quaternion.identity);
        vehicles.Add(newVehicle);
    }
}