using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class foodSpawner : MonoBehaviour
{
    public GameObject[] prefabs;
    private List<GameObject> edibles;
    [Range(0.5f, 5.0f)] public float spawnRate = 2.5f;
    
    private float width=9;
    private float height=5;

    private void Start()
    {
        edibles = new List<GameObject>();
        InvokeRepeating("spawnFood",0,spawnRate);
    }

    private void Update()
    {
    }

    private void spawnFood()
    {
        int index= Random.Range(0,2);
        Vector3 spawnPoint = new Vector3(Random.Range(-width,width),Random.Range(-height,height),0);
        GameObject newFood = Instantiate(prefabs[index], spawnPoint, Quaternion.identity);
        edibles.Add(newFood);
    }

    public List<GameObject> getEdibles()
    {
        return edibles;
    }

    public void removeEdible(GameObject food)
    {
        edibles.Remove(food);
    }
}
