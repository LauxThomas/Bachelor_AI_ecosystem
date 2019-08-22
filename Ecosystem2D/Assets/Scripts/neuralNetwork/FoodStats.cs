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