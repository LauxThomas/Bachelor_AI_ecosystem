using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodStats : MonoBehaviour
{
    public float foodAmountAvailable = 100;
    private Color color;
    public bool isFertile;
    public bool fertileIsNear;
    public bool drawRayO = false;
    public bool drawRayU = false;
    public bool drawRayL = false;
    public bool drawRayR = false;
    private bool alreadyFed;

    // Start is called before the first frame update
    void Start()
    {
        color = GetComponent<SpriteRenderer>().color;
        isFertile = amIFertile();
        fertileIsNear = isFertileNear();
    }

    // Update is called once per frame
    void Update()
    {
        checkForInputs();
        debugDrawing();
        if (CompareTag("poison") && foodAmountAvailable != 100f)
        {
            foodAmountAvailable = 100f;
        }
        else
        {
            updateFoodAmount();
            if (CompareTag("poison"))
            {
                color = Color.blue;
            }
            else if (isFertile && !CompareTag("poison"))
            {
                color = Color.green;
            }
            else
            {
                color = Color.white;
            }

            color.a = foodAmountAvailable / 100;
            GetComponent<SpriteRenderer>().color = color;

            foodAmountAvailable = Mathf.Clamp(foodAmountAvailable, 0, 100);
        }
    }

    private void checkForInputs()
    {
        
    }

    private void debugDrawing()
    {
        if (drawRayO)
        {
            Debug.DrawLine(transform.position, new Vector3(transform.position.x, transform.position.y + 10),
                Color.green, 0.3f);
        }

        if (drawRayU)
        {
            Debug.DrawLine(transform.position, new Vector3(transform.position.x, transform.position.y - 10),
                Color.green, 0.3f);
        }

        if (drawRayR)
        {
            Debug.DrawLine(transform.position, new Vector3(transform.position.x - 10, transform.position.y),
                Color.green, 0.3f);
        }

        if (drawRayL)
        {
            Debug.DrawLine(transform.position, new Vector3(transform.position.x + 10, transform.position.y),
                Color.green, 0.3f);
        }
    }

    private void updateFoodAmount()
    {
        isFertile = amIFertile();
        fertileIsNear = isFertileNear();

        if (isFertile || fertileIsNear)
        {
            foodAmountAvailable += Time.deltaTime*3;
        }
    }

    private bool isFertileNear()
    {
        bool returnvalue = false;
        RaycastHit2D hitUp = Physics2D.Raycast(transform.position,
            transform.up, 1f,
            LayerMask.GetMask("poison", "food"));

        RaycastHit2D hitDown = Physics2D.Raycast(transform.position,
            -transform.up, 1f,
            LayerMask.GetMask("poison", "food"));

        RaycastHit2D hitLeft = Physics2D.Raycast(transform.position,
            -transform.right, 1f,
            LayerMask.GetMask("poison", "food"));

        RaycastHit2D hitRight = Physics2D.Raycast(transform.position,
            transform.right, 1f,
            LayerMask.GetMask("poison", "food"));


        if (hitUp.transform != null && hitUp.transform.GetComponent<FoodStats>().amIFertile())
        {
            returnvalue = true;
        }

        else if (hitDown.transform != null && hitDown.transform.GetComponent<FoodStats>().amIFertile())
        {
            returnvalue = true;
        }

        else if (hitLeft.transform != null && hitLeft.transform.gameObject.GetComponent<FoodStats>().amIFertile())
        {
            returnvalue = true;
        }

        else if (hitRight.transform != null && hitRight.transform.GetComponent<FoodStats>().amIFertile())
        {
            returnvalue = true;
        }

        return returnvalue;
    }

    public void removeFoodAmountAvailableDeltaTime()
    {
        foodAmountAvailable -= Time.deltaTime;
    }

    public bool amIFertile()
    {
        if (foodAmountAvailable > 50)
        {
            isFertile = true;
            return true;
        }

        return false;
    }

}