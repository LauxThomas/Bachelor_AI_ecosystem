using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using DefaultNamespace;
using UnityEditor.Rendering;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

public class Bibit : MonoBehaviour
{
    public float energy = 150;
    private float age = 0;
    public float ageModifier;

    private NeuralNetwork brain;

    private const float STARTENERGY = 150f;
    private const float MINIMUMSURVIVALENERGY = 100f;

    private const String NAME_IN_BIAS = "bias";
    private const String NAME_IN_DISTTONEARESTFOOD = "Distance to nearest Food";
    private const String NAME_IN_DISTTONEARESTPOISON = "istance to nearest Poison";
    private const String NAME_IN_NEARESTFOODVECTORX = "Occlusion Feeler";
    private const String NAME_IN_ENERGY = "Energy";
    private const String NAME_IN_AGE = "Age";
    private const String NAME_IN_FORWARDVECTORX = "Genetic Difference";
    private const String NAME_IN_FORWARDVECTORY = "Genetic Difference";
    private const String NAME_IN_NEARESTFOODVECTORY = "Was Attacked";
    private const String NAME_IN_NEARESTPOISONVECTORX = "Water On Feeler";
    private const String NAME_IN_NEARESTPOISONVECTORY = "Water On Creature";

    private const String NAME_OUT_BIRTH = "Birth";
    private const String NAME_OUT_ROTATE = "Rotate";
    private const String NAME_OUT_FORWARD = "Forward";
    private const String NAME_OUT_FEELERANGLE = "Feeler Angle";
    private const String NAME_OUT_ATTACK = "Attack";
    private const String NAME_OUT_EAT = "Eat";

    private InputNeuron inBias = new InputNeuron();
    private InputNeuron inDistToNearestFood = new InputNeuron();
    private InputNeuron inDistToNearestPoison = new InputNeuron();
    private InputNeuron inNearestFoodVectorX = new InputNeuron();
    private InputNeuron inEnergy = new InputNeuron();
    private InputNeuron inAge = new InputNeuron();
    private InputNeuron inForwardVectorX = new InputNeuron();
    private InputNeuron inForwardVectorY = new InputNeuron();
    private InputNeuron inNearestFoodVectorY = new InputNeuron();
    private InputNeuron inNearestPoisonVectorX = new InputNeuron();
    private InputNeuron inNearestPoisonVectorY = new InputNeuron();

    public WorkingNeuron outBirth = new WorkingNeuron();
    public WorkingNeuron outRotate = new WorkingNeuron();
    public WorkingNeuron outForward = new WorkingNeuron();
    public WorkingNeuron outFeelerangle = new WorkingNeuron();
    public WorkingNeuron outAttack = new WorkingNeuron();
    public WorkingNeuron outEat = new WorkingNeuron();

    private Color color;

    private Rigidbody2D rb;
    private float rotation;
    private float speed = 10;
    private float force = 10;
    private FoodProducer foodProducer;
    private BibitProducer bibitProducer;
    private Vector3 lu;
    private Vector3 lo;
    private Vector3 ru;
    private Vector3 ro;
    private float distToNearestFood;
    private GameObject nearestFood;
    private float ROTATIONCOST = 0.1f;
    private float MOVECOST = 0.1f;
    public float debugValue;
    private float distToNearestPoison;
    private GameObject nearestPoison;
    private Vector3 vecToNearestFood;
    private Vector3 vecToNearestPoison;


    private void Start()
    {
        lu = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0));
        lo = Camera.main.ViewportToWorldPoint(new Vector3(0, 1, 0));
        ru = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, 0));
        ro = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, 0));
        rb = GetComponent<Rigidbody2D>();
        foodProducer = FindObjectOfType<FoodProducer>();
        bibitProducer = FindObjectOfType<BibitProducer>();
    }

    public void pseudoConstructor1()
    {
        inBias.setName(NAME_IN_BIAS);
        inDistToNearestFood.setName(NAME_IN_DISTTONEARESTFOOD);
        inDistToNearestPoison.setName(NAME_IN_DISTTONEARESTPOISON);
        inNearestFoodVectorX.setName(NAME_IN_NEARESTFOODVECTORX);
        inEnergy.setName(NAME_IN_ENERGY);
        inAge.setName(NAME_IN_AGE);
        inForwardVectorX.setName(NAME_IN_FORWARDVECTORX);
        inNearestFoodVectorY.setName(NAME_IN_NEARESTFOODVECTORY);
        inNearestPoisonVectorX.setName(NAME_IN_NEARESTPOISONVECTORX);
        inNearestPoisonVectorY.setName(NAME_IN_NEARESTPOISONVECTORY);

        outBirth.setName(NAME_OUT_BIRTH);
        outRotate.setName(NAME_OUT_ROTATE);
        outForward.setName(NAME_OUT_FORWARD);
        outFeelerangle.setName(NAME_OUT_FEELERANGLE);
        outAttack.setName(NAME_OUT_ATTACK);
        outEat.setName(NAME_OUT_EAT);

        brain = new NeuralNetwork();
        brain.addInputNeuron(inBias);
        brain.addInputNeuron(inDistToNearestFood);
        brain.addInputNeuron(inDistToNearestPoison);
        brain.addInputNeuron(inNearestFoodVectorX);
        brain.addInputNeuron(inEnergy);
        brain.addInputNeuron(inAge);
        brain.addInputNeuron(inForwardVectorX);
        brain.addInputNeuron(inForwardVectorY);
        brain.addInputNeuron(inNearestFoodVectorY);
        brain.addInputNeuron(inNearestPoisonVectorX);
        brain.addInputNeuron(inNearestPoisonVectorY);

        brain.generateHiddenNeurons(10);

        brain.addOutputNeuron(outBirth);
        brain.addOutputNeuron(outRotate);
        brain.addOutputNeuron(outForward);
        brain.addOutputNeuron(outFeelerangle);
        brain.addOutputNeuron(outAttack);
        brain.addOutputNeuron(outEat);

        brain.generateFullMesh();
        brain.randomizeAllWeights();
//            CalculateFellerPos();
        color = new Color(Random.value, Random.value, Random.value);
    }

    public void pseudoConstructor2(Bibit mother)
    {
        this.transform.position = mother.transform.position;
        brain = new NeuralNetwork(); //DEBUGGING
        this.brain = mother.brain.cloneFullMesh();
        this.energy = 150;

        inBias = brain.getInputNeuronFromName(NAME_IN_BIAS);
        inDistToNearestFood = brain.getInputNeuronFromName(NAME_IN_DISTTONEARESTFOOD);
        inDistToNearestPoison = brain.getInputNeuronFromName(NAME_IN_DISTTONEARESTPOISON);
        inNearestFoodVectorX = brain.getInputNeuronFromName(NAME_IN_NEARESTFOODVECTORX);
        inNearestFoodVectorY = brain.getInputNeuronFromName(NAME_IN_NEARESTFOODVECTORY);
        inEnergy = brain.getInputNeuronFromName(NAME_IN_ENERGY);
        inAge = brain.getInputNeuronFromName(NAME_IN_AGE);
        inForwardVectorX = brain.getInputNeuronFromName(NAME_IN_FORWARDVECTORX);
        inForwardVectorY = brain.getInputNeuronFromName(NAME_IN_FORWARDVECTORY);
        inNearestPoisonVectorX = brain.getInputNeuronFromName(NAME_IN_NEARESTPOISONVECTORX);
        inNearestPoisonVectorY = brain.getInputNeuronFromName(NAME_IN_NEARESTPOISONVECTORY);

        outBirth = brain.getOutputNeuronFromName(NAME_OUT_BIRTH);
        outRotate = brain.getOutputNeuronFromName(NAME_OUT_ROTATE);
        outForward = brain.getOutputNeuronFromName(NAME_OUT_FORWARD);
        outFeelerangle = brain.getOutputNeuronFromName(NAME_OUT_FEELERANGLE);
        outAttack = brain.getOutputNeuronFromName(NAME_OUT_ATTACK);
        outEat = brain.getOutputNeuronFromName(NAME_OUT_EAT);

//            CalculateFeelerPos();
        for (int i = 0; i < 10; i++)
        {
            brain.RandomMutation(0.5f);
        }

        float r = mother.color.r;
        float g = mother.color.g;
        float b = mother.color.b;

        r += Random.value * 0.1f - 0.05f;
        g += Random.value * 0.1f - 0.05f;
        b += Random.value * 0.1f - 0.05f;
        r = Mathf.Clamp(r, 0, 1);
        g = Mathf.Clamp(g, 0, 1);
        b = Mathf.Clamp(b, 0, 1);

        color = new Color(r, g, b);
    }


    private void Update()
    {
        ageModifier += Time.deltaTime / 2;
        readSensors();
        updateBrain();
        executeAction();
        flipIfNecessary();
    }

    private void executeAction()
    {
        if (gameObject != null)
        {
            float costMult = 1;
            actRotate(costMult);
            actMove(costMult);
            actBirth();
//        actFeelerRotate();
            actEat(costMult, nearestFood);
            age += Time.deltaTime;
        }
    }

    private void actEat(float costMult, GameObject o)
    {
        float eatWish = outEat.getValue();
        if (eatWish > 0)
        {
            nom(eatWish, nearestFood);
        }
    }

    private void nom(float eatWish, GameObject o)
    {
        if (o != null)
        {
            float dist = Vector3.Distance(o.transform.position, transform.position);
            if (dist < 0.5f)
            {
                bool wasFood = nearestFood.CompareTag("food");
                energy += wasFood ? 10 : -20;
                foodProducer.removeFood(nearestFood);
                Destroy(nearestFood);
                foodProducer.createFoods(1);
            }
        }
    }

    private void actBirth()
    {
        float birthWish = outBirth.getValue();
        if (birthWish > 0)
        {
            if (energy > STARTENERGY + MINIMUMSURVIVALENERGY * 1.1f)
            {
                bibitProducer.spawnChild(gameObject);
                energy -= STARTENERGY;
            }
        }
    }

    private void actMove(float costMult)
    {
        Vector3 forceToAdd = outForward.getValue() * outForward.getValue() * speed * transform.up.normalized;
        if (float.IsNaN(forceToAdd.x) || float.IsNaN(forceToAdd.y))
        {
            forceToAdd = new Vector3(Random.value * speed, Random.value * speed);
        }

        rb.AddForce(forceToAdd);
        if (rb.velocity.magnitude > 10)
        {
            rb.velocity = rb.velocity.normalized * 10;
        }

        energy -= Mathf.Abs(outForward.getValue() * outForward.getValue() * MOVECOST * ageModifier);
    }

    private void actRotate(float costMult)
    {
        float rotateForce = outRotate.getValue();
        rotateForce -= 0.5f;
        if (float.IsNaN(rotateForce))
        {
            rotateForce = Random.Range(-0.5f, 0.5f);
        }

        debugValue = rotateForce;

        transform.Rotate(0, 0, rotateForce * force, Space.Self);
        energy -= Mathf.Abs(rotateForce * ROTATIONCOST * ageModifier);
    }

    private void updateBrain()
    {
        brain.invalidate();
        inBias.setValue(1);

        inDistToNearestFood.setValue(distToNearestFood);
        inDistToNearestPoison.setValue(distToNearestPoison); //todo
        inNearestFoodVectorX.setValue(vecToNearestFood.x * distToNearestFood); //todo
        inNearestFoodVectorY.setValue(vecToNearestFood.y * distToNearestFood); //todo
        inNearestPoisonVectorX.setValue(vecToNearestPoison.x * distToNearestPoison); //todo:
        inNearestPoisonVectorY.setValue(vecToNearestPoison.y * distToNearestPoison); //todo
        inEnergy.setValue((energy - MINIMUMSURVIVALENERGY) / (STARTENERGY - MINIMUMSURVIVALENERGY));
        inAge.setValue(age);
        inForwardVectorX.setValue(transform.forward.x); //todo
        inForwardVectorY.setValue(transform.forward.y); //todo
    }

    private void readSensors()
    {
        //Kill if necessary:
        if (energy < 100)
        {
            bibitProducer.removeBibit(gameObject);
            Destroy(gameObject);
        }
        //if still alive:
        else
        {
//            color.a = (energy - MINIMUMSURVIVALENERGY) /(STARTENERGY - MINIMUMSURVIVALENERGY); //set coloralpha to healthpercentage
            GetComponent<SpriteRenderer>().color = color;
            List<GameObject> allFoods = foodProducer.getAllFoods();
            distToNearestFood = float.PositiveInfinity;
            distToNearestPoison = float.PositiveInfinity;
            for (int i = 0; i < allFoods.Count; i++)
            {
                float dist = Vector3.Distance(transform.position, allFoods[i].transform.position);

                if (dist < distToNearestFood)
                {
                    if (allFoods[i].CompareTag("food"))
                    {
                        distToNearestFood = dist;
                        nearestFood = allFoods[i];
                    }
                }

                if (dist < distToNearestPoison)
                {
                    if (allFoods[i].CompareTag("poison"))
                    {
                        distToNearestPoison = dist;
                        nearestPoison = allFoods[i];
                    }
                }
            }
        }


        if (distToNearestFood < 0.5f)
        {
            eat(nearestFood, true);
        }
        else if (distToNearestFood < float.PositiveInfinity && nearestFood != null)
        {
            vecToNearestFood = nearestFood.transform.position - transform.position;
        }

        if (distToNearestPoison < 0.5f)
        {
            eat(nearestPoison, false);
        }
        else if (distToNearestPoison < float.PositiveInfinity && nearestPoison != null)
        {
            vecToNearestPoison = nearestPoison.transform.position - transform.position;
        }

//        Debug.DrawLine(transform.position, transform.position + vecToNearestFood, Color.green);
//        Debug.DrawLine(transform.position, transform.position + vecToNearestPoison, Color.red);
    }


    private void flipIfNecessary()
    {
        Vector3 pos = transform.position;
        if (pos.x > ru.x)
        {
            pos.x = lu.x;
        }
        else if (pos.x < lu.x)
        {
            pos.x = ru.x;
        }

        else if (pos.y > lo.y)
        {
            pos.y = lu.y;
        }
        else if (pos.y < lu.y)
        {
            pos.y = lo.y;
        }

        if (pos != transform.position)
        {
            transform.position = pos;
        }
    }

//    private void checkInputs()
//    {
//        if (Input.GetKey(KeyCode.W))
//        {
//            moveForward(1);
//        }
//
//        if (Input.GetKey(KeyCode.S))
//        {
//            moveForward(-1);
//        }
//
//        if (Input.GetKey(KeyCode.A))
//        {
//            rotateClockwise(1);
//        }
//
//        if (Input.GetKey(KeyCode.D))
//        {
//            rotateClockwise(-1);
//        }
//    }

    private void rotateClockwise(float p0)
    {
        p0 *= force;
        transform.Rotate(0, 0, p0, Space.World);
    }

    private void moveForward(int i)
    {
        rb.AddForce(speed * i * transform.up.normalized);
        if (rb.velocity.magnitude > 10)
        {
            rb.velocity = rb.velocity.normalized * 10;
        }
    }


    private void eat(GameObject collisionGameObject, Boolean wasFood)
    {
        foodProducer.removeFood(collisionGameObject);
        Destroy(collisionGameObject);
        energy += wasFood ? 40 : -50;
    }
}