using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Unity.Entities;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

[SuppressMessage("ReSharper", "StringLiteralTypo")]
[SuppressMessage("ReSharper", "IdentifierTypo")]
public class BibitProducer : MonoBehaviour
{
    public static GameObject bibitPrefab;
    [SerializeField] public int initialNumberOfBibits = 200;
    [SerializeField] public int minimumNumberOfBibits = 10;
    private static int maximumNumberOfBibits = 2000;

    public List<String> bibitNames;
//    public List<Sprite> sprites;

    private static Vector3 lu;
    private static Vector3 lo;
    private static Vector3 ru;
    private static Vector3 bigBibit, smallBibit;
    public static int maxGeneration;
    public static int maxCurrentGeneration;
    public static int respawns;
    private static String maxGenAnc;
    private static String maxCurrGenAnc;
    public float wiff;
    public float h8;
    private GameObject fpsDisplay;

    [SerializeField] private static List<GameObject> allBibits;
    private static GameObject parent;
    public static float CameraSize;

    // Start is called before the first frame update
    private void Start()
    {
        World.Active.GetExistingSystem<BibitFieldMeasurementSystem>().Enabled = true;
        fpsDisplay = GameObject.Find("PFSCounter");

//        StreamWriter writer = new StreamWriter("Assets/Resources/test.txt", false);
        bigBibit = new Vector3(2, 2, 2);
        smallBibit = new Vector3(0.5f, 0.5f, 0.5f);
        bibitPrefab = (GameObject) Resources.Load("Bibit");
//        InvokeRepeating("readGenerations", 3, 2);
        CameraSize = Camera.main.orthographicSize;
        lu = FoodProducer.lu;
        lo = FoodProducer.lo;
        ru = FoodProducer.ru;
        wiff = Camera.main.orthographicSize * Screen.width / Screen.height;
        h8 = (Camera.main.orthographicSize * Screen.height / Screen.width) * 2;
        allBibits = new List<GameObject>();
        parent = new GameObject("Bibits");
        parent.AddComponent<childCounter>();
        initLists();
        respawns = -2 * bibitNames.Count;
        for (int i = 0; i < initialNumberOfBibits; i++)
        {
            initBibits();
        }
    }

    private void initBibits()
    {
//        for (int i = 0; i < followerNames.Count * 2; i++)
//        {
//            spawnBibit(followerNames[i % followerNames.Count]);
//        }
        spawnBibit(bibitNames[Random.Range(0, bibitNames.Count)]);
        respawns++;
    }

    private void initLists()
    {
        bibitNames.Add("Hans-Werner");
        bibitNames.Add("Frauke");
        bibitNames.Add("Michael");
        bibitNames.Add("Markus");
        bibitNames.Add("Phil");
        bibitNames.Add("Peter");
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            initBibits(1);
        }

        if (Input.GetKey(KeyCode.Space) && Input.GetKey(KeyCode.LeftShift))
        {
            initBibits(25);
        }

        if (allBibits.Count < minimumNumberOfBibits)
        {
            initBibits(1);
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            fpsDisplay.GetComponent<FPSDisplay>().enabled = false;
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            fpsDisplay.GetComponent<FPSDisplay>().enabled = true;
        }
    }

    private void initBibits(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            spawnBibit(bibitNames[Random.Range(0, bibitNames.Count)]);
        }

        respawns += amount;
    }

    private void readGenerations()
    {
        maxCurrentGeneration = 0;
        foreach (GameObject go in allBibits)
        {
            int gen = go.GetComponent<Bibit>().getGeneration();
            if (gen > maxGeneration)
            {
                maxGeneration = gen;
                maxGenAnc = go.name;
                maxGenAnc = maxGenAnc.Substring(0, maxGenAnc.IndexOf(",", StringComparison.Ordinal));
            }

            if (gen > maxCurrentGeneration)
            {
                maxCurrentGeneration = gen;
                maxCurrGenAnc = go.name;
                maxCurrGenAnc = maxCurrGenAnc.Substring(0, maxCurrGenAnc.IndexOf(",", StringComparison.Ordinal));
            }
        }
    }

    private void spawnBibit(string displayName)
    {
        maxCurrentGeneration = 0;
        Vector3 spawnPos = new Vector3(Random.Range(-wiff, wiff), Random.Range(-h8, h8));
        GameObject newBibit = Instantiate(bibitPrefab, spawnPos, Quaternion.identity);
        newBibit.GetComponent<Bibit>().pseudoConstructor1();
        addBibit(newBibit);
        newBibit.GetComponent<Bibit>().displayName = displayName;
    }

    private static void addBibit(GameObject bibit)
    {
        allBibits.Add(bibit);
        bibit.transform.parent = parent.transform;
    }

    public static void removeBibit(GameObject bibit)
    {
        allBibits.Remove(bibit);
    }

    public static void spawnChild(GameObject o)
    {
        if (allBibits.Count < maximumNumberOfBibits)
        {
            Vector3 spawnPos = new Vector3(Random.Range(lu.x, ru.x), Random.Range(lu.y, lo.y));
            GameObject newBibit = Instantiate(bibitPrefab, spawnPos, Quaternion.identity);
            newBibit.GetComponent<Bibit>().pseudoConstructor2(o.GetComponent<Bibit>());
            addBibit(newBibit);
//            newBibit.GetComponent<Bibit>().displayName = o.GetComponent<Bibit>().displayName;
        }
    }

    public static IEnumerable<GameObject> getAllBibits()
    {
        return allBibits;
    }

    public static string getStats()
    {
        return "\n" +
               "running: " + (int) Time.timeSinceLevelLoad + " @ " + Time.timeScale + " \n" +
               "Deaths: " + respawns + " \n" +
               "maxGeneration: " + maxGeneration + " @ " + maxGenAnc + " \n" +
               "maxCurrentGeneration: " + maxCurrentGeneration + " @ " + maxCurrGenAnc + " \n" +
               "numberOfBibits: " + allBibits.Count;
    }

    public static int getNumberOfBibits()
    {
        return allBibits.Count;
    }

    public static void updateGeneration(int generation)
    {
        if (maxGeneration < generation)
        {
            maxGeneration = generation;
        }
    }

    public static void updateMaxGeneration(int generation, Bibit bibit)
    {
        if (generation > 0)
        {
            if (generation >= maxCurrentGeneration)
            {
                maxCurrentGeneration = generation;
                bibit.transform.localScale = bigBibit;
            }
            else
            {
                bibit.transform.localScale = smallBibit;
            }
        }
    }
}