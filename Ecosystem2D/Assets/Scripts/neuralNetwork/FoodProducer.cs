using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class FoodProducer : MonoBehaviour
{
    public GameObject prefab1;
    public GameObject prefab2;

    public int width;
    public int height;
    public static Vector3 lu;
    public static Vector3 lo;
    public static Vector3 ru;
    private GameObject parent;
    [SerializeField] [Range(0, 1f)] private float offset = 0;

//    private static List<GameObject> allFoodsAndPoisons;
    private static List<GameObject> allFoods;

    private static List<GameObject> allPoisons;
    private static List<GameObject> allFoodsAndPoisons;
    public List<int> foodIndices;


//    GameObject spawnFab;
    private Camera mainCam;


    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 300;
        mainCam = Camera.main;
        lu = mainCam.ViewportToWorldPoint(new Vector3(0, 0, 0));
        lo = mainCam.ViewportToWorldPoint(new Vector3(0, 1, 0));
        ru = mainCam.ViewportToWorldPoint(new Vector3(1, 0, 0));
        width = (int) Vector3.Distance(lu, ru);
        height = (int) Vector3.Distance(lu, lo);

//        height = mainCam.orthographicSize * 2;
//        width = height / 9 * 16;

        allFoods = new List<GameObject>();
        allPoisons = new List<GameObject>();
        allFoodsAndPoisons = new List<GameObject>();
        foodIndices = new List<int>();
        fillWholeWorld();
        addFoodsToParent();
    }

    private void Update()
    {
//        updateFertileStatus();
        if (Input.GetKeyDown(KeyCode.Keypad0))
        {
            foreach (GameObject go in allFoods)
            {
                if (go.CompareTag("food") && go.GetComponent<FoodStats>().fertileStatus)
                {
                    go.GetComponent<FoodStats>().foodAmountAvailable = 70;
                }
            }

//        }
        }
    }

    private void updateFertileStatus()
    {
        for (int i = 0; i < foodIndices.Count; i++)
        {
            allFoodsAndPoisons[foodIndices[i]].GetComponent<FoodStats>().fertileStatus =
                getFertileStatus(foodIndices[i]);
        }
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
        for (int y = (int) (height / 2) + 1; y >= (int) -height / 2 - 1; y--)
        {
            for (int x = (int) (-width / 2) - 1; x <= (int) width / 2 + 1; x++)
            {
                GameObject spawnPrefab;
//                float randomValue = Random.value;
//                if (waterpools < mainCam.orthographicSize / 2)
//                {
//                    if (randomValue < 0.99f)
//                    {
//                        spawnPrefab = prefab1;
//                    }
//                    else
//                    {
//                        spawnPrefab = prefab2;
//                        waterpools++;
//                    }
//                }
//                else
//                {
//                    spawnPrefab = prefab1;
//                }
                spawnPrefab = Mathf.PerlinNoise(x / 10f + randX, y / 10f + randY) < 0.6f ? prefab1 : prefab2;

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
            }
        }

//        if (waterpools < mainCam.orthographicSize / 2)
//        {
//            foreach (GameObject foods in allFoods)
//            {
//                Destroy(foods);
//            }
//
//            allFoods.Clear();
//
//            foreach (GameObject poisons in allPoisons)
//            {
//                Destroy(poisons);
//            }
//
//            allPoisons.Clear();
//
//            fillWholeWorld();
//        }
    }


    public static IEnumerable<GameObject> getAllFoodsAndPoisons()
    {
        return allFoods;
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

    public static int getIndexFrom(GameObject go)
    {
        return allFoods.IndexOf(go);
    }

    public static IEnumerable<GameObject> getAllFoods()
    {
        return allFoods;
    }

    public static IEnumerable<GameObject> getAllPoisons()
    {
        return allPoisons;
    }

    public static GameObject getRandomFood()
    {
        return allFoods[Random.Range(0, allFoods.Count)];
    }

    public bool getFertileStatus(int i)
    {
        //check for self fertile
        if (getFoodAmountAt(i) > 70 || getFoodAmountAt(i) < -95)
        {
            return true;
        }

        int newWidth = width + 2;
        //check above:
        if (i - newWidth >= 0)
        {
            if (getFoodAmountAt(i - newWidth) > 70 || getFoodAmountAt(i - newWidth) < -95)
            {
                return true;
            }
        }

        //check below:
        if (i + newWidth < allFoodsAndPoisons.Count)
        {
            if (getFoodAmountAt(i + newWidth) > 70 || getFoodAmountAt(i + newWidth) < -95)
            {
                return true;
            }
        }

        //check Left:
        if (i % newWidth != 0)
        {
            if (getFoodAmountAt(i - 1) > 70 || getFoodAmountAt(i - 1) < -95)
            {
                return true;
            }
        }

        if (i % newWidth < newWidth - 1)
        {
            if (getFoodAmountAt(i + 1) > 70 || getFoodAmountAt(i + 1) < -95)
            {
                return true;
            }
        }

        return false;
    }

    public float getFoodAmountAt(int i)
    {
        if (i < allFoodsAndPoisons.Count)
        {
            return allFoodsAndPoisons[i].GetComponent<FoodStats>().foodAmountAvailable;
        }

        return 0;
    }

    public static List<FoodStats> getAllFoodsAndPoisonsAsFoodStats()
    {
        List<FoodStats> returnList = new List<FoodStats>();
        foreach (GameObject go in allFoodsAndPoisons)
        {
            returnList.Add(go.GetComponent<FoodStats>());
        }

        return returnList;
    }
}