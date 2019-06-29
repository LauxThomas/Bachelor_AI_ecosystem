using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class FoodProducer : MonoBehaviour
{
    public GameObject prefab1;
    public GameObject prefab2;

    private float width;
    private float height;
    public static Vector3 lu;
    public static Vector3 lo;
    public static Vector3 ru;
    private GameObject parent;
    [SerializeField] [Range(0, 1f)] private float offset = 0;

    [SerializeField]
//    private static List<GameObject> allFoodsAndPoisons;
    private static List<GameObject> allFoods;

    private static List<GameObject> allPoisons;

//    GameObject spawnFab;
    private Camera mainCam;


    // Start is called before the first frame update
    void Start()
    {
        mainCam = Camera.main;
        lu = mainCam.ViewportToWorldPoint(new Vector3(0, 0, 0));
        lo = mainCam.ViewportToWorldPoint(new Vector3(0, 1, 0));
        ru = mainCam.ViewportToWorldPoint(new Vector3(1, 0, 0));
        width = Vector3.Distance(lu, ru);
        height = Vector3.Distance(lu, lo);

        allFoods = new List<GameObject>();
        allPoisons = new List<GameObject>();
        fillWholeWorld();
        addFoodsToParent();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Keypad0))
        {
            foreach (GameObject go in allFoods)
            {
                if (go.CompareTag("food") && go.GetComponent<FoodStats>().isFertileNear())
                {
                    go.GetComponent<FoodStats>().foodAmountAvailable = 70;
                }
            }

//        }
        }

        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            Time.timeScale = 1;
        }

        if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            Time.timeScale += 1;
        }

        if (Input.GetKeyDown(KeyCode.Keypad3))
        {
            Time.timeScale = 10;
        }
        if (Input.GetKeyDown(KeyCode.Keypad4))
        {
            Time.timeScale = 25;
        }
    }


    private void addFoodsToParent()
    {
        if (parent == null)
        {
            parent = new GameObject("Foods");
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
        float randX = (Random.value * 2 - 1)*Random.value*100;
        float randY = (Random.value * 2 - 1)*Random.value*100;
//        Debug.Log(Mathf.PerlinNoise(-0.1f, -0.1f));
//        Debug.Log(Mathf.PerlinNoise(-0.1f, 0));
//        Debug.Log(Mathf.PerlinNoise(-0.1f, 0.1f));
//        Debug.Log(Mathf.PerlinNoise(0, -0.1f));
//        Debug.Log(Mathf.PerlinNoise(0, 0));
//        Debug.Log(Mathf.PerlinNoise(0, 0.1f));
//        Debug.Log(Mathf.PerlinNoise(0.1f, -0.1f));
//        Debug.Log(Mathf.PerlinNoise(0.1f, 0));
//        Debug.Log(Mathf.PerlinNoise(0.1f, 0.1f));
        int waterpools = 0;
        for (int y = (int) (height / 2) + 1; y >= (int) -height / 2 - 1; y--)
        {
            for (int x = (int) (-width / 2) - 1; x <= (int) width / 2 + 1; x++)
            {
                Debug.Log("x / y" + x + " / " + y + ": " + Mathf.PerlinNoise(x  / 10f+randX, y / 10f+randY));
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
                spawnPrefab = Mathf.PerlinNoise(x  / 10f+randX, y / 10f+randY) < 0.55f ? prefab1 : prefab2;

                GameObject newObj = Instantiate(spawnPrefab, new Vector3(x, y), Quaternion.identity);
                newObj.GetComponent<FoodStats>().foodAmountAvailable = 0;
                if (spawnPrefab == prefab1)
                {
                    allFoods.Add(newObj);
                }
                else
                {
                    allPoisons.Add(newObj);
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

    public static double eatPoison()
    {
        return -100 * Time.deltaTime;
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
        return eatenFoodStats.foodAmountAvailable * Time.deltaTime;
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
}