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
    public GameObject bibitPrefab;

    public List<String> followerNames;
//    public List<Sprite> sprites;

    private Vector3 lu;
    private Vector3 lo;
    private Vector3 ru;

    [SerializeField] private int maxGeneration;
    [SerializeField] private int maxCurrentGeneration;

    [SerializeField] private List<GameObject> allBibits;
    private GameObject parent;
    public static float CameraSize;

    // Start is called before the first frame update
    private void Start()
    {
        CameraSize = Camera.main.orthographicSize;
        lu = FoodProducer.lu;
        lo = FoodProducer.lo;
        ru = FoodProducer.ru;
        allBibits = new List<GameObject>();
        initLists();
        initBibits();
    }

    private void initBibits()
    {
        for (int i = 0; i < followerNames.Count * 4; i++)
        {
            spawnBibit(followerNames[i % followerNames.Count]);
            
        }
    }

    private void initLists()
    {
        followerNames.Add("Stoney0815");
        followerNames.Add("Altay1010");
        followerNames.Add("smoomorli94");
//        sprites.Add(Resources.Load<Sprite>("Sprites/mummySprite"));
//        sprites.Add(Resources.Load<Sprite>("Sprites/frankensteinSprite"));
//        sprites.Add(Resources.Load<Sprite>("Sprites/reaperSprite"));
//        sprites.Add(Resources.Load<Sprite>("Sprites/scarecrowSprite"));
//        sprites.Add(Resources.Load<Sprite>("Sprites/vampyrSprite"));
//        sprites.Add(Resources.Load<Sprite>("Sprites/vampyrSprite"));
//        sprites.Add(Resources.Load<Sprite>("Sprites/witchSprite"));
//        sprites.Add(Resources.Load<Sprite>("Sprites/zomieSprite"));
    }

    // Update is called once per frame
    private void Update()
    {
        if (allBibits.Count <= 0)
        {
            initBibits();
        }

        maxCurrentGeneration = 0;
        if (allBibits.Count > CameraSize * 50)
        {
            Debug.LogError("EXIT APPLICATION, TOO MANY BIBITS");
//            EditorApplication.isPlaying = false;
            Application.Quit(1);
        }

        foreach (GameObject go in allBibits)
        {
            int gen = go.GetComponent<Bibit>().getGeneration();
            if (gen > maxGeneration)
            {
                maxGeneration = gen;
            }

            if (gen > maxCurrentGeneration)
            {
                maxCurrentGeneration = gen;
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
//        newBibit.GetComponent<SpriteRenderer>().sprite = sprite;
    }


//    private void spawnBibit()
//    {
//        Vector3 spawnPos = new Vector3(Random.Range(lu.x, ru.x), Random.Range(lu.y, lo.y));
//        GameObject newBibit = Instantiate(bibitPrefab, spawnPos, Quaternion.identity);
//        newBibit.GetComponent<Bibit>().pseudoConstructor1();
//        addBibit(newBibit);
//    }

    private void addBibit(GameObject bibit)
    {
        allBibits.Add(bibit);
        if (parent == null)
        {
            parent = new GameObject("Bibits");
        }

        bibit.transform.parent = parent.transform;
    }

    public void removeBibit(GameObject bibit)
    {
        allBibits.Remove(bibit);
    }

    public void spawnChild(GameObject o)
    {
        Vector3 spawnPos = new Vector3(Random.Range(lu.x, ru.x), Random.Range(lu.y, lo.y));
        GameObject newBibit = Instantiate(bibitPrefab, spawnPos, Quaternion.identity);
        newBibit.GetComponent<Bibit>().pseudoConstructor2(o.GetComponent<Bibit>());
        addBibit(newBibit);
        newBibit.GetComponent<Bibit>().displayName = o.GetComponent<Bibit>().displayName;
//        newBibit.GetComponent<SpriteRenderer>().sprite = sprites[Random.Range(0,sprites.Count)];
    }

    public IEnumerable<GameObject> getAllBibits()
    {
        return allBibits;
    }
}