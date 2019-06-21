using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.PlayerLoop;
using Debug = UnityEngine.Debug;
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
        fillWholeWorld();
        addFoodsToParent();
//        float scaler = Camera.main.orthographicSize / 10;
//        prefab1.transform.localScale = new Vector3(scaler,scaler,scaler);
//        prefab2.transform.localScale = new Vector3(scaler,scaler,scaler);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Keypad0))
        {
            foreach (GameObject go in allFoods)
            {
                if (go.CompareTag("food") && go.GetComponent<FoodStats>().fertileIsNear)
                {
                    go.GetComponent<FoodStats>().foodAmountAvailable = 70;
                }
            }
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
        for (int x = (int) (-width / 2); x <= (int) width / 2; x++)
        {
            for (int y = (int) (-height / 2) + 1; y <= (int) height / 2 - 1; y++)
            {
                GameObject spawnFab = Random.value < 0.95 ? prefab1 : prefab2;
                GameObject newObj = Instantiate(spawnFab, new Vector3(x, y), Quaternion.identity);
                newObj.GetComponent<FoodStats>().foodAmountAvailable = 0;
                allFoods.Add(newObj);
            }
        }
    }


    private void fillWorldWithPoison()
    {
        Vector3 poolCenter = new Vector3((int) Random.Range(-width / 2, width / 2),
            (int) Random.Range(-height / 2, height / 2));

//        Vector3 poolCenter = new Vector3(0, 0);
        GameObject center = Instantiate(prefab2, poolCenter, Quaternion.identity);
        allFoods.Add(center);
        poolCenter.x += 1;
        GameObject right = Instantiate(prefab2, poolCenter, Quaternion.identity);
        allFoods.Add(right);
        poolCenter.x -= 2;
        GameObject left = Instantiate(prefab2, poolCenter, Quaternion.identity);
        allFoods.Add(left);
        poolCenter.x += 1;
        poolCenter.y += 1;
        GameObject top = Instantiate(prefab2, poolCenter, Quaternion.identity);
        allFoods.Add(top);
        poolCenter.y -= 2;
        GameObject bottom = Instantiate(prefab2, poolCenter, Quaternion.identity);
        allFoods.Add(bottom);
    }


    public List<GameObject> getAllFoods()
    {
        return allFoods;
    }

//    public void removeFood(GameObject eatenFood)
//    {
//        allFoods.Remove(eatenFood);
//    }

    public double eatFood(GameObject eatenFood, double eatWish)
    {
        if (eatenFood.CompareTag("poison"))
        {
//            Debug.LogError("POISON @ " + eatenFood.transform.position);
            return -5 * Math.Abs(eatWish * Time.deltaTime);
        }

        if (eatenFood.GetComponent<FoodStats>().foodAmountAvailable > 0)
        {
            eatenFood.GetComponent<FoodStats>().foodAmountAvailable -= (float) eatWish * Time.deltaTime;
            return eatWish * Time.deltaTime;
        }

        return 0;
    }
}