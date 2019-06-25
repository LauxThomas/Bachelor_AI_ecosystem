using System;
using System.Diagnostics.CodeAnalysis;
using DefaultNamespace;
using UnityEngine;
using Random = UnityEngine.Random;

[SuppressMessage("ReSharper", "IdentifierTypo")]
[SuppressMessage("ReSharper", "CommentTypo")]
public class Bibit : MonoBehaviour
{
    public double energy = 150;
    [SerializeField] private float age;
    public float ageModifier = 1;
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
    private float timescaler;

    private GameObject nearestFood;

    public double debugValue;
    private float distToNearestPoison;
    private GameObject nearestPoison;
    private Vector3 vecToNearestFood;
    private Vector3 vecToNearestPoison;
    private Vector3 forceToAdd;
    private double rotateForce;
    private float angleToNearestFood;
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


    private void Start()
    {
        lu = FoodProducer.lu;
        lo = FoodProducer.lo;
        ru = FoodProducer.ru;
//        float scaler = BibitProducer.CameraSize / 3;
//        transform.localScale = new Vector3(scaler, scaler, scaler);
        foodProducer = FindObjectOfType<FoodProducer>();
        bibitProducer = FindObjectOfType<BibitProducer>();
        rb = GetComponent<Rigidbody2D>();
        gameObject.name = displayName + ", " + generation + ". seines Namens";
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
        brain = new NeuralNetwork(); //DEBUGGING
        brain = mother.brain.cloneFullMesh();
        energy = 150;
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
        timescaler = Time.deltaTime * 15;
        energy -= Time.deltaTime * ageModifier;
        //Kill if necessary:
        if (energy < 100)
        {
            bibitProducer.removeBibit(gameObject);
            Destroy(gameObject);
        }
        else
        {
            flipIfNecessary();
            color.a = (float) ((energy - MINIMUMSURVIVALENERGY) /
                               (STARTENERGY - MINIMUMSURVIVALENERGY)); //set coloralpha to healthpercentage
            GetComponent<SpriteRenderer>().color = color;

            //exponential growth:
            ageModifier = Mathf.Clamp(ageModifier, 0.001f, 200);
            ageModifier += ageModifier * Time.deltaTime / 10;
            readSensors();
            updateBrain();
            executeAction();
        }
    }

    private void executeAction()
    {
        if (gameObject != null)
        {
            float rotationCost, moveCost, birthCost, eatCost, attackCost;
            rotationCost = 1.2f;
            moveCost = 1.1f;
            birthCost = 0.8f;
            eatCost = 1.0f;
            attackCost = 0.8f;
            actRotate(rotationCost);
            actMove(moveCost);
            actBirth(birthCost);
            actEat(eatCost);
            if (nearestBibit != null)
            {
                actAttack(nearestBibit, attackCost);
            }

            age += Time.deltaTime;
        }
    }

    private void actAttack(GameObject attackedBibit, float cost)
    {
        geneticDifferenceToAttackedBibit = 0;
        if (angleToNearestBibit != null &&
            Vector3.Distance(transform.position, attackedBibit.transform.position) < 0.3f &&
            Math.Abs((float) angleToNearestBibit) < 70)
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
                    energy += attackWish * cost;
                }
            }
        }
    }

    private void actEat(float eatCost)
    {
        double eatWish = outEat.getValue() * 30;
        if (eatWish > 0)
        {
            nom(eatWish, eatCost);
        }
    }

    private void nom(double eatWish, float eatCost)
    {
        foreach (GameObject go in foodProducer.getAllFoods())
        {
            if (GetComponent<Renderer>().bounds.Intersects(go.GetComponent<Renderer>().bounds))
            {
                if ((go.CompareTag("poison") || go.CompareTag("food")))
                {
                    energy += FoodProducer.eatFood(go, eatWish) * eatCost;
                }
            }
        }
    }

    private void actBirth(float birthCost)
    {
        double birthWish = outBirth.getValue();
        debugValue = birthWish;
//        if (birthWish > 0)
//        {
        if (energy > STARTENERGY + MINIMUMSURVIVALENERGY * birthCost * 1.5f)
        {
            bibitProducer.spawnChild(gameObject);
            energy -= STARTENERGY * birthCost;
        }

//        }
    }

    private void actMove(float moveCost)
    {
        float remappedForward = (float) HelperFunctions.remap(outForward.getValue(), -1, 1, 0, 1); //not going backwards
//        float remappedForward = (float)outForward.getValue();
        forceToAdd = SPEED * remappedForward * remappedForward * timescaler * transform.up.normalized;
        if (rb.velocity.magnitude > 10)
        {
            Debug.Log("clamped");
            rb.velocity = rb.velocity.normalized * 10;
        }

//        rb.AddForce(forceToAdd*Vector3.up.normalized);
        rb.AddForce(forceToAdd);

        energy -= forceToAdd.magnitude / SPEED / 10f * ageModifier * moveCost;
    }

    private void actRotate(float rotationCost)
    {
        rotateForce = outRotate.getValue() * timescaler;
        if (double.IsNaN(rotateForce))
        {
            Debug.Log("rotateForce was NaN");
            rotateForce = Random.Range(-1f, 1f);
        }

        transform.Rotate(0, 0, (float) rotateForce * FORCE, Space.Self);
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
        distToNearestPoison = float.PositiveInfinity;
        angleToNearestPoison = null;
        foodAmountAtCurrentBlock = 0;
        foodAmountInSightRadius = 0;
        distToMaxFoodBlockAround = float.PositiveInfinity;
        angleToMaxFoodBlockAround = null;
        amountOfMaxFoodBlockAround = 0;


        Transform trans = transform;
        Vector3 transPos = trans.position;
        foreach (GameObject go in foodProducer.getAllFoods())
        {
            Vector3 goPos = go.transform.position;
            if (Vector3.Distance(transPos, goPos) < 3)
            {
                if (Vector3.Distance(transPos, goPos) < distToNearestPoison)
                {
                    // distToNearestPoison
                    distToNearestPoison = Vector3.Distance(trans.position, goPos);
                    // angleToNearestPoison
                    angleToNearestPoison = Vector3.SignedAngle(trans.up, goPos - transPos,
                        trans.forward);
                }

                // foodAmountATCurrentBlock
                if (GetComponent<Renderer>().bounds.Intersects(go.GetComponent<Renderer>().bounds))
                {
                    if (go.CompareTag("poison"))
                    {
                        foodAmountAtCurrentBlock = -100;
                        energy += FoodProducer.eatFood(go, 100);
                    }
                    else
                    {
                        foodAmountAtCurrentBlock = go.GetComponent<FoodStats>().foodAmountAvailable;
                    }
                }

                // amountOfMaxFoodBlockAround
                // distToMaxFoodBlockAround
                // angleToMaxFoodBlockAround
                if (go.GetComponent<FoodStats>().foodAmountAvailable > amountOfMaxFoodBlockAround &&
                    go.CompareTag("food"))
                {
                    amountOfMaxFoodBlockAround = go.GetComponent<FoodStats>().foodAmountAvailable;
                    distToMaxFoodBlockAround = Vector3.Distance(transPos, goPos);
                    angleToMaxFoodBlockAround = Vector3.SignedAngle(trans.up,
                        goPos - transPos, trans.forward);
                }
            }
        }

        numberOfBibitsNear = 0;
        distToNearestBibit = float.PositiveInfinity;
        angleToNearestBibit = 0;
        angleToNearestBibit = null;

        foreach (GameObject go in bibitProducer.getAllBibits())
        {
            if (go != gameObject)
            {
                Vector3 goPos = go.transform.position;
                if (Vector3.Distance(transPos, goPos) < 3)
                {
                    numberOfBibitsNear++;
                    if (Vector3.Distance(transPos, goPos) < distToNearestBibit)
                    {
                        distToNearestBibit = Vector3.Distance(transPos, goPos);
                        angleToNearestBibit = Vector3.SignedAngle(trans.up,
                            goPos - transPos, trans.forward);
                        nearestBibit = go;
                    }
                }
            }
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
}