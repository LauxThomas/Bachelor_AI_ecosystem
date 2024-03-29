﻿using System.Collections.Generic;
using UnityEngine;

public class vehicleSpawner : MonoBehaviour
{
    public bool spawnAdditionalVehicles = false;
    private bool switched = false;


    public GameObject[] prefabs;
    private List<GameObject> vehicles;
    [Range(0.5f, 25.0f)] public float spawnRate = 2.5f;
    [Range(1, 25)] public int initialRate = 1;
    private GameManager gm;
    private GameObject parent;

    private void Start()
    {
        parent = GameObject.Find("Vehicles");
        vehicles = new List<GameObject>();
        gm = FindObjectOfType<GameManager>();
        for (int i = 0; i < initialRate; i++)
        {
            spawnOrganism();
        }
    }

    private void Update()
    {
        reloadIfNecessary();
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

    private void reloadIfNecessary()
    {
        if (parent.transform.childCount == 0)
        {
            Application.LoadLevel(Application.loadedLevelName);
        }
    }

    public void spawnOrganism()
    {
        Vector3 spawnPosition = new Vector3(Random.Range(-gm.getWindowWidth(), gm.getWindowWidth()),
            Random.Range(-gm.getWindowHeight(), gm.getWindowHeight()), 0);
        GameObject newVehicle =
            Instantiate(prefabs[Random.Range(0, prefabs.Length - 1)], spawnPosition, Quaternion.identity);
        vehicles.Add(newVehicle);

        if (parent == null)
        {
            parent = new GameObject("Vehicles");
        }

        newVehicle.transform.parent = parent.transform;
    }

    public GameObject cloneThis(GameObject go)
    {
        GameObject newVehicle = Instantiate(go, go.transform.position, go.transform.rotation);
        vehicles.Add(newVehicle);
        newVehicle.transform.parent = parent.transform;
        return newVehicle;
    }

    public int getVehicleCount()
    {
        return vehicles.Count;
    }
}