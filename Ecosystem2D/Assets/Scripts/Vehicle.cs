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
    private const float Speed = 5;
    private GameManager gm;
    private Spawner spawner;
    private Transform nearestFood;

    private void Start()
    {
        gm = FindObjectOfType<GameManager>();
        spawner = FindObjectOfType<Spawner>();

        position = transform.position;
        findTarget();
    }

    private void findTarget()
    {
        target = new Vector3(0, 0, position.z);
        float dist = float.PositiveInfinity;

        int index = -1;
        List<Transform> foodList = spawner.allFoods;

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

    private void Update()
    {
        moveToTarget();
    }

    private void moveToTarget()
    {
        findTarget();
        position = transform.position;
        transform.position = Vector3.MoveTowards(position, target, Speed * Time.deltaTime);
        if (Vector3.Distance(position, target) < 0.3f)
        {
            Destroy(nearestFood.gameObject);
            spawner.addNewFood();
        }
    }

}