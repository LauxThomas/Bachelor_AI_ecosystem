using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Boid : MonoBehaviour
{
    Vector3 velocity = Vector3.zero;
    Vector3 acceleration = Vector3.zero;
    private Vector3 desired = Vector3.zero;
    private Vector3 target = Vector3.zero;
    private Vector3 steer = Vector3.zero;
    private Rigidbody rb;
    private foodSpawner foodSpawner;
    private GameObject nearestFoodObject;
    private float health;
    private GameManager gm;

    public float maxSpeed = 5; //5
    public float maxforce = 10; //10
    public float arrivingRadius = 3;

    private void Start()
    {
        gm = FindObjectOfType<GameManager>();
        health = gm.vehicleHealth;
        foodSpawner = FindObjectOfType<foodSpawner>();
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        updateValues();

        seekTarget();
    }

    private void seekTarget()
    {
        desired = target - transform.position;
        desired = desired.normalized;
        float dist = Vector3.Distance(transform.position, target);
        //Arriving behaviour
        if (dist < arrivingRadius)
        {
            float mult = HelperFunctions.remap(dist, 0, arrivingRadius, 0, maxSpeed);
            desired *= mult;
        }
        else
        {
            desired *= maxSpeed;
        }

        //Addforce:
        steer = desired - velocity;
        steer = Vector3.ClampMagnitude(steer, maxforce);
        rb.AddForce(steer);

        //Debug:
        Debug.DrawLine(transform.position, steer, Color.red);
        Debug.DrawLine(transform.position, desired, Color.blue);
        Debug.DrawLine(transform.position, velocity, Color.green);
    }


    private void updateValues()
    {
        health -= Time.deltaTime;
        velocity = rb.velocity;
        target = findNearestFood();
        recoloringBoid();
    }

    private void recoloringBoid()
    {
        var col = Color.Lerp(Color.red, Color.green, health / gm.vehicleHealth);
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        sr.color = col;
    }

    private Vector3 findNearestFood()
    {
        int index = -1;
        float dist = float.PositiveInfinity;
        List<GameObject> edibles = foodSpawner.getEdibles();
        for (int i = 0; i < edibles.Count; i++)
        {
            float distToFood = Vector3.Distance(transform.position, edibles[i].transform.position);
            if (distToFood < dist)
            {
                dist = distToFood;
                index = i;
            }
        }

        if (index != -1)
        {
            nearestFoodObject = edibles[index];
            if (dist < 0.5f)
            {
                modifyHealth(foodSpawner.isPoison(nearestFoodObject));
                foodSpawner.removeEdible(nearestFoodObject);
                Destroy(nearestFoodObject.gameObject);
            }

            return nearestFoodObject.transform.position;
        }

        return Vector3.zero;
    }

    private void modifyHealth(bool atePoison)
    {
        health += atePoison ? -50 : 50;
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }
}