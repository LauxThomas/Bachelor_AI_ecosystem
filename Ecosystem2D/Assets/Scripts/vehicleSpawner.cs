using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class vehicleSpawner : MonoBehaviour
{
    public bool spawnAdditionalVehicles = false;
    private bool switched = false;


    public GameObject[] prefabs;
    private List<GameObject> vehicles;
    [Range(0.5f, 25.0f)] public float spawnRate = 2.5f;
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
    }

    private void Update()
    {
        if (spawnAdditionalVehicles && switched)
        {
            switched = false;
            InvokeRepeating("spawnOrganism", spawnRate, spawnRate);
        }
        else if (!spawnAdditionalVehicles && !switched)
        {
            switched = true;
            CancelInvoke();
        }
    }

    void spawnOrganism()
    {
        Vector3 spawnPosition = new Vector3(Random.Range(-gm.getWindowWidth(), gm.getWindowWidth()),
            Random.Range(-gm.getWindowHeight(), gm.getWindowHeight()), 0);
        GameObject newVehicle =
            Instantiate(prefabs[Random.Range(0, prefabs.Length - 1)], spawnPosition, Quaternion.identity);
        vehicles.Add(newVehicle);
    }

    public void cloneThis(GameObject go)
    {
        GameObject newVehicle = Instantiate(go, go.transform.position, go.transform.rotation);
        vehicles.Add(newVehicle);
    }
}