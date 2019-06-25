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
    [SerializeField]
    private List<GameObject> allFoods;
    GameObject spawnFab;
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
    }

    private void fillWholeWorld()
    {
        int waterpools = 0;
        for (int x = (int) (-width / 2) - 1; x <= (int) width / 2 + 1; x++)
        {
            for (int y = (int) (-height / 2); y <= (int) height / 2; y++)
            {
                GameObject spawnPrefab;
                float randomValue = Random.value;
                if (waterpools < mainCam.orthographicSize)
                {
                    if (randomValue < 0.99f)
                    {
                        spawnPrefab = prefab1;
                    }
                    else
                    {
                        spawnPrefab = prefab2;
                        waterpools++;
                    }
                }
                else
                {
                    spawnPrefab = prefab1;
                }

                GameObject newObj = Instantiate(spawnPrefab, new Vector3(x, y), Quaternion.identity);
                newObj.GetComponent<FoodStats>().foodAmountAvailable = 0;
                allFoods.Add(newObj);
            }
        }

        if (waterpools < mainCam.orthographicSize)
        {
            foreach (GameObject foods in allFoods)
            {
                Destroy(foods);
                
            }
            allFoods.Clear();
            
            fillWholeWorld();
        }
    }




    public IEnumerable<GameObject> getAllFoods()
    {
        return allFoods;
    }


    public static double eatFood(GameObject eatenFood, double eatWish)
    {
        if (eatenFood.CompareTag("poison"))
        {
//            Debug.LogError("POISON @ " + eatenFood.transform.position);
            return -3 * Math.Abs(eatWish * Time.deltaTime);
        }

        if (eatenFood.GetComponent<FoodStats>().foodAmountAvailable>eatWish)
        {
            
            eatenFood.GetComponent<FoodStats>().foodAmountAvailable -= (float) eatWish * Time.deltaTime;
            return eatWish * Time.deltaTime;
        }
        else
        {
            float temp = eatenFood.GetComponent<FoodStats>().foodAmountAvailable;
            eatenFood.GetComponent<FoodStats>().foodAmountAvailable = 0;
            return temp*Time.deltaTime;
        }

        return 0;
    }
}