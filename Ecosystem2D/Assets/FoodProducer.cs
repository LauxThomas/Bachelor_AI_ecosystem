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
    public List<GameObject> allWater;
    public List<GameObject> fertileSpots;
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
        allWater = new List<GameObject>();
        fertileSpots = new List<GameObject>();
        fillWholeWorld();
//        fillWorldWithPoison();
    }

    private void fillWholeWorld()
    {
        for (int x = (int) (-width / 2); x <= (int) width / 2; x++)
        {
            for (int y = (int) (-height / 2)+1; y <= (int) height / 2-1; y++)
            {
                GameObject spawnFab = Random.value < 0.95 ? prefab1 : prefab2;
                GameObject newObj = Instantiate(spawnFab, new Vector3(x,y), Quaternion.identity);
                newObj.GetComponent<FoodStats>().foodAmountAvailable = 0;
                fertileSpots.Add(newObj);
            }
        }
    }



    private void fillWorldWithPoison()
    {
        Vector3 poolCenter = new Vector3((int) Random.Range(-width / 2, width / 2),
            (int) Random.Range(-height / 2, height / 2));

//        Vector3 poolCenter = new Vector3(0, 0);
        GameObject center = Instantiate(prefab2, poolCenter, Quaternion.identity);
        allWater.Add(center);
        fertileSpots.Add(center);
        poolCenter.x += 1;
        GameObject right = Instantiate(prefab2, poolCenter, Quaternion.identity);
        allWater.Add(right);
        fertileSpots.Add(right);
        poolCenter.x -= 2;
        GameObject left = Instantiate(prefab2, poolCenter, Quaternion.identity);
        allWater.Add(left);
        fertileSpots.Add(left);
        poolCenter.x += 1;
        poolCenter.y += 1;
        GameObject top = Instantiate(prefab2, poolCenter, Quaternion.identity);
        allWater.Add(top);
        fertileSpots.Add(top);
        poolCenter.y -= 2;
        GameObject bottom = Instantiate(prefab2, poolCenter, Quaternion.identity);
        allWater.Add(bottom);
        fertileSpots.Add(bottom);
    }


    public List<GameObject> getAllFoods()
    {
        return allFoods;
    }

    public void removeFood(GameObject eatenFood)
    {
        allFoods.Remove(eatenFood);
    }
}