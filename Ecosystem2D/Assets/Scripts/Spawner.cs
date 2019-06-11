using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Spawner : MonoBehaviour
{
    public Transform foodPrefab;
    [Range(1, 25)] public int initialFoodCount = 10;
    public List<Transform> allFoods;

    public Transform[] vehiclePrefabs;
    public List<Transform> allvehicles;


    private GameManager gm;

    void Start()
    {
        gm = FindObjectOfType<GameManager>();
        allFoods = new List<Transform>();
        allvehicles = new List<Transform>();
        populateFood();
        InvokeRepeating("SpawnOrganism", 2.0f, 2.0f);
    }

    private void populateFood()
    {
        for (var i = 0; i < initialFoodCount; i++)
        {
            addNewFood();
        }
    }

    public void addNewFood()
    {
        Vector3 spawnPosition = new Vector3(Random.Range(-gm.windowWidth, gm.windowWidth),
            Random.Range(-gm.windowHeight, gm.windowHeight), 0);
        Transform newFood = Instantiate(foodPrefab, spawnPosition, Quaternion.identity);
        allFoods.Add(newFood);
        if (allFoods.Count >= 500)
        {
            reshapeList();
        }
    }

    private void Update()
    {
    }

    void SpawnOrganism()
    {
        Vector3 spawnPosition = new Vector3(Random.Range(-gm.windowWidth, gm.windowWidth),
            Random.Range(-gm.windowHeight, gm.windowHeight), 0);
        Transform newVehicle = Instantiate(vehiclePrefabs[Random.Range(0, vehiclePrefabs.Length-1)], spawnPosition, Quaternion.identity);
        allvehicles.Add(newVehicle);
    }

    private void reshapeList()
    {
        for (var i = allFoods.Count - 1; i > -1; i--)
        {
            if (allFoods[i] == null)
                allFoods.RemoveAt(i);
        }
    }
}