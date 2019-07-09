using System;
using UnityEngine;

public class FoodStats : MonoBehaviour
{
    public float foodAmountAvailable = 100;
    private Color color;
    private bool alreadyFed;
    public bool hasFood;
    public bool iAmFertile;
    public bool fertileIsNear;
    public bool isNearestFoodOfABibit;

    // Start is called before the first frame update
    private void Start()
    {
        color = GetComponent<SpriteRenderer>().color;
        InvokeRepeating("checkBools", 0, 3);
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
            hasFood = foodAmountAvailable > 0.5f;
            updateFoodAmount();
            color = iAmFertile ? Color.green : Color.white;
//            color = isNearestFoodOfABibit ? Color.red : Color.white;


            color.a = Mathf.Abs(foodAmountAvailable) / 100;
            GetComponent<SpriteRenderer>().color = color;
        }
    }

    void checkBools()
    {
        iAmFertile = amIFertile();
        fertileIsNear = isFertileNear();
        isNearestFoodOfABibit = false;
    }

    private void updateFoodAmount()
    {
        foodAmountAvailable = Mathf.Clamp(foodAmountAvailable, 0, 100);
        if (iAmFertile || fertileIsNear)
        {
            foodAmountAvailable += Time.deltaTime * 1;
        }
    }

    public bool isFertileNear()
    {
        Transform trans = transform;
        Vector3 pos = trans.position;
        Vector3 up = trans.up;
        Vector3 right = trans.right;


        RaycastHit2D hitUp = Physics2D.Raycast(pos,
            up, 1f,
            LayerMask.GetMask("poison", "food"));

        if (hitUp.transform != null && hitUp.transform.GetComponent<FoodStats>().iAmFertile)
        {
            return true;
        }

        RaycastHit2D hitDown = Physics2D.Raycast(pos,
            -up, 1f,
            LayerMask.GetMask("poison", "food"));
        if (hitDown.transform != null && hitDown.transform.GetComponent<FoodStats>().iAmFertile)
        {
            return true;
        }

        RaycastHit2D hitLeft = Physics2D.Raycast(pos,
            -right, 1f,
            LayerMask.GetMask("poison", "food"));

        if (hitLeft.transform != null && hitLeft.transform.gameObject.GetComponent<FoodStats>().iAmFertile)
        {
            return true;
        }

        RaycastHit2D hitRight = Physics2D.Raycast(pos,
            right, 1f,
            LayerMask.GetMask("poison", "food"));


        if (hitRight.transform != null && hitRight.transform.GetComponent<FoodStats>().iAmFertile)
        {
            return true;
        }

        return false;
    }


    private bool amIFertile()
    {
        return CompareTag("poison") || foodAmountAvailable > 70;
    }
}