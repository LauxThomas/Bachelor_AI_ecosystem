using System;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using Random = UnityEngine.Random;

public class Bibit : MonoBehaviour
{
    public double energy = 150;
    [SerializeField] private float age = 0;
    public float ageModifier = 1;
    private int generation = 0;
    public Boolean hasModifiedAgeModifier;

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

    public WorkingNeuron outBirth = new WorkingNeuron();
    public WorkingNeuron outEat = new WorkingNeuron();
    public WorkingNeuron outRotate = new WorkingNeuron();
    public WorkingNeuron outForward = new WorkingNeuron();
    public WorkingNeuron outMemory = new WorkingNeuron();
    public WorkingNeuron outAttack = new WorkingNeuron();

    private Color color;

    private Rigidbody2D rb;
    private float rotation;
    private float speed = 5;
    private float force = 5;
    private FoodProducer foodProducer;
    private BibitProducer bibitProducer;
    private Vector3 lu;
    private Vector3 lo;
    private Vector3 ru;
    private Vector3 ro;
    private float distToNearestFood;
    private float timescaler;

    private GameObject nearestFood;

//    private float ROTATIONCOST = 0.1f;
//    private float MOVECOST = 0.1f;
    public double debugValue;
    private float distToNearestPoison;
    private GameObject nearestPoison;
    private Vector3 vecToNearestFood;
    private Vector3 vecToNearestPoison;
    private Vector3 forceToAdd;
    private double rotateForce;
    private float angleToNearestFood;
    private float? angleToNearestPoison;
    public List<Collider2D> cols;
    private double foodAmountAtCurrentBlock;
    private double foodAmountInSightRadius;
    private double distToMaxFoodBlockAround;
    private double? angleToMaxFoodBlockAround;
    private double amountOfMaxFoodBlockAround;
    private int numberOfBibitsNear;
    private double distToNearestBibit;
    private double? angleToNearestBibit;
    private GameObject nearestBibit;


    private void Start()
    {
        lu = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0));
        lo = Camera.main.ViewportToWorldPoint(new Vector3(0, 1, 0));
        ru = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, 0));
        ro = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, 0));
        rb = GetComponent<Rigidbody2D>();
        float scaler = Camera.main.orthographicSize / 3;
        transform.localScale = new Vector3(scaler, scaler, scaler);
        foodProducer = FindObjectOfType<FoodProducer>();
        bibitProducer = FindObjectOfType<BibitProducer>();
        gameObject.name = generation + ". Gen";
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

        brain.generateHiddenNeurons(10);

        brain.addOutputNeuron(outBirth);
        brain.addOutputNeuron(outRotate);
        brain.addOutputNeuron(outForward);
        brain.addOutputNeuron(outEat);
        brain.addOutputNeuron(outMemory);
        brain.addOutputNeuron(outAttack);


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
        this.generation = mother.getGeneration() + 1;


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

        r += Random.value * 0.01f - 0.005f;
        g += Random.value * 0.01f - 0.005f;
        b += Random.value * 0.01f - 0.005f;
        r = Mathf.Clamp(r, 0, 1);
        g = Mathf.Clamp(g, 0, 1);
        b = Mathf.Clamp(b, 0, 1);

        color = new Color(r, g, b);
    }

    private void Update()
    {
        timescaler = Time.deltaTime * 15;
        //Kill if necessary:
        if (energy < 100)
        {
            bibitProducer.removeBibit(gameObject);
            Destroy(gameObject);
        }
        else
        {
            color.a = (float) ((energy - MINIMUMSURVIVALENERGY) /
                               (STARTENERGY - MINIMUMSURVIVALENERGY)); //set coloralpha to healthpercentage
            GetComponent<SpriteRenderer>().color = color;

            //exponential growth:
            ageModifier = Mathf.Clamp(ageModifier, 0.001f, 200);
            ageModifier += ageModifier * Time.deltaTime / 10;
            readSensors();
            updateBrain();
            executeAction();
            flipIfNecessary();
        }
    }

    private void executeAction()
    {
        if (gameObject != null)
        {
            float costMult = 1;
            actRotate(costMult);
            actMove(costMult);
            actBirth();
            actEat(costMult, nearestFood);
            actAttack(nearestBibit);
            age += Time.deltaTime;
        }
    }

    private void actAttack(GameObject attackedBibit)
    {
        float attackCost = 1.2f;
        if (angleToNearestBibit != null &&
            Vector3.Distance(transform.position, attackedBibit.transform.position) < 0.3f &&
            Math.Abs((float) angleToNearestBibit) < 10)
        {
            Debug.Log("CHARGE!!!!!!!!!");
            double attackWish = outAttack.getValue() * Time.deltaTime*30;

            attackedBibit.GetComponent<Bibit>().energy -= attackWish;
            energy += attackWish/attackCost;
        }
    }

    private void actEat(float costMult, GameObject o)
    {
        double eatWish = outEat.getValue()*30;
        if (eatWish > 0)
        {
            nom(eatWish);
        }
    }

    private void nom(double eatWish)
    {
//        RaycastHit2D hit = Physics2D.Raycast(new Vector3(transform.position.x, transform.position.y, 10),
//            new Vector3(transform.position.x, transform.position.y, -10), 100f,
//            LayerMask.GetMask("poison", "food"));
//        Debug.DrawLine(new Vector3(transform.position.x, transform.position.y, 10),
//            new Vector3(transform.position.x, transform.position.y, -10), Color.blue);

        foreach (GameObject go in foodProducer.getAllFoods())
        {
            if (GetComponent<Renderer>().bounds.Intersects(go.GetComponent<Renderer>().bounds))
            {
                if ((go.CompareTag("poison") || go.CompareTag("food")))
                {
                    energy += foodProducer.eatFood(go, eatWish);
                }
            }
        }
    }

    private void actBirth()
    {
        double birthWish = outBirth.getValue();
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
        float remappedForward = (float) HelperFunctions.remap(outForward.getValue(), -1, 1, 0, 1); //not going backwards
        forceToAdd = remappedForward * remappedForward * speed * timescaler * transform.up.normalized;
        if (float.IsNaN(forceToAdd.x) || float.IsNaN(forceToAdd.y))
        {
            Debug.Log("forceToAdd was NaN");
            forceToAdd = new Vector3(Random.value * speed, Random.value * speed);
        }

        rb.AddForce(forceToAdd);
        if (rb.velocity.magnitude > 10)
        {
            rb.velocity = rb.velocity.normalized * 10;
        }

        energy -= forceToAdd.magnitude / speed / 10f * ageModifier;
    }

    private void actRotate(float costMult)
    {
        rotateForce = outRotate.getValue() * timescaler;
//        rotateForce = HelperFunctions.remap(rotateForce, 0.5f, 1.0f, -0.5f, 0.5f);
        if (double.IsNaN(rotateForce))
        {
            Debug.Log("rotateForce was NaN");
            rotateForce = Random.Range(-1f, 1f);
        }

        debugValue = rotateForce;

        transform.Rotate(0, 0, (float) rotateForce * force, Space.Self);

        energy -= Mathf.Abs((float) (rotateForce * ageModifier));
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
    }

    private void readSensors()
    {
        //TODO: 
        // need to calculate:

        // distToNearestPoison
        // angleToNearestPoison

        // foodAmountATCurrentBlock

        // foodAmountInSightRadius

        // amountOfMaxFoodBlockAround
        // distToMaxFoodBlockAround
        // angleToMaxFoodBlockAround

        // numberOfBibitsNear
        // distToNearestBibit
        // angleToNearestBibit

        distToNearestPoison = float.PositiveInfinity;
        angleToNearestPoison = null;
        foodAmountAtCurrentBlock = 0;
        foodAmountInSightRadius = 0;
        distToMaxFoodBlockAround = float.PositiveInfinity;
        angleToMaxFoodBlockAround = null;
        amountOfMaxFoodBlockAround = 0;


        foreach (GameObject go in foodProducer.getAllFoods())
        {
            if (Vector3.Distance(transform.position, go.transform.position) < 3)
            {
                if (Vector3.Distance(transform.position, go.transform.position) < distToNearestPoison)
                {
                    // distToNearestPoison
                    distToNearestPoison = Vector3.Distance(transform.position, go.transform.position);
                    // angleToNearestPoison
                    angleToNearestPoison = Vector3.SignedAngle(transform.up, go.transform.position - transform.position,
                        transform.forward);
                }

                // foodAmountATCurrentBlock
                if (GetComponent<Renderer>().bounds.Intersects(go.GetComponent<Renderer>().bounds))
                {
                    if (go.CompareTag("poison"))
                    {
                        foodAmountAtCurrentBlock = -100;
                        energy += foodProducer.eatFood(go, 100);
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
                    distToMaxFoodBlockAround = Vector3.Distance(transform.position, go.transform.position);
                    angleToMaxFoodBlockAround = Vector3.SignedAngle(transform.up,
                        go.transform.position - transform.position, transform.forward);
                }
            }
        }

        numberOfBibitsNear = 0;
        distToNearestBibit = float.PositiveInfinity;
        angleToNearestBibit = null;

        foreach (GameObject go in bibitProducer.getAllBibits())
        {
            if (go != gameObject)
            {
                if (Vector3.Distance(transform.position, go.transform.position) < 3)
                {
                    numberOfBibitsNear++;
                    if (Vector3.Distance(transform.position, go.transform.position) < distToNearestBibit)
                    {
                        distToNearestBibit = Vector3.Distance(transform.position, go.transform.position);
                        angleToNearestBibit = Vector3.SignedAngle(transform.up,
                            go.transform.position - transform.position, transform.forward);
                        nearestBibit = go;
                    }
                }
            }
        }

        #region backupOld

//        List<GameObject> allFoods = foodProducer.getAllFoods();
//        distToNearestFood = float.PositiveInfinity;
//        distToNearestPoison = float.PositiveInfinity;
//        for (int i = 0; i < allFoods.Count; i++)
//        {
//            float dist = Vector3.Distance(transform.position, allFoods[i].transform.position);
//            if (allFoods[i].GetComponent<FoodStats>().isFertile)
//            {
//                if (dist < distToNearestFood)
//                {
//                    if (allFoods[i].CompareTag("food"))
//                    {
//                        distToNearestFood = dist;
//                        nearestFood = allFoods[i];
//                    }
//                }
//
//                if (dist < distToNearestPoison)
//                {
//                    if (allFoods[i].CompareTag("poison"))
//                    {
//                        distToNearestPoison = dist;
//                        nearestPoison = allFoods[i];
//                    }
//                }
//            }
//        }
//
//
//        if (distToNearestFood < float.PositiveInfinity && nearestFood != null)
//        {
//            vecToNearestFood = nearestFood.transform.position - transform.position;
//        }
//
//        if (distToNearestPoison < float.PositiveInfinity && nearestPoison != null)
//        {
//            vecToNearestPoison = nearestPoison.transform.position - transform.position;
//        }
//
//        //calculate angle to nearest Food / poison
//        angleToNearestFood = Vector3.SignedAngle(transform.up, vecToNearestFood, transform.forward);
//        angleToNearestPoison = Vector3.SignedAngle(transform.up, vecToNearestPoison, transform.forward);

        #endregion

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


//    private void eat(GameObject collisionGameObject, Boolean wasFood)
//    {
//        
//        foodProducer.removeFood(collisionGameObject);
//        Destroy(collisionGameObject);
//        energy += wasFood ? 40 : -50;
//    }

    public int getGeneration()
    {
        return generation;
    }
}