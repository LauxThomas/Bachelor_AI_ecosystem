using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using Random = UnityEngine.Random;

public class Boid : MonoBehaviour
{
    Vector3 velocity = Vector3.zero;
    Vector3 acceleration = Vector3.zero;
    private Vector3 desired = Vector3.zero;
    private Vector3 target = Vector3.zero;
    private Vector3 steer = Vector3.zero;
    private Rigidbody rb;
    private foodSpawner foodSpawner;
    private vehicleSpawner vehicleSpawner;
    private GameObject nearestFoodObject;
    private float health;
    private GameManager gm;
    private float cloningRate = 20;
    private VehicleConfig conf;

    public float maxSpeed; //5
    public float maxforce = 10; //10
    public float arrivingRadius = 3;
    private SpriteRenderer sr;
    private Vector3 position;
    private Vector3 wanderTarget;
    private Vector3 randomVector;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        gm = FindObjectOfType<GameManager>();
        conf = FindObjectOfType<VehicleConfig>();
        foodSpawner = FindObjectOfType<foodSpawner>();
        vehicleSpawner = FindObjectOfType<vehicleSpawner>();
        rb = GetComponent<Rigidbody>();
        health = gm.vehicleHealth;
        maxSpeed = gm.vehicleMaxSpeed;
        InvokeRepeating("cloneMe", cloningRate, cloningRate);
        
        randomVector = new Vector3(Random.Range(-gm.windowWidth, gm.windowWidth),
            Random.Range(-gm.windowHeight, gm.windowHeight), 0);
        StartCoroutine(createRandomVector3());
    }

    private void Update()
    {
        updateValues();

        checkForFood();
    }

    private void checkForFood()
    {
        float dist = Vector3.Distance(target, transform.position);
        if (dist < gm.viewRadius)
        {
            seekTarget(false);
        }
        else
        {
            seekTarget(true);
        }
    }

    private void seekTarget(bool shouldWander)
    {
        if (!shouldWander)
        {
            desired = target - transform.position;
        }
        else
        {
            desired = randomVector;
        }

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
        flipIfNecessary();

        //Debug:
//        Debug.DrawLine(transform.position, steer, Color.red);
//        Debug.DrawLine(transform.position, desired, Color.blue);
//        Debug.DrawLine(transform.position, velocity, Color.green);
    }

    private void flipIfNecessary()
    {
        Vector3 pos = transform.position;
        float x = pos.x;
        float y = pos.y;
        if (x > gm.windowWidth)
        {
            transform.position = new Vector3(-gm.windowWidth, y, 0);
        }
        else if (x < -gm.windowWidth)
        {
            transform.position = new Vector3(gm.windowWidth, y, 0);
        }
        else if (y > gm.windowHeight)
        {
            transform.position = new Vector3(x, -gm.windowHeight, 0);
        }
        else if (y < -gm.windowHeight)
        {
            transform.position = new Vector3(x, gm.windowHeight, 0);
        }
    }

    private IEnumerator createRandomVector3()
    {
        yield return new WaitForSecondsRealtime(2);
        randomVector = new Vector3(Random.Range(-gm.windowWidth, gm.windowWidth),
            Random.Range(-gm.windowHeight, gm.windowHeight), 0);
    }


    private void updateValues()
    {
        health -= Time.deltaTime * gm.healthModifier;
        velocity = rb.velocity;
        target = findNearestFood();
        recoloringBoid();
        
        //kill if necessary
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void recoloringBoid()
    {
//        var col = Color.Lerp(Color.red, Color.green, health / gm.vehicleHealth);
        sr.color = Color.Lerp(Color.red, Color.green, health / gm.vehicleHealth);
        ;
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
    }

    private void cloneMe()
    {
        vehicleSpawner.cloneThis(gameObject);
    }

    private void wanderAround()
    {
        Debug.Log("I'm wandering");
        acceleration = Wander();
        acceleration = Vector3.ClampMagnitude(acceleration, conf.maxAcceleration);
        velocity += acceleration * Time.deltaTime;
        velocity = Vector3.ClampMagnitude(velocity, maxSpeed);
        position = position + velocity * Time.deltaTime;
        conf.wrapAround(ref position, -gm.windowWidth, gm.windowWidth, -gm.windowHeight, gm.windowHeight);
        transform.position = position;
    }

    protected Vector3 Wander()
    {
        position = transform.position;
        float jitter = conf.wanderJitter * Time.deltaTime;
        wanderTarget = new Vector3(conf.randomBinominal() * jitter, conf.randomBinominal() * jitter, 0);
        wanderTarget = wanderTarget.normalized;
        wanderTarget *= conf.wanderRadius;
        Vector3 targetInLocalSpace =
            wanderTarget + new Vector3(conf.randomBinominal() * jitter, conf.randomBinominal() * jitter, 0);
        Vector3 targetInWorldSpace = transform.TransformPoint(targetInLocalSpace);
        targetInWorldSpace -= position;
        return targetInWorldSpace.normalized;
    }
}