using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.Profiling;
using Random = UnityEngine.Random;

[SuppressMessage("ReSharper", "IdentifierTypo")]
[SuppressMessage("ReSharper", "CommentTypo")]
public class Bibit : MonoBehaviour
{
    public double energy = 150;
    [SerializeField] private float age;
    public float ageModifier = 0.001f;
    public int generation;
    public String displayName;

    private NeuralNetwork brain;

    private const float STARTENERGY = 150f;
    private const float MINIMUMSURVIVALENERGY = 100f;

    private const String NAME_IN_BIAS = "bias";
    private const String NAME_IN_ENERGY = "Current Energy";
    private const String NAME_IN_AGE = "Current Age";
    private const String NAME_IN_MEMORY = "My in Memory";
    private const String NAME_IN_DISTTONEARESTPOISON = "Distance to nearest poison aka. water";
    private const String NAME_IN_ANGLETONEARESTPOISON = "Angle to nearest poison aka. water";
    private const String NAME_IN_FOODAMOUNTATCURRENTBLOCK = "Amount of food at current block";
    private const String NAME_IN_FOODAMOUNTINSIGHTRADIUS = "sum of food around";
    private const String NAME_IN_DISTTOMAXFOODBLOCKAROUND = "Distance to foodblock with max amount insight";
    private const String NAME_IN_ANGLETOMAXFOODBLOCKAROUND = "Angle to foodblock with max amount insight";
    private const String NAME_IN_FOODAMOUNTOFMAXFOODBLOCKAROUND = "Foodamount to foodblock with max amount insight";
    private const String NAME_IN_NUMBEROFBIBITSNEAR = "number of near bibits";
    private const String NAME_IN_DISTTONEARESTBIBIT = "Distance to nearest bibit";
    private const String NAME_IN_ANGLETONEARESTBIBIT = "Angle to nearest bibit";
    private const String NAME_IN_GENETICDIFFERENCETONEARESTBIBIT = "Genetic difference to nearest bibit";
    private const String NAME_IN_CENTERPOSITION = "Position of the worlds center";

//
    private const String NAME_OUT_BIRTH = "Birth";
    private const String NAME_OUT_ROTATE = "Rotate";
    private const String NAME_OUT_FORWARD = "Forward";
    private const String NAME_OUT_EAT = "Eat";
    private const String NAME_OUT_MEMORY = "My out Memory";
    private const String NAME_OUT_ATTACK = "Attack";

    private InputNeuron inBias = new InputNeuron();
    private InputNeuron inEnergy = new InputNeuron();
    private InputNeuron inAge = new InputNeuron();
    private InputNeuron inMemory = new InputNeuron();
    private InputNeuron inDistToNearestPoison = new InputNeuron();
    private InputNeuron inAngleToNearestPoison = new InputNeuron();
    private InputNeuron inFoodAmountAtCurrentBlock = new InputNeuron();
    private InputNeuron inFoodAmountInSightRadius = new InputNeuron();
    private InputNeuron inDistToMaxFoodBlockAround = new InputNeuron();
    private InputNeuron inAngleToMaxFoodBlockAround = new InputNeuron();
    private InputNeuron inFoodAmountOfMaxFoodBlockAround = new InputNeuron();
    private InputNeuron inNumberOfBibitsNear = new InputNeuron();
    private InputNeuron inDistToNearestBibit = new InputNeuron();
    private InputNeuron inAngleToNearestBibit = new InputNeuron();
    private InputNeuron inGeneticDifferenceToNearestBibit = new InputNeuron();
    private InputNeuron inCenterPosition = new InputNeuron();

    public WorkingNeuron outBirth = new WorkingNeuron();
    public WorkingNeuron outEat = new WorkingNeuron();
    public WorkingNeuron outRotate = new WorkingNeuron();
    public WorkingNeuron outForward = new WorkingNeuron();
    public WorkingNeuron outMemory = new WorkingNeuron();
    public WorkingNeuron outAttack = new WorkingNeuron();

    private Color color;

    private float rotation;
    private const float SPEED = 5;
    private const float FORCE = 5;
    private FoodProducer foodProducer;
    private BibitProducer bibitProducer;
    private Vector3 lu;
    private Vector3 lo;
    private Vector3 ru;
    private float distToNearestFood;

    private GameObject nearestFood;

    public double debugValue;
    private float distToNearestPoison;
    private GameObject nearestPoison;
    private Vector3 vecToNearestFood;
    private Vector3 vecToNearestPoison;
    private Vector3 forceToAdd;
    private double rotateForce;
    private float? angleToNearestFood;
    private float? angleToNearestPoison;
    private double foodAmountAtCurrentBlock;
    private double foodAmountInSightRadius;
    private double distToMaxFoodBlockAround;
    private double? angleToMaxFoodBlockAround;
    private double amountOfMaxFoodBlockAround;
    private int numberOfBibitsNear;
    private double distToNearestBibit;
    private double? angleToNearestBibit;
    private GameObject nearestBibit;
    private float geneticDifferenceToAttackedBibit;
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private int layerMask;
    private float rotationCost, moveCost, birthCost, eatCost, attackCost;
    private List<double> foodAmountAvailableAllFoods;
    private List<FoodStats> foodStatsPoison;
    private List<Vector3> transformsFood;
    private List<Vector3> transformsPoison;
    private List<GameObject> gameObjectsFood;
    private List<GameObject> gameObjectsPoison;


    private void Start()
    {
        lu = FoodProducer.lu;
        lo = FoodProducer.lo;
        ru = FoodProducer.ru;
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        gameObject.name = displayName + ", der " + generation;
        layerMask = LayerMask.GetMask("food", "poison");

        rotationCost = 1.2f;
        moveCost = 1.1f;
        birthCost = 0.8f;
        eatCost = 1.0f;
        attackCost = 0.8f;

        Profiler.BeginSample("MySampleGetAllFieldStats");
        getAllFieldStats();
        Profiler.EndSample();
        InvokeRepeating("updateFoodAvailable", 3, Random.Range(1f, 3f));
    }


    public void pseudoConstructor1()
    {
        inBias.setName(NAME_IN_BIAS);
        inEnergy.setName(NAME_IN_ENERGY);
        inAge.setName(NAME_IN_AGE);
        inMemory.setName(NAME_IN_MEMORY);
        inDistToNearestPoison.setName(NAME_IN_DISTTONEARESTPOISON);
        inAngleToNearestPoison.setName(NAME_IN_ANGLETONEARESTPOISON);
        inFoodAmountAtCurrentBlock.setName(NAME_IN_FOODAMOUNTATCURRENTBLOCK);
        inFoodAmountInSightRadius.setName(NAME_IN_FOODAMOUNTINSIGHTRADIUS);
        inDistToMaxFoodBlockAround.setName(NAME_IN_DISTTOMAXFOODBLOCKAROUND);
        inAngleToMaxFoodBlockAround.setName(NAME_IN_ANGLETOMAXFOODBLOCKAROUND);
        inFoodAmountOfMaxFoodBlockAround.setName(NAME_IN_FOODAMOUNTOFMAXFOODBLOCKAROUND);
        inNumberOfBibitsNear.setName(NAME_IN_NUMBEROFBIBITSNEAR);
        inDistToNearestBibit.setName(NAME_IN_DISTTONEARESTBIBIT);
        inAngleToNearestBibit.setName(NAME_IN_ANGLETONEARESTBIBIT);
        inGeneticDifferenceToNearestBibit.setName(NAME_IN_GENETICDIFFERENCETONEARESTBIBIT);
        inCenterPosition.setName(NAME_IN_CENTERPOSITION);

        outBirth.setName(NAME_OUT_BIRTH);
        outRotate.setName(NAME_OUT_ROTATE);
        outForward.setName(NAME_OUT_FORWARD);
        outEat.setName(NAME_OUT_EAT);
        outMemory.setName(NAME_OUT_MEMORY);
        outAttack.setName(NAME_OUT_ATTACK);

        brain = new NeuralNetwork();


        brain.addInputNeuron(inBias);
        brain.addInputNeuron(inEnergy);
        brain.addInputNeuron(inAge);
        brain.addInputNeuron(inMemory);
        brain.addInputNeuron(inDistToNearestPoison);
        brain.addInputNeuron(inAngleToNearestPoison);
        brain.addInputNeuron(inFoodAmountAtCurrentBlock);
        brain.addInputNeuron(inFoodAmountInSightRadius);
        brain.addInputNeuron(inDistToMaxFoodBlockAround);
        brain.addInputNeuron(inAngleToMaxFoodBlockAround);
        brain.addInputNeuron(inFoodAmountOfMaxFoodBlockAround);
        brain.addInputNeuron(inNumberOfBibitsNear);
        brain.addInputNeuron(inDistToNearestBibit);
        brain.addInputNeuron(inAngleToNearestBibit);
        brain.addInputNeuron(inGeneticDifferenceToNearestBibit);
        brain.addInputNeuron(inCenterPosition);

        brain.generateHiddenNeurons(20);

        brain.addOutputNeuron(outBirth);
        brain.addOutputNeuron(outRotate);
        brain.addOutputNeuron(outForward);
        brain.addOutputNeuron(outEat);
        brain.addOutputNeuron(outMemory);
        brain.addOutputNeuron(outAttack);


        brain.generateFullMesh();
        brain.randomizeAllWeights();
        color = new Color(Random.value, Random.value, Random.value);
    }

    public void pseudoConstructor2(Bibit mother)
    {
        transform.position = mother.transform.position;
        brain = mother.brain.cloneFullMesh();
        energy = 150;
        displayName = mother.displayName;
        generation = mother.getGeneration() + 1;


        inBias = brain.getInputNeuronFromName(NAME_IN_BIAS);
        inEnergy = brain.getInputNeuronFromName(NAME_IN_ENERGY);
        inAge = brain.getInputNeuronFromName(NAME_IN_AGE);
        inMemory = brain.getInputNeuronFromName(NAME_IN_MEMORY);
        inDistToNearestPoison = brain.getInputNeuronFromName(NAME_IN_DISTTONEARESTPOISON);
        inAngleToNearestPoison = brain.getInputNeuronFromName(NAME_IN_ANGLETONEARESTPOISON);
        inFoodAmountAtCurrentBlock = brain.getInputNeuronFromName(NAME_IN_FOODAMOUNTATCURRENTBLOCK);
        inFoodAmountInSightRadius = brain.getInputNeuronFromName(NAME_IN_FOODAMOUNTINSIGHTRADIUS);
        inDistToMaxFoodBlockAround = brain.getInputNeuronFromName(NAME_IN_DISTTOMAXFOODBLOCKAROUND);
        inAngleToMaxFoodBlockAround = brain.getInputNeuronFromName(NAME_IN_ANGLETOMAXFOODBLOCKAROUND);
        inFoodAmountOfMaxFoodBlockAround = brain.getInputNeuronFromName(NAME_IN_FOODAMOUNTOFMAXFOODBLOCKAROUND);
        inNumberOfBibitsNear = brain.getInputNeuronFromName(NAME_IN_NUMBEROFBIBITSNEAR);
        inDistToNearestBibit = brain.getInputNeuronFromName(NAME_IN_DISTTONEARESTBIBIT);
        inAngleToNearestBibit = brain.getInputNeuronFromName(NAME_IN_ANGLETONEARESTBIBIT);
        inGeneticDifferenceToNearestBibit = brain.getInputNeuronFromName(NAME_IN_GENETICDIFFERENCETONEARESTBIBIT);
        inCenterPosition = brain.getInputNeuronFromName(NAME_IN_CENTERPOSITION);

        outBirth = brain.getOutputNeuronFromName(NAME_OUT_BIRTH);
        outRotate = brain.getOutputNeuronFromName(NAME_OUT_ROTATE);
        outForward = brain.getOutputNeuronFromName(NAME_OUT_FORWARD);
        outEat = brain.getOutputNeuronFromName(NAME_OUT_EAT);
        outMemory = brain.getOutputNeuronFromName(NAME_OUT_MEMORY);
        outAttack = brain.getOutputNeuronFromName(NAME_OUT_ATTACK);

//            CalculateFeelerPos();
        for (int i = 0; i < 10; i++)
        {
            brain.RandomMutation(0.2f);
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
        rb.AddForce(Vector3.up * Time.deltaTime);
        energy -= Time.deltaTime * ageModifier;

        //Kill if necessary:
        if (energy < 100)
        {
            BibitProducer.removeBibit(gameObject);
            Destroy(gameObject);
        }

        else
        {
            flipIfNecessary();
            color.a = (float) ((energy - MINIMUMSURVIVALENERGY) /
                               (STARTENERGY - MINIMUMSURVIVALENERGY)); //set coloralpha to healthpercentage
            sr.color = color;

            //exponential growth:
            //int numOfBibits = BibitProducer.getNumberOfBibits();
            /*
            if (numOfBibits > 200)
            {
                ageModifier += ageModifier * Time.deltaTime * 0.1f * numOfBibits * 0.1f; //bibitcountmod
            }
            else
            {
                ageModifier += ageModifier * Time.deltaTime * 0.1f;
            }
            */
            ageModifier = (float) (-(3 / Math.Pow(Math.E, 0.1 * age - 10) + 1) + 3);
            ageModifier = Mathf.Clamp(ageModifier, 0.001f, 200);

            Profiler.BeginSample("readSensors");
            readSensors();
            Profiler.EndSample();
            Profiler.BeginSample("updateBrain");
            updateBrain();
            Profiler.EndSample();
            Profiler.BeginSample("executeAction");
            executeAction();
            Profiler.EndSample();
        }
    }

    private void executeAction()
    {
        bool isOnPoison = distToNearestPoison < distToNearestFood;
        actRotate(rotationCost, isOnPoison);
        actMove(moveCost, isOnPoison);
        actBirth(birthCost, isOnPoison);
        actEat(eatCost, isOnPoison);
        if (nearestBibit)
        {
            actAttack(nearestBibit, attackCost, isOnPoison);
        }

        age += Time.deltaTime;
    }

    private void actAttack(GameObject attackedBibit, float attackCosts, bool isOnPoison)
    {
        if (isOnPoison)
        {
            attackCosts *= 2;
        }

        geneticDifferenceToAttackedBibit = 0;
        if (angleToNearestBibit != null &&
            (Vector3.Distance(transform.position, attackedBibit.transform.position) < 0.3f * 0.3f &&
             Math.Abs((float) angleToNearestBibit) < 70))
        {
            geneticDifferenceToAttackedBibit +=
                Math.Abs(color.r - attackedBibit.GetComponent<SpriteRenderer>().color.r);
            geneticDifferenceToAttackedBibit +=
                Math.Abs(color.g - attackedBibit.GetComponent<SpriteRenderer>().color.g);
            geneticDifferenceToAttackedBibit +=
                Math.Abs(color.b - attackedBibit.GetComponent<SpriteRenderer>().color.b);
            if (geneticDifferenceToAttackedBibit > 0.2f)
            {
                {
                    double attackWish = outAttack.getValue() * Time.deltaTime * 30;

                    attackedBibit.GetComponent<Bibit>().energy -= attackWish;
                    energy += attackWish / attackCosts;
                }
            }
        }
    }

    private void actEat(float eatCost, bool isOnPoison)
    {
        if (isOnPoison)
        {
            eatCost *= 2;
        }

        double eatWish = outEat.getValue() * 30;
        if (eatWish <= 0) return;

        if (distToNearestFood < distToNearestPoison && distToNearestFood < 1)
        {
            energy += FoodProducer.eatFood(nearestFood, eatWish);
        }
        else if (distToNearestPoison < 1)
        {
            energy += FoodProducer.eatPoison(eatWish);
        }
    }

    private void actBirth(float birthCost, bool isOnPoison)
    {
        double birthWish = outBirth.getValue();
//        debugValue = birthWish;
        if (birthWish > 0)
        {
            if (energy > STARTENERGY + MINIMUMSURVIVALENERGY * birthCost * 1.15f)
            {
                BibitProducer.spawnChild(gameObject);
                if (isOnPoison)
                {
                    birthCost *= 3;
                }

                energy -= STARTENERGY * birthCost;
            }
        }
    }

    private void actMove(float moveCost, bool isOnPoison)
    {
        if (isOnPoison)
        {
            moveCost *= 3;
        }

        float speedForce = (float) outForward.getValue();
//        float remappedForward = (float) HelperFunctions.remap(outForward.getValue(), -1, 1, 0, 1); //not going backwards
//        float remappedForward = (float)outForward.getValue();
        forceToAdd = SPEED * speedForce * speedForce * 15 * Time.deltaTime * transform.up.normalized;
//        if (rb.velocity.magnitude > 10)
//        {
//            rb.velocity = rb.velocity.normalized * 10;
//        }

//        rb.AddForce(forceToAdd*Vector3.up.normalized);
        rb.AddForce(forceToAdd);

        energy -= forceToAdd.magnitude / SPEED / 10f * ageModifier * moveCost;
    }

    private void actRotate(float rotationCost, bool isOnPoison)
    {
        if (isOnPoison)
        {
            rotationCost *= 3;
        }

        rotateForce = outRotate.getValue() * 15 * Time.deltaTime;

        rb.SetRotation(rb.rotation + (float) rotateForce * FORCE);
        energy -= Mathf.Abs((float) (rotateForce * ageModifier * rotationCost));
    }

    private void updateBrain()
    {
        brain.invalidate();
        inBias.setValue(1);
        inEnergy.setValue(energy);
        inAge.setValue(age);
        inMemory.setValue(outMemory.getValue());
        inDistToNearestPoison.setValue(distToNearestPoison);
        if (angleToNearestPoison != null) inAngleToNearestPoison.setValue((float) angleToNearestPoison);
        inFoodAmountAtCurrentBlock.setValue(foodAmountAtCurrentBlock);
        inFoodAmountInSightRadius.setValue(foodAmountInSightRadius);
        inDistToMaxFoodBlockAround.setValue(distToMaxFoodBlockAround);
        if (angleToMaxFoodBlockAround != null) inAngleToMaxFoodBlockAround.setValue((float) angleToMaxFoodBlockAround);
        inFoodAmountOfMaxFoodBlockAround.setValue(amountOfMaxFoodBlockAround);
        inNumberOfBibitsNear.setValue(numberOfBibitsNear);
        inDistToNearestBibit.setValue(distToNearestBibit);
        if (angleToNearestBibit != null) inAngleToNearestBibit.setValue((double) angleToNearestBibit);
        Transform trans = transform;
        inCenterPosition.setValue(Vector3.SignedAngle(trans.up, Vector3.zero - trans.position,
            trans.forward));
    }

    private void readSensors()
    {
        bool temp = false;
        distToNearestPoison = float.PositiveInfinity;
        angleToNearestPoison = null;
        foodAmountAtCurrentBlock = 0;

        distToNearestFood = float.PositiveInfinity;
        angleToNearestFood = null;

        foodAmountInSightRadius = 0;
        distToMaxFoodBlockAround = float.PositiveInfinity;
        angleToMaxFoodBlockAround = null;
        amountOfMaxFoodBlockAround = 0;


        Transform trans = transform;
        Vector3 transPos = trans.position;
//        transPos.z = -0.3f;

        Profiler.BeginSample("Raycast&Hit");
        //do raycast, if hit=food bla, else
        RaycastHit2D hitUp = Physics2D.Raycast(transPos, transPos + trans.forward, 1f, layerMask);
//        Debug.DrawLine(transPos, hitUp.point, Color.red);
        if (hitUp.collider != null)
        {
            temp = true;
            if (hitUp.collider.CompareTag("food") &&
                hitUp.collider.gameObject.GetComponent<FoodStats>().foodAmountAvailable > 10)
            {
                nearestFood = hitUp.collider.gameObject;
                distToNearestFood = Vector3.Distance(transPos, nearestFood.transform.position);
            }
        }

        Profiler.EndSample();

//        Profiler.BeginSample("Raycast&NOTHit");
        if (temp)
        {
            for (int i = 0; i < gameObjectsFood.Count; i++)
            {
                Profiler.BeginSample("DistCalculationDist");
                float dist = Vector3.Distance(transPos, transformsFood[i]);
                Profiler.EndSample();

                if (dist < distToNearestFood && foodAmountAvailableAllFoods[i] > 10)
                {
                    Profiler.BeginSample("distAssignment");
                    distToNearestFood = dist;
                    Profiler.EndSample();

                    Profiler.BeginSample("nearestFoodAssignment");
                    nearestFood = gameObjectsFood[i];
                    Profiler.EndSample();
                }
            }
        }

//        Profiler.EndSample();

        if (nearestFood)
        {
            angleToNearestFood = Vector3.SignedAngle(trans.up, nearestFood.transform.position - transPos,
                trans.forward);
        }


        Profiler.BeginSample("PoisonStuff");
        for (int i = 0; i < gameObjectsPoison.Count; i++)
        {
            float dist = Vector3.Distance(transPos, transformsPoison[i]);
            if (dist < distToNearestPoison)
            {
                distToNearestPoison = dist;
                nearestPoison = gameObjectsPoison[i];
            }
        }

        Profiler.EndSample();

        if (nearestPoison)
        {
            angleToNearestPoison = Vector3.SignedAngle(trans.up, nearestPoison.transform.position - transPos,
                trans.forward);
        }

        if (distToNearestFood < distToNearestPoison)
        {
            foodAmountAtCurrentBlock = nearestFood.GetComponent<FoodStats>().foodAmountAvailable;
        }

        numberOfBibitsNear = 0;
        distToNearestBibit = float.PositiveInfinity;
        angleToNearestBibit = 0;
        angleToNearestBibit = null;


        Profiler.BeginSample("BibitStuff");
        foreach (GameObject go in BibitProducer.getAllBibits())
        {
            Vector3 goPos = go.transform.position;
            float dist = Vector3.Distance(transPos, goPos);
            if (dist < 9)
            {
                if (go != gameObject)
                {
                    numberOfBibitsNear++;
                    if (dist < distToNearestBibit * distToNearestBibit)
                    {
                        distToNearestBibit = dist;

                        nearestBibit = go;
                    }
                }
            }
        }

        Profiler.EndSample();
        if (nearestBibit != null)
        {
            angleToNearestBibit = Vector3.SignedAngle(trans.up,
                nearestBibit.transform.position - transPos, trans.forward);
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


    public int getGeneration()
    {
        return generation;
    }

    private void updateFoodAvailable()
    {
        Profiler.BeginSample("UpdateFoodAmountInvokeRepeating");
        foreach (GameObject go in FoodProducer.getAllFoods())
        {
            transformsFood.Add(go.transform.position);
        }

        Profiler.EndSample();
    }

    private void getAllFieldStats()
    {
        /*
        private List<FoodStats> foodStatsFood;    //DONE
        private List<FoodStats> foodStatsPoison;
        private List<Vector3> transformsFood;
        private List<Vector3> transformsPoison;
          */
        //todo: getAllFoodStats();
        foodAmountAvailableAllFoods = new List<double>();
        transformsFood = new List<Vector3>();
        gameObjectsFood = new List<GameObject>();
        foreach (GameObject go in FoodProducer.getAllFoods())
        {
            foodAmountAvailableAllFoods.Add(go.GetComponent<FoodStats>().foodAmountAvailable);
            transformsFood.Add(go.transform.position);
            gameObjectsFood.Add(go);
        }


        transformsPoison = new List<Vector3>();
        gameObjectsPoison = new List<GameObject>();
        //todo: getAllPoisonStats();
        foreach (GameObject go in FoodProducer.getAllPoisons())
        {
            transformsPoison.Add(go.transform.position);
            gameObjectsPoison.Add(go);
        }
    }
}