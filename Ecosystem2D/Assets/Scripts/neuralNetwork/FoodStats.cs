using UnityEngine;

public class FoodStats : MonoBehaviour
{
    public float foodAmountAvailable = 100;
    private Color color;
    private bool alreadyFed;
    public bool hasFood;

    // Start is called before the first frame update
    private void Start()
    {
        color = GetComponent<SpriteRenderer>().color;
    }

    // Update is called once per frame
    private void Update()
    {
        if (CompareTag("poison"))
        {
            foodAmountAvailable = 100f;
        }
        else
        {
            hasFood = foodAmountAvailable > 0.5f;
            updateFoodAmount();
            if (CompareTag("poison"))
            {
                color = Color.blue;
            }
            else if (amIFertile() && !CompareTag("poison"))
            {
                color = Color.green;
            }
            else
            {
                color = Color.white;
            }

            color.a = foodAmountAvailable / 100;
            GetComponent<SpriteRenderer>().color = color;
        }
    }


    private void updateFoodAmount()
    {
        foodAmountAvailable = Mathf.Clamp(foodAmountAvailable, 0, 100);
        if (amIFertile() || isFertileNear())
        {
            foodAmountAvailable += Time.deltaTime * 3;
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

        if (hitUp.transform != null && hitUp.transform.GetComponent<FoodStats>().amIFertile())
        {
            return true;
        }

        RaycastHit2D hitDown = Physics2D.Raycast(pos,
            -up, 1f,
            LayerMask.GetMask("poison", "food"));
        if (hitDown.transform != null && hitDown.transform.GetComponent<FoodStats>().amIFertile())
        {
            return true;
        }

        RaycastHit2D hitLeft = Physics2D.Raycast(pos,
            -right, 1f,
            LayerMask.GetMask("poison", "food"));

        if (hitLeft.transform != null && hitLeft.transform.gameObject.GetComponent<FoodStats>().amIFertile())
        {
            return true;
        }

        RaycastHit2D hitRight = Physics2D.Raycast(pos,
            right, 1f,
            LayerMask.GetMask("poison", "food"));


        if (hitRight.transform != null && hitRight.transform.GetComponent<FoodStats>().amIFertile())
        {
            return true;
        }

        return false;
        return true;
    }


    public bool amIFertile()
    {
        return foodAmountAvailable > 50;
    }
}