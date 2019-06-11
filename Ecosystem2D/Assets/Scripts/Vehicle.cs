using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using Random = UnityEngine.Random;

public class Vehicle : MonoBehaviour
{
    private Vector3 target;
    private Vector3 position;
    private Spawner spawner;
    private Transform nearestFood;
    private GameManager gm;
    private float health;
    private float score;
    private float viewRadius;


    private void Start()
    {
        gm = FindObjectOfType<GameManager>();
        spawner = FindObjectOfType<Spawner>();
        health = gm.vehicleHealth;
        viewRadius = gm.viewRadius;

        position = transform.position;
        findTarget();
    }

    private void Update()
    {
        debugStuff();
        decreaseHealth();

        if (looking4Food(viewRadius))
        {
            moveToTarget();
        }
        else
        {
            wanderAround();
        }
    }

    private void wanderAround()
    {
        throw new NotImplementedException();
    }

    private bool looking4Food(float radius)
    {
        findTarget();
        float dist = Vector3.Distance(target, position);
        Debug.Log(dist);
        if (dist <= radius)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void debugStuff()
    {
        Debug.DrawLine(position, target, Color.red);
    }

    private void decreaseHealth()
    {
        score += Time.deltaTime;
        health -= Time.deltaTime;
        var col = gameObject.GetComponent<SpriteRenderer>().color;
        col.a = health / gm.vehicleHealth;
        gameObject.GetComponent<SpriteRenderer>().color = col;
        if (health < 0)
        {
            Destroy(this.gameObject);
            Debug.Log("Score: " + score);
        }
    }

    private void findTarget()
    {
        target = new Vector3(0, 0, position.z);
        float dist = float.PositiveInfinity;

        int index = -1;
        List<Transform> foodList = spawner.getAllFoods();

        for (int i = 0; i < foodList.Count; i++)
        {
            if (foodList[i] != null && Vector3.Distance(foodList[i].position, position) < dist)
            {
                dist = Vector3.Distance(foodList[i].position, position);
                index = i;
            }
        }

        nearestFood = foodList[index];
        target = nearestFood.position;
    }


    private void moveToTarget()
    {
        position = transform.position;

        transform.position = Vector3.MoveTowards(position, target, gm.vehicleSpeed * Time.deltaTime);


        if (Vector3.Distance(position, target) < 0.1f)
        {
            Destroy(nearestFood.gameObject);
//            spawner.addNewFood();
            health += 3;
        }
    }
}