using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class FoodProducer : MonoBehaviour
{
    public MyScriptableObjectClass myScriptableObject;
    public GameObject prefab1;
    public GameObject prefab2;

    public int width;
    public int height;
    public static Vector3 lu;
    public static Vector3 lo;
    public static Vector3 ru;
    private GameObject parent;
    [SerializeField] [Range(0, 1f)] private float offset = 0;
    [SerializeField] [Range(5, 100f)] public float landmassConnection = 50;

//    private static List<GameObject> allFoodsAndPoisons;
    private static List<GameObject> allFoods;

    private static List<GameObject> allPoisons;
    private static List<GameObject> allFoodsAndPoisons;
    public static FoodStats[,] fieldsArray;
    public List<int> foodIndices;


//    GameObject spawnFab;
    private Camera mainCam;
    public int percentageOfGrassSpots=60;


    // Start is called before the first frame update
    void Start()
    {
        percentageOfGrassSpots = myScriptableObject.grassPercentage;
        landmassConnection = myScriptableObject.landMassConnection;
        Application.targetFrameRate = 300;
        mainCam = Camera.main;
        lu = mainCam.ViewportToWorldPoint(new Vector3(0, 0, 0));
        lo = mainCam.ViewportToWorldPoint(new Vector3(0, 1, 0));
        ru = mainCam.ViewportToWorldPoint(new Vector3(1, 0, 0));
        width = (int) Vector3.Distance(lu, ru);
        height = (int) Vector3.Distance(lu, lo);

        allFoods = new List<GameObject>();
        allPoisons = new List<GameObject>();
        allFoodsAndPoisons = new List<GameObject>();
        foodIndices = new List<int>();

        fillWholeWorld();
        addFoodsToParent();
    }





    private void addFoodsToParent()
    {
        if (parent == null)
        {
            parent = new GameObject("Foods");
            parent.AddComponent<childCounter>();
        }

        foreach (GameObject go in allFoods)
        {
            go.transform.SetParent(parent.transform);
        }

        foreach (GameObject go in allPoisons)
        {
            go.transform.SetParent(parent.transform);
        }
    }

    private void fillWholeWorld()
    {
        float randX = (Random.value * 2 - 1) * Random.value * 100;
        float randY = (Random.value * 2 - 1) * Random.value * 100;
        int waterpools = 0;
        int newWidth = math.abs((-width / 2) - 1 - width / 2 + 1) + 3;
        int newHeight = (height / 2) + 1 - (-height / 2 - 1) + 1;
        int newX = 0, newY = 0;
        fieldsArray = new FoodStats[newWidth, newHeight];
        for (int y = (int) (height / 2) + 1; y >= (int) -height / 2 - 1; y--)
        {
            for (int x = (int) (-width / 2) - 1; x <= (int) width / 2 + 1; x++)
            {
                GameObject spawnPrefab;
                spawnPrefab = Mathf.PerlinNoise(x / landmassConnection + randX, y / landmassConnection + randY) < percentageOfGrassSpots/100f ? prefab1 : prefab2;

                GameObject newObj = Instantiate(spawnPrefab, new Vector3(x, y), Quaternion.identity);
                newObj.GetComponent<FoodStats>().foodAmountAvailable = 0;
                if (spawnPrefab == prefab1)
                {
                    allFoods.Add(newObj);
                    allFoodsAndPoisons.Add(newObj);
                    foodIndices.Add(allFoodsAndPoisons.Count - 1);
                }
                else
                {
                    allPoisons.Add(newObj);
                    allFoodsAndPoisons.Add(newObj);
                }

                fieldsArray[newX, newY] = newObj.GetComponent<FoodStats>();
                newX++;
                newX %= newWidth;
            }

            newY++;
            newY %= newHeight;
        }

    }



    public static double eatPoison(double eatWish)
    {
        return -3 * eatWish * Time.deltaTime;
    }

    public static double eatFood(GameObject eatenFood, double eatWish)
    {
        FoodStats eatenFoodStats = eatenFood.GetComponent<FoodStats>();

        if (eatenFoodStats.foodAmountAvailable > eatWish)
        {
            eatenFoodStats.foodAmountAvailable -= (float) eatWish * Time.deltaTime;
            return eatWish * Time.deltaTime;
        }

        eatenFoodStats.foodAmountAvailable = 0;
        return 0;
    }


    public static IEnumerable<GameObject> getAllFoods()
    {
        return allFoods;
    }

    public static IEnumerable<GameObject> getAllPoisons()
    {
        return allPoisons;
    }


}