using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using Random = UnityEngine.Random;

public class FoodProducer : MonoBehaviour
{
    public GameObject prefab1;
    public GameObject prefab2;
    
    private float width;
    private float height;
    private Vector3 lu;
    private Vector3 lo;
    private Vector3 ru;
    private Vector3 ro;
    private GameObject parent;
    private List<GameObject> allFoods;
    GameObject spawnFab;


    // Start is called before the first frame update
    void Start()
    {
        lu = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0));
        lo = Camera.main.ViewportToWorldPoint(new Vector3(0, 1, 0));
        ru = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, 0));
        ro = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, 0));
        width = Vector3.Distance(lu, ru);
        height = Vector3.Distance(lu, lo);

        allFoods = new List<GameObject>();
        createFoods(50);
    }

    public void createFoods(int amount)
    {
        for (int i = 0; i < amount; i++)
        {

            spawnFab = Random.value < 0.95 ? prefab1 : prefab2;
            Vector3 spawnPos = new Vector3(Random.Range(lu.x, ru.x), Random.Range(lu.y, lo.y), 0);
            GameObject newFood = Instantiate(spawnFab, spawnPos, Quaternion.identity);
            if (parent == null)
            {
                parent = new GameObject("foods");
            }
            newFood.transform.parent = parent.transform;
            allFoods.Add(newFood);
        }
    }

    public List<GameObject> getAllFoods()
    {
        return allFoods;
    }
    

    // Update is called once per frame
    void Update()
    {
        while (allFoods.Count<50)
        {
            createFoods(1);
        }
    }

    public void removeFood(GameObject eatenFood)
    {
        allFoods.Remove(eatenFood);
    }
}