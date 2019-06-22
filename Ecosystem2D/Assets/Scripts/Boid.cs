using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour
{
    public int itemsNearby = 0;

    private float mutationRate;
    public float sleepTime = 0;
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
    [SerializeField] private float maxSpeed = GameManager.staticVehicleMaxSpeed; //5
    [SerializeField] private float maxForce = GameManager.staticVehicleMaxForce;


    private float arrivingRadius = 3;
    private SpriteRenderer sr;
    private Vector3 position;
    private Vector3 wanderTarget;
    private Vector3 randomVector;
    private bool foundTarget;
    private GameObject vehicleParent;


    private void Start()
    {
        gameObject.name = "Werner der " + gen + ".";
        vehicleParent = GameObject.Find("Vehicles");
        sr = GetComponent<SpriteRenderer>();
        gm = FindObjectOfType<GameManager>();
        foodSpawner = FindObjectOfType<foodSpawner>();
        vehicleSpawner = FindObjectOfType<vehicleSpawner>();
        rb = GetComponent<Rigidbody>();
        maxHealth = health;
        cloningRate = gm.cloningRate;
        mutationRate = gm.mutationRate;
//        InvokeRepeating("cloneMe", cloningRate, cloningRate);
        InvokeRepeating("createRandomVector3", 1, 1);

        randomVector = new Vector3(Random.Range(-gm.getWindowWidth(), gm.getWindowWidth()),
            Random.Range(-gm.getWindowHeight(), gm.getWindowHeight()), 0);
        setDna();
    }

    private void Update()
    {
        updateValues();
        behaviours(foodSpawner.getEdibles(), foodSpawner.getPoisons(), dna[2], dna[3]);

        cloneMe();
    }

    private void setDna()
    {
        if (dna.Count == 0)
        {
            //foodWeight
            dna.Add(Random.Range(-5, 5));
            //poisonWeight
            dna.Add(Random.Range(-5, 5));
            //foodViewRadius
            dna.Add(Random.Range(0, 5));
            //poisonViewRadius
            dna.Add(Random.Range(0, 5));
        }

        for (int i = 0; i < dna.Count; i++)
        {
            if (dna[i] == 0)
            {
                dna[i] = HelperFunctions.randomBinominal(1);
            }
        }
    }


    private void behaviours(List<GameObject> good, List<GameObject> bad, float goodRadius, float badRadius)
    {
        Vector3 steerG = eat(good, goodRadius);
        Vector3 steerB = eat(bad, badRadius);

        steerG *= dna[0];
        steerB *= dna[1];

        applyForce(steerG);
        applyForce(steerB);
    }


    private Vector3 eat(List<GameObject> list, float sightRadius)
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

        if (record < sightRadius)
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
    }

    private void applyForce(Vector3 forceToAdd)
    {
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
        int count = vehicleParent.transform.childCount;
//        Debug.Log("Anzahl Vehicles: " + count);
        if (count > 100)
        {
            health -= Time.deltaTime * gm.healthDegen * count / 10f;
        }
        else if (count < 3)
        {
            vehicleSpawner.spawnOrganism();
        }
        else
        {
            health -= Time.deltaTime * gm.healthDegen;
        }

//        velocity = rb.velocity;
//        target = findNearestFood();
        recoloringBoid();
        if (gm.drawHalos)
        {
            drawSigthRadius(gameObject.transform.GetChild(0), Color.green, dna[2]);
            drawSigthRadius(gameObject.transform.GetChild(1), Color.red, dna[3]);
        }

        //kill if necessary
        if (health <= 0)
        {
            Vector3 spawnpos = transform.position;
            Destroy(gameObject);
            //Y U NO WORK
            foodSpawner.spawnFoodAt(spawnpos);
        }

        if (rb.IsSleeping())
        {
            sleepTime += Time.deltaTime;
            if (sleepTime > 5)
            {
                Debug.Log("work work death");
                Destroy(gameObject);
            }
        }
        else
        {
            sleepTime = 0;
        }
    }

    private void drawSigthRadius(Transform halo, Color col, float radius)
    {
        radius *= 4;
        col = new Color(col.r, col.g, col.b, 0.1f);
        halo.GetComponent<SpriteRenderer>().color = col;
        halo.transform.localScale = new Vector3(radius, radius, radius);

//        
//        radius /= 2;
//        int numSegments = 30;
//
//        LineRenderer lineRenderer = gameObject.GetComponent<LineRenderer>();
//        lineRenderer.material = lineRenderer.materials[1];
//        lineRenderer.SetColors(Color.white, Color.white);
//        lineRenderer.SetWidth(0.1f, 0.1f);
//        lineRenderer.SetVertexCount(numSegments + 1);
//        lineRenderer.useWorldSpace = false;
//
//        float deltaTheta = (float) (2.0 * Mathf.PI) / numSegments;
//        float theta = 0f;
//
//        for (int i = 0; i < numSegments + 1; i++)
//        {
//            float x = radius * Mathf.Cos(theta);
//            float z = radius * Mathf.Sin(theta);
//            Vector3 pos = new Vector3(x, z, 0);
//            lineRenderer.SetPosition(i, pos);
//            theta += deltaTheta;
//        }
    }

    private void recoloringBoid()
    {
        sr.color = Color.Lerp(Color.red, Color.green, health / maxHealth);
    }

    private void modifyHealth(bool atePoison)
    {
        health += atePoison ? -50 : 20;
//        if (health > maxHealth)
//        {
//            health = maxHealth;
//        }
    }

    private void cloneMe()
    {
        if (Random.value < 0.0025)
        {
            GameObject child = vehicleSpawner.cloneThis(gameObject);
            mutateMe(child);
        }
    }

    private void mutateMe(GameObject child)
    {
        Boid childStats = child.GetComponent<Boid>();
        childStats.gen = gen + 1;
        childStats.gameObject.name = gameObject.name = "Werner der " + childStats.gen + ".";

        if (Random.value < mutationRate)
        {
            childStats.maxHealth = maxHealth + HelperFunctions.randomBinominal(100) * GameManager.staticVehicleHealth;
            childStats.health = childStats.maxHealth;
        }
        else
        {
            childStats.maxHealth = maxHealth;
            childStats.health = maxHealth;
        }

        if (Random.value < mutationRate)
        {
            childStats.maxSpeed = maxSpeed + HelperFunctions.randomBinominal(100) * GameManager.staticVehicleMaxSpeed;
        }
        else
        {
            childStats.maxSpeed = maxSpeed;
        }

        if (Random.value < mutationRate)
        {
            childStats.maxForce = maxForce + HelperFunctions.randomBinominal(100) * GameManager.staticVehicleMaxForce;
        }
        else
        {
            childStats.maxForce = maxForce;
        }

        for (int i = 0; i < dna.Count; i++)
        {
            if (Random.value < mutationRate)
            {
                childStats.dna[i] = dna[i] + HelperFunctions.randomBinominal(100) * dna[i];
            }
            else
            {
                childStats.dna[i] = dna[i];
            }
        }
    }
}