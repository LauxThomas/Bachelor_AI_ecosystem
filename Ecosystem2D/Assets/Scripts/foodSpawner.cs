using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class foodSpawner : MonoBehaviour
{
    public GameObject[] prefabs;
    private List<GameObject> edibles;
    private List<GameObject> poisons;
    [Range(0.5f, 5.0f)] public float spawnRate = 2.5f;
    [Range(5, 25)] public int initialRate = 10;
    private GameManager gm;
    private GameObject edibleParent;
    private GameObject poisonsParent;

    private void Start()
    {
        edibleParent = GameObject.Find("Edibles");
        poisonsParent = GameObject.Find("Poisons");
        edibles = new List<GameObject>();
        poisons = new List<GameObject>();
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

    public void spawnFood()
    {
        int index;
        //completely Random:
//         index = Random.Range(0, 2);

        //weighted 70%:
        index = Random.value < 0.7f ? 0 : 1;

        if (index == 0)
        {
            Vector3 spawnPoint = new Vector3(Random.Range(-gm.getWindowWidth(), gm.getWindowWidth()),
                Random.Range(-gm.getWindowHeight(), gm.getWindowHeight()), 0);
            GameObject newFood = Instantiate(prefabs[index], spawnPoint, Quaternion.identity);
            edibles.Add(newFood);
            if (edibleParent == null)
            {
                edibleParent = new GameObject("Edibles");
            }

            newFood.transform.parent = edibleParent.transform;
        }
        else
        {
            Vector3 spawnPoint = new Vector3(Random.Range(-gm.getWindowWidth(), gm.getWindowWidth()),
                Random.Range(-gm.getWindowHeight(), gm.getWindowHeight()), 0);
            GameObject newFood = Instantiate(prefabs[index], spawnPoint, Quaternion.identity);
            poisons.Add(newFood);
            if (poisonsParent == null)
            {
                poisonsParent = new GameObject("Poisons");
            }

            newFood.transform.parent = poisonsParent.transform;
        }
    }

    public List<GameObject> getEdibles()
    {
        return edibles;
    }

    public List<GameObject> getPoisons()
    {
        return poisons;
    }

    public void removePoison(GameObject poison)
    {
        poisons.Remove(poison);
    }

    public void removeEdible(GameObject food)
    {
        edibles.Remove(food);
    }

    public void removeObject(GameObject food)
    {
        if (edibles.Contains(food))
        {
            edibles.Remove(food);
        }
        else
        {
            poisons.Remove(food);
        }
    }

    public bool isPoison(GameObject food)
    {
        return poisons.Contains(food);
    }

    public void spawnFoodAt(Vector3 spawnpos)
    {
        int index;
        index = Random.value < 0.7f ? 0 : 1;

        if (index == 0)
        {
            GameObject newFood = Instantiate(prefabs[index], spawnpos, Quaternion.identity);
            edibles.Add(newFood);
            if (edibleParent == null)
            {
                edibleParent = new GameObject("Edibles");
            }

            Debug.Log(spawnpos);
            newFood.transform.parent = edibleParent.transform;
        }
        else
        {
            GameObject newFood = Instantiate(prefabs[index], spawnpos, Quaternion.identity);
            poisons.Add(newFood);
            if (poisonsParent == null)
            {
                poisonsParent = new GameObject("Poisons");
            }

            Debug.Log(spawnpos);
            newFood.transform.parent = poisonsParent.transform;
        }
    }
}