using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Profiling;
using UnityScript.Macros;
using Random = System.Random;

public class FoodStats : MonoBehaviour
{
    public float foodAmountAvailable = 0;

    public bool fertileStatus = false;
    public bool growFoodStatus = false;
    public bool isFood;

    public int x;

    public int y;

    public FoodStats aboveNeighbour;
    public FoodStats belowNeighbour;
    public FoodStats leftNeighbour;

    public FoodStats rightNeighbour;
    /*
    #region commentingOut

    public Color color;
    public bool hasFood;


//    public bool fertileIsNear;
    public bool isNearestFoodOfABibit;
//    private LayerMask layerMask;

    // Start is called before the first frame update
    private void Start()
    {
        color = GetComponent<SpriteRenderer>().color;
//        layerMask = LayerMask.GetMask("poison", "food");
//        InvokeRepeating("checkBools", 0, 3);
    }

    // Update is called once per frame
    private void Update()
    {
        if (CompareTag("poison"))
        {
            foodAmountAvailable = -100f;
        }
        else
        {
//            checkBools();
            hasFood = foodAmountAvailable > 0.5f;
            updateFoodAmount();
            color = fertileStatus ? Color.green : Color.white;
//            color = isNearestFoodOfABibit ? Color.red : Color.white;


            color.a = math.abs(foodAmountAvailable) / 100;
            GetComponent<SpriteRenderer>().color = color;
        }
    }

//    void checkBools()
//    {
//        Profiler.BeginSample("amIFertile");
//        iAmFertile = amIFertile();
//        Profiler.EndSample();
//        Profiler.BeginSample("isFertileNear");
//        fertileIsNear = isFertileNear();
//        Profiler.EndSample();
//        isNearestFoodOfABibit = false;
//    }

    private void updateFoodAmount()
    {
        foodAmountAvailable = math.clamp(foodAmountAvailable, 0, 100);
        if (fertileStatus)
        {
            foodAmountAvailable += Time.deltaTime * 0.5f;
        }
    }

//    public bool isFertileNear()
//    {
//        Transform trans = transform;
//        Vector3 pos = trans.position;
//        Vector3 up = trans.up;
//        Vector3 right = trans.right;
//
//
////        RaycastHit2D hitUp = Physics2D.Raycast(pos,
////            up, 1f, layerMask);
////
////        if (hitUp.transform != null && hitUp.transform.GetComponent<FoodStats>().iAmFertile)
////        {
////            return true;
////        }
////        RaycastHit2D hitDown = Physics2D.Raycast(pos,
////            -up, 1f, layerMask);
////
////        if (hitDown.transform != null && hitDown.transform.GetComponent<FoodStats>().iAmFertile)
////        {
////            return true;
////        }
////
////        RaycastHit2D hitLeft = Physics2D.Raycast(pos,
////            -right, 1f, layerMask);
////
////        if (hitLeft.transform != null && hitLeft.transform.gameObject.GetComponent<FoodStats>().iAmFertile)
////        {
////            return true;
////        }
////
////        RaycastHit2D hitRight = Physics2D.Raycast(pos,
////            right, 1f, layerMask);
////
////
////        if (hitRight.transform != null && hitRight.transform.GetComponent<FoodStats>().iAmFertile)
////        {
////            return true;
////        }
////
////        return false;
//
//        Profiler.BeginSample("Raycasts");
//        RaycastHit2D hitUp = Physics2D.Raycast(pos,
//            up, 1f, layerMask);
//        RaycastHit2D hitDown = Physics2D.Raycast(pos,
//            -up, 1f, layerMask);
//        RaycastHit2D hitLeft = Physics2D.Raycast(pos,
//            -right, 1f, layerMask);
//        RaycastHit2D hitRight = Physics2D.Raycast(pos,
//            right, 1f, layerMask);
//        Profiler.EndSample();
//
//        bool returnvalue = false;
//        Profiler.BeginSample("Conditions");
//        if (hitUp.transform != null && hitUp.transform.GetComponent<FoodStats>().fertileStatus)
//        {
//            returnvalue = true;
//        }
//
//        if (hitDown.transform != null && hitDown.transform.GetComponent<FoodStats>().fertileStatus)
//        {
//            returnvalue = true;
//        }
//
//
//        if (hitLeft.transform != null && hitLeft.transform.gameObject.GetComponent<FoodStats>().fertileStatus)
//        {
//            returnvalue = true;
//        }
//
//
//        if (hitRight.transform != null && hitRight.transform.GetComponent<FoodStats>().fertileStatus)
//        {
//            returnvalue = true;
//        }
//
//        Profiler.EndSample();
//
//        return returnvalue;
//    }


//    private bool amIFertile()
//    {
//        return CompareTag("poison") || foodAmountAvailable > 70;
//    }

    #endregion
*/
}


public class FoodStatsSystem : ComponentSystem
{
    protected override void OnStartRunning()
    {
        Entities.ForEach((FoodStats fs, Transform transform, SpriteRenderer sr) =>
        {
            fs.x = (int) transform.position.x;
            fs.y = (int) transform.position.y;
            fs.growFoodStatus = false;
            fs.fertileStatus = false;
            fs.isFood = transform.CompareTag("food");
            //calculateNeighbours:
            if (fs.isFood)
            {
                sr.color = Color.clear;
            }
            else
            {
                sr.color = Color.blue;
            }
        });
        Entities.ForEach((FoodStats fs) =>
        {
            Entities.ForEach((FoodStats currentFood) =>
            {
                //left
                if (fs.x == currentFood.x - 1 && fs.y == currentFood.y)
                {
                    currentFood.leftNeighbour = fs;
                }

                //right
                if (fs.x == currentFood.x + 1 && fs.y == currentFood.y)
                {
                    currentFood.rightNeighbour = fs;
                }

                //above
                if (fs.y == currentFood.y + 1 && fs.x == currentFood.x)
                {
                    currentFood.aboveNeighbour = fs;
                }

                //below
                if (fs.y == currentFood.y - 1 && fs.x == currentFood.x)
                {
                    currentFood.belowNeighbour = fs;
                }
            });
        });
    }

    protected override void OnUpdate()
    {
//        if (UnityEngine.Random.value < 1 / 3f)
        {
            float dt = Time.deltaTime;
            Entities.ForEach((FoodStats fs, SpriteRenderer sr) =>
            {
                //checkNeighbours:
                if (fs.aboveNeighbour != null && fs.aboveNeighbour.fertileStatus ||
                    fs.belowNeighbour != null && fs.belowNeighbour.fertileStatus ||
                    fs.leftNeighbour != null && fs.leftNeighbour.fertileStatus ||
                    fs.rightNeighbour != null && fs.rightNeighbour.fertileStatus)
                {
                    fs.growFoodStatus = true;
                }
                else
                {
                    fs.growFoodStatus = false;
                }

                //checkFertilityAndUpdateValue:
                if (fs.isFood)
                {
                    if (fs.fertileStatus)
                    {
                        fs.growFoodStatus = true;
                    }

                    if (fs.foodAmountAvailable < 100f && fs.growFoodStatus)
                    {
                        fs.foodAmountAvailable += 1f * dt;
                    }

                    fs.fertileStatus = fs.foodAmountAvailable > 70;

                }
                else
                {
                    {
                        fs.foodAmountAvailable = -100;
                        fs.fertileStatus = true;
                        fs.growFoodStatus = true;
                    }
                }

                if (UnityEngine.Random.value < 0.3f)
                {
                    if (fs.isFood)
                    {
                        sr.color = new Color(Color.white.r, Color.white.g, Color.white.b,
                            math.abs(fs.foodAmountAvailable) / 100);
                    }
                    else
                    {
                        sr.color = Color.blue;
                    }
                }

            });
        }
    }
}