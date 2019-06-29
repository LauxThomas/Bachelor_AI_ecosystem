using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

[SuppressMessage("ReSharper", "StringLiteralTypo")]
[SuppressMessage("ReSharper", "IdentifierTypo")]
public class BibitProducer : MonoBehaviour
{
    public static GameObject bibitPrefab;

    public List<String> followerNames;
//    public List<Sprite> sprites;

    private static Vector3 lu;
    private static Vector3 lo;
    private static Vector3 ru;
    public static int maxGeneration;
    public static int maxCurrentGeneration;
    public static int respawns;
    private static String maxGenAnc;
    private static String maxCurrGenAnc;

    [SerializeField] private static List<GameObject> allBibits;
    private static GameObject parent;
    public static float CameraSize;

    // Start is called before the first frame update
    private void Start()
    {
        bibitPrefab = (GameObject) Resources.Load("Bibit");
        InvokeRepeating("readGenerations", 3, 2);
        CameraSize = Camera.main.orthographicSize;
        lu = FoodProducer.lu;
        lo = FoodProducer.lo;
        ru = FoodProducer.ru;
        allBibits = new List<GameObject>();
        parent = new GameObject("Bibits");
        parent.AddComponent<childCounter>();
        initLists();
        initBibits();
    }

    private void initBibits()
    {
        for (int i = 0; i < followerNames.Count * 2; i++)
        {
            spawnBibit(followerNames[i % followerNames.Count]);
        }
    }

    private void initLists()
    {
        followerNames.Add("Stoney0815");
        followerNames.Add("Altay1010");
        followerNames.Add("smoomorli94");
        followerNames.Add("losemymindyesterday");
        followerNames.Add("SporkCodes");
        followerNames.Add("TinkyOwO");
    }

    // Update is called once per frame
    private void Update()
    {
        if (allBibits.Count < 2 * followerNames.Count)
        {
            respawns += followerNames.Count;
            initBibits();
        }

        if (allBibits.Count > 500)
        {
//            Debug.LogError("EXIT APPLICATION, TOO MANY BIBITS");
//            EditorApplication.isPlaying = false;
//            Application.Quit(1);
        }
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
        Vector3 spawnPos = new Vector3(Random.Range(lu.x, ru.x), Random.Range(lu.y, lo.y));
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
        if (allBibits.Count < 500)
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
               "Respawns: " + respawns + " \n" +
               "maxGeneration: " + maxGeneration + " @ " + maxGenAnc + " \n" +
               "maxCurrentGeneration: " + maxCurrentGeneration + " @ " + maxCurrGenAnc + " \n" +
               "numberOfBibits: " + allBibits.Count;
    }

    public static int getNumberOfBibits()
    {
        return allBibits.Count;
    }
}