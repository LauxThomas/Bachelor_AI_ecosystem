using System;
using UnityEngine;
using UnityEngine.Profiling;

public class FoodStats : MonoBehaviour
{
    public float foodAmountAvailable = 100;
    private Color color;
    private bool alreadyFed;
    public bool hasFood;
    public bool fertileStatus = true;
//    public bool fertileIsNear;
    public bool isNearestFoodOfABibit;
    private LayerMask layerMask;

    // Start is called before the first frame update
    private void Start()
    {
        color = GetComponent<SpriteRenderer>().color;
        layerMask = LayerMask.GetMask("poison", "food");
//        InvokeRepeating("checkBools", 0, 3);
    }

    // Update is called once per frame
    private void Update()
    {
        if (CompareTag("poison"))
        {
            Profiler.BeginSample("FoodStatsUpdate-100");
            foodAmountAvailable = -100f;
            Profiler.EndSample();
        }
        else
        {
            Profiler.BeginSample("FoodStatsUpdate-elsefall");
            Profiler.BeginSample("FoodStatsUpdate-checkBools");
//            checkBools();
            Profiler.EndSample();
            Profiler.BeginSample("FoodStatsUpdate-rest");
            hasFood = foodAmountAvailable > 0.5f;
            updateFoodAmount();
            color = fertileStatus ? Color.green : Color.white;
//            color = isNearestFoodOfABibit ? Color.red : Color.white;


            color.a = Mathf.Abs(foodAmountAvailable) / 100;
            GetComponent<SpriteRenderer>().color = color;
            Profiler.EndSample();
            Profiler.EndSample();
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
        foodAmountAvailable = Mathf.Clamp(foodAmountAvailable, 0, 100);
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
}