using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public Transform foodPrefab;
    private List<Transform> allFoods;

    public Transform[] vehiclePrefabs;
    private List<Transform> allvehicles;


    private GameManager gm;

    void Start()
    {
        gm = FindObjectOfType<GameManager>();
        allFoods = new List<Transform>();
        allvehicles = new List<Transform>();
        populateFood();
        //do every x seconds:
        InvokeRepeating("SpawnOrganism", 1.0f, 10.0f);
        InvokeRepeating("addNewFood", 1.0f, 5.0f);
    }

    private void populateFood()
    {
        for (var i = 0; i < gm.initialFoodCount; i++)
        {
            addNewFood();
        }
    }

    public void addNewFood()
    {
        Vector3 spawnPosition = new Vector3(Random.Range(-gm.getWindowWidth(), gm.getWindowWidth()),
            Random.Range(-gm.getWindowHeight(), gm.getWindowHeight()), 0);
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
        Vector3 spawnPosition = new Vector3(Random.Range(-gm.getWindowWidth(), gm.getWindowWidth()),
            Random.Range(-gm.getWindowHeight(), gm.getWindowHeight()), 0);
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

    public List<Transform> getAllFoods()
    {
        return allFoods;
    }
}