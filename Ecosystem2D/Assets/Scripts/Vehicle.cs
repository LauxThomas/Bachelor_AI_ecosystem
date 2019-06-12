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
    private VehicleConfig conf;
    private float health;
    private float score;
    private float viewRadius;
    private Vector3 wanderTarget;
    public Vector3 velocity;
    public Vector3 acceleration;


    private void Start()
    {
        gm = FindObjectOfType<GameManager>();
        spawner = FindObjectOfType<Spawner>();
        conf = FindObjectOfType<VehicleConfig>();
        health = gm.vehicleHealth;
        viewRadius = gm.viewRadius;
        velocity = new Vector3(Random.Range(-gm.getWindowWidth(), gm.getWindowWidth()),
            Random.Range(-gm.getWindowHeight(), gm.getWindowHeight()), 0);

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
        Debug.Log("I'm wandering");
        acceleration = Wander();
        acceleration = Vector3.ClampMagnitude(acceleration, conf.maxAcceleration);
        velocity += acceleration * Time.deltaTime;
        velocity = Vector3.ClampMagnitude(velocity, gm.vehicleMaxSpeed);
        position = position + velocity * Time.deltaTime;
        conf.wrapAround(ref position, -gm.getWindowWidth(), gm.getWindowWidth(), -gm.getWindowHeight(), gm.getWindowHeight());
        transform.position = position;
    }

    protected Vector3 Wander()
    {
        position = transform.position;
        float jitter = conf.wanderJitter * Time.deltaTime;
        wanderTarget += new Vector3(conf.randomBinominal() * jitter, conf.randomBinominal() * jitter, 0);
        wanderTarget = wanderTarget.normalized;
        wanderTarget *= conf.wanderRadius;
        Vector3 targetInLocalSpace =
            wanderTarget + new Vector3(conf.randomBinominal() * jitter, conf.randomBinominal() * jitter, 0);
        Vector3 targetInWorldSpace = transform.TransformPoint(targetInLocalSpace);
        targetInWorldSpace -= position;
        return targetInWorldSpace.normalized;
    }

    private void randomizeTarget()
    {
        target = new Vector3(Random.Range(-gm.getWindowWidth(), gm.getWindowWidth()),
            Random.Range(-gm.getWindowHeight(), gm.getWindowHeight()), 0);
    }

    private bool looking4Food(float radius)
    {
        findTarget();
        float dist = Vector3.Distance(target, position);
//        Debug.Log(dist);
        if (dist <= radius)
        {
            return true;
        }

        return false;
    }

    private void debugStuff()
    {
        Debug.DrawLine(position, target, Color.red);
        Debug.DrawLine(position, wanderTarget, Color.blue);
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
//            Debug.Log("Score: " + score);
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

        if (index == -1)
        {
            return;
        }

        nearestFood = foodList[index];
        target = nearestFood.position;
    }


    private void moveToTarget()
    {
        position = transform.position;

        transform.position = Vector3.MoveTowards(position, target, gm.vehicleMaxSpeed * Time.deltaTime);


        if (Vector3.Distance(position, target) < 0.1f)
        {
            Destroy(nearestFood.gameObject);
//            spawner.addNewFood();
            health += 3;
        }
    }
}