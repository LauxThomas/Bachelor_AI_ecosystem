using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;

public class Boid : MonoBehaviour
{
    public int itemsNearby = 0;

    public int gen = 0;
    private Vector3 velocity = Vector3.zero;
    private Vector3 acceleration = Vector3.zero;
    private Vector3 desired = Vector3.zero;
    private Vector3 target = Vector3.zero;
    private Vector3 steer = Vector3.zero;
    private Rigidbody rb;
    private foodSpawner foodSpawner;
    private vehicleSpawner vehicleSpawner;
    private GameObject nearestFoodObject;
    private GameManager gm;
    private float cloningRate;
    [SerializeField] private List<float> dna;
    [SerializeField] private float health = GameManager.staticVehicleHealth;
    [SerializeField] private float maxHealth;
    [SerializeField] private float viewRadius = GameManager.staticViewRadius;
    [SerializeField] private float maxSpeed = GameManager.staticVehicleMaxSpeed; //5
    [SerializeField] private float maxForce = GameManager.staticVehicleMaxForce;


    private float arrivingRadius = 3;
    private SpriteRenderer sr;
    private Vector3 position;
    private Vector3 wanderTarget;
    private Vector3 randomVector;
    private bool foundTarget;


    private void Start()
    {
        gameObject.name = "Werner der " + gen + ".";
        sr = GetComponent<SpriteRenderer>();
        gm = FindObjectOfType<GameManager>();
        foodSpawner = FindObjectOfType<foodSpawner>();
        vehicleSpawner = FindObjectOfType<vehicleSpawner>();
        rb = GetComponent<Rigidbody>();
        maxHealth = health;
        cloningRate = gm.cloningRate;
        InvokeRepeating("cloneMe", cloningRate, cloningRate);
        InvokeRepeating("createRandomVector3", 1, 1);

        randomVector = new Vector3(Random.Range(-gm.getWindowWidth(), gm.getWindowWidth()),
            Random.Range(-gm.getWindowHeight(), gm.getWindowHeight()), 0);
        setDna();
    }

    private void setDna()
    {
        if (dna.Count==0)
        {
            dna.Add(Random.Range(-5, 5));
            dna.Add(Random.Range(-5, 5));
        }
    }


    private void behaviours(List<GameObject> good, List<GameObject> bad)
    {
        Vector3 steerG = eat(good);
        Vector3 steerB = eat(bad);

        steerG *= dna[0];
        steerB *= dna[1];

        applyForce(steerG);
        Debug.DrawLine(transform.position, steerG, Color.green);
        applyForce(steerB);
        Debug.DrawLine(transform.position, steerB, Color.red);
    }

    private void Update()
    {
        updateValues();

//        checkForFoodInRange();
//        eat(foodSpawner.getEdibles());
//        eat(foodSpawner.getPoisons());
        behaviours(foodSpawner.getEdibles(), foodSpawner.getPoisons());
    }

    private void checkForFoodInRange()
    {
        float dist = Vector3.Distance(target, transform.position);
        if (dist < viewRadius)
        {
            eat(foodSpawner.getEdibles());
        }
        else
        {
            wanderAround();
        }
    }

    private Vector3 eat(List<GameObject> list)
    {
        float record = float.PositiveInfinity;
        int closest = -1;
        for (int i = 0; i < list.Count; i++)
        {
            float d = Vector3.Distance(transform.position, list[i].transform.position);
            if (d < record)
            {
                record = d;
                closest = i;
            }
        }

        if (record < 0.5f)
        {
            nom(list[closest]);
            return seekTarget(randomVector);
        }

        if (record < viewRadius)
        {
            desired = list[closest].transform.position - transform.position;
//            desired = list[closest].transform.position;
            foundTarget = true;
            return seekTarget(desired);
        }

//        return Vector3.zero;
        return seekTarget(randomVector);
//
//        if (!foundTarget)
//        {
////            wanderAround();
//        }
    }

    private void nom(GameObject eatenObject)
    {
        modifyHealth(foodSpawner.isPoison(eatenObject));
        foodSpawner.removeObject(eatenObject);
        Destroy(eatenObject.gameObject);
    }

    private void wanderAround()
    {
        sr.color = Color.black;
        seekTarget(randomVector);
    }

    private Vector3 seekTarget(Vector3 desired)
    {
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
        steer = Vector3.ClampMagnitude(steer, maxForce);
        flipIfNecessary();
        return steer;
        applyForce(steer);

        //Debug:
//        Debug.DrawLine(transform.position, steer, Color.red);
//        Debug.DrawLine(transform.position, desired, Color.blue);
//        Debug.DrawLine(transform.position, velocity, Color.green);
    }

    private void applyForce(Vector3 forceToAdd)
    {
        Debug.DrawLine(transform.position, forceToAdd, Color.magenta, 0.05f);
        rb.AddForce(forceToAdd);
        if (rb.velocity.magnitude > maxSpeed)
        {
            rb.velocity = rb.velocity.normalized * maxSpeed;
        }
    }

    private void flipIfNecessary()
    {
        Vector3 pos = transform.position;
        float x = pos.x;
        float y = pos.y;
        if (x > gm.getWindowWidth())
        {
            transform.position = new Vector3(-gm.getWindowWidth(), y, 0);
            makeNewRandomVector();
        }
        else if (x < -gm.getWindowWidth())
        {
            transform.position = new Vector3(gm.getWindowWidth(), y, 0);
            makeNewRandomVector();
        }
        else if (y > gm.getWindowHeight())
        {
            transform.position = new Vector3(x, -gm.getWindowHeight(), 0);
            makeNewRandomVector();
        }
        else if (y < -gm.getWindowHeight())
        {
            transform.position = new Vector3(x, gm.getWindowHeight(), 0);
            makeNewRandomVector();
        }
    }

    private IEnumerator createRandomVector3()
    {
        yield return new WaitForSecondsRealtime(1);
        randomVector = new Vector3(Random.Range(-gm.getWindowWidth(), gm.getWindowWidth()),
            Random.Range(-gm.getWindowHeight(), gm.getWindowHeight()), 0);
    }

    void makeNewRandomVector()
    {
        randomVector = new Vector3(Random.Range(-gm.getWindowWidth(), gm.getWindowWidth()),
            Random.Range(-gm.getWindowHeight(), gm.getWindowHeight()), 0);
    }


    private void updateValues()
    {
        if (vehicleSpawner.getVehicleCount() > 100)
        {
            health -= Time.deltaTime * gm.healthModifier * 5;
        }
        else
        {
            health -= Time.deltaTime * gm.healthModifier;
        }

//        velocity = rb.velocity;
//        target = findNearestFood();
        recoloringBoid();
        drawSeekRadii();

        //kill if necessary
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void drawSeekRadii()
    {
        float radius = viewRadius / 2;
        int numSegments = 128;

        LineRenderer lineRenderer = gameObject.GetComponent<LineRenderer>();
        Color c1 = Color.green;
//        lineRenderer.material = new Material(Shader.Find("Particles/Additive"));
        lineRenderer.SetColors(c1, c1);
        lineRenderer.SetWidth(0.1f, 0.1f);
        lineRenderer.SetVertexCount(numSegments + 1);
        lineRenderer.useWorldSpace = false;

        float deltaTheta = (float) (2.0 * Mathf.PI) / numSegments;
        float theta = 0f;

        for (int i = 0; i < numSegments + 1; i++)
        {
            float x = radius * Mathf.Cos(theta);
            float z = radius * Mathf.Sin(theta);
            Vector3 pos = new Vector3(x, z, 0);
            lineRenderer.SetPosition(i, pos);
            theta += deltaTheta;
        }
    }

    private void recoloringBoid()
    {
        sr.color = Color.Lerp(Color.red, Color.green, health / maxHealth);
    }

    private void modifyHealth(bool atePoison)
    {
        health += atePoison ? -20 : 50;
//        if (health > maxHealth * 1.5f)
//        {
//            health = maxHealth;
//        }
    }

    private void cloneMe()
    {
        GameObject child = vehicleSpawner.cloneThis(gameObject);
        mutateMe(child);
    }

    private void mutateMe(GameObject child)
    {
        Boid childStats = child.GetComponent<Boid>();
        childStats.health = maxHealth + HelperFunctions.randomBinominal() * 0.5f * maxHealth;
        childStats.maxHealth = childStats.health;
        childStats.viewRadius = viewRadius + HelperFunctions.randomBinominal() * 0.5f * viewRadius;
        childStats.maxSpeed = maxSpeed + HelperFunctions.randomBinominal() * 0.5f * maxSpeed;
        childStats.maxForce = maxForce + HelperFunctions.randomBinominal() * 0.5f * maxForce;
        childStats.gen = gen + 1;
        childStats.gameObject.name = gameObject.name = "Werner der " + childStats.gen + ".";
        for (int i = 0; i < dna.Count; i++)
        {
            childStats.dna[i] = dna[i] + HelperFunctions.randomBinominal() * 0.5f * dna[i];
        }
    }
}