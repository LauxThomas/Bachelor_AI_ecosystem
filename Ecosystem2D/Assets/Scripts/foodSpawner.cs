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
    [Range(5, 25)] public int initialRate = 10;
    private GameManager gm;

    private void Start()
    {
        edibles = new List<GameObject>();
        gm = FindObjectOfType<GameManager>();

        for (int i = 0; i < initialRate; i++)
        {
            spawnFood();
        }

        InvokeRepeating("spawnFood", 0, spawnRate);
    }

    private void Update()
    {
    }

    private void spawnFood()
    {
        int index;
        //completely Random:
//         index = Random.Range(0, 2);

        //weighted 70%:
        index = Random.value < 0.7f ? 0 : 1;

        Vector3 spawnPoint = new Vector3(Random.Range(-gm.getWindowWidth(), gm.getWindowWidth()),
            Random.Range(-gm.getWindowHeight(), gm.getWindowHeight()), 0);
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

    public bool isPoison(GameObject go)
    {
        return !go.name.ToLower().Contains("food");
    }
}