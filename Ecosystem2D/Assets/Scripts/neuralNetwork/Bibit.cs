using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using DefaultNamespace;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

[SuppressMessage("ReSharper", "IdentifierTypo")]
[SuppressMessage("ReSharper", "CommentTypo")]
public class Bibit : MonoBehaviour
{
    public double energy = 150;
    [SerializeField] public float age;
    public float ageModifier = 0.001f;
    public int generation;
    public String displayName;

    public NeuralNetwork brain;

    public const float STARTENERGY = 150f;
    public const float MINIMUMSURVIVALENERGY = 100f;

    private const String NAME_IN_BIAS = "bias";
    private const String NAME_IN_ENERGY = "Current Energy";
    private const String NAME_IN_AGE = "Current Age";
    private const String NAME_IN_MEMORY = "My in Memory";
    private const String NAME_IN_MEMORY2 = "My in Memory2";
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
    private const String NAME_OUT_MEMORY2 = "My out Memory2";
    private const String NAME_OUT_ATTACK = "Attack";

    public InputNeuron inBias = new InputNeuron();
    public InputNeuron inEnergy = new InputNeuron();
    public InputNeuron inAge = new InputNeuron();
    public InputNeuron inMemory = new InputNeuron();
    public InputNeuron inMemory2 = new InputNeuron();
    public InputNeuron inDistToNearestPoison = new InputNeuron();
    public InputNeuron inAngleToNearestPoison = new InputNeuron();
    public InputNeuron inFoodAmountAtCurrentBlock = new InputNeuron();
    public InputNeuron inFoodAmountInSightRadius = new InputNeuron();
    public InputNeuron inDistToMaxFoodBlockAround = new InputNeuron();
    public InputNeuron inAngleToMaxFoodBlockAround = new InputNeuron();
    public InputNeuron inFoodAmountOfMaxFoodBlockAround = new InputNeuron();
    public InputNeuron inNumberOfBibitsNear = new InputNeuron();
    public InputNeuron inDistToNearestBibit = new InputNeuron();
    public InputNeuron inAngleToNearestBibit = new InputNeuron();
    private InputNeuron inGeneticDifferenceToNearestBibit = new InputNeuron();
    public InputNeuron inCenterPosition = new InputNeuron();

    public WorkingNeuron outBirth = new WorkingNeuron();
    public WorkingNeuron outEat = new WorkingNeuron();
    public WorkingNeuron outRotate = new WorkingNeuron();
    public WorkingNeuron outForward = new WorkingNeuron();
    public WorkingNeuron outMemory = new WorkingNeuron();
    public WorkingNeuron outMemory2 = new WorkingNeuron();
    public WorkingNeuron outAttack = new WorkingNeuron();

    public Color color;

    private float rotation;
    public const float SPEED = 5;
    public const float FORCE = 5;
    private FoodProducer foodProducer;
    private BibitProducer bibitProducer;
    public Vector3 lu;
    public Vector3 lo;
    public Vector3 ru;
    public float distToNearestFood;

    public GameObject nearestFood;
    public GameObject nearestPoison;

    public double debugValue;
    public float distToNearestPoison;
    private Vector3 vecToNearestFood;
    private Vector3 vecToNearestPoison;
    private Vector3 forceToAdd;
    private double rotateForce;
    public float? angleToNearestFood;
    public float? angleToNearestPoison;
    public double foodAmountAtCurrentBlock;
    public double foodAmountInSightRadius;
    public double distToMaxFoodBlockAround;
    public double? angleToMaxFoodBlockAround;
    public double amountOfMaxFoodBlockAround;
    public int numberOfBibitsNear;
    public double distToNearestBibit;
    public double? angleToNearestBibit;
    public GameObject nearestBibit;
    private float geneticDifferenceToAttackedBibit;
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private int layerMask;
    public float rotationCost;
    public float moveCost;
    public float birthCost;
    public float eatCost, attackCost;
    private List<double> foodAmountAvailableAllFoods;
    private List<FoodStats> foodStatsPoison;
    private List<Vector3> transformsFood;
    private List<Vector3> transformsPoison;
    private List<GameObject> gameObjectsFood;
    private List<GameObject> gameObjectsPoison;
    private JobHandle jobHandle;
    public bool isDead;
    public bool isOnPoison;
    public int poisonModifier = 2;


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
        attackCost = 1.05f;


//        updateFoodAvailable();
//        InvokeRepeating("updateFoodAvailable", 1, Random.Range(0.1f, 1f));
    }


    public void pseudoConstructor1()
    {
        inBias.setName(NAME_IN_BIAS);
        inEnergy.setName(NAME_IN_ENERGY);
        inAge.setName(NAME_IN_AGE);
        inMemory.setName(NAME_IN_MEMORY);
        inMemory2.setName(NAME_IN_MEMORY2);
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
        outMemory2.setName(NAME_OUT_MEMORY2);
        outAttack.setName(NAME_OUT_ATTACK);

        brain = new NeuralNetwork();


        brain.addInputNeuron(inBias);
        brain.addInputNeuron(inEnergy);
        brain.addInputNeuron(inAge);
        brain.addInputNeuron(inMemory);
        brain.addInputNeuron(inMemory2);
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

        brain.generateHiddenNeurons(25);

        brain.addOutputNeuron(outBirth);
        brain.addOutputNeuron(outRotate);
        brain.addOutputNeuron(outForward);
        brain.addOutputNeuron(outEat);
        brain.addOutputNeuron(outMemory);
        brain.addOutputNeuron(outMemory2);
        brain.addOutputNeuron(outAttack);


        brain.generateFullMesh();
        brain.randomizeAllWeights();
        color = new Color(Random.value, Random.value, Random.value);
    }

    private void writeStuff(Bibit mother, NeuralNetwork myBrain)
    {
        StreamWriter writer = new StreamWriter("Assets/Resources/test.txt", true);
        String printString = "";


        printString += myBrain.outputNeurons[0].connections[0].weight + "";
        Debug.Log(printString);
        writer.WriteLine(printString);
        writer.Close();
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
        BibitProducer.updateGeneration(generation);
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
        r = math.clamp(r, 0, 1);
        g = math.clamp(g, 0, 1);
        b = math.clamp(b, 0, 1);

        color = new Color(r, g, b);
//        writeStuff(mother, brain);
    }

    private void Update()
    {
        if (isDead)
        {
            Destroy(gameObject);
        }
    }


    public int getGeneration()
    {
        return generation;
    }

    private void updateFoodAvailable()
    {
//        Profiler.BeginSample("updateFoodAvailable");
        transformsFood.Clear();
        foreach (GameObject go in FoodProducer.getAllFoods())
        {
            transformsFood.Add(go.transform.position);
        }

//        Profiler.EndSample();
    }

    private void getAllFieldStats()
    {
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
        foreach (GameObject go in FoodProducer.getAllPoisons())
        {
            transformsPoison.Add(go.transform.position);
            gameObjectsPoison.Add(go);
        }
    }
}

//TODO: Change to JobComponentSystem and add BurstCompile
public class BibitFlippingSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((Bibit bibit, Transform transform) =>
        {
            Vector3 pos = transform.position;
            if (pos.x > bibit.ru.x)
            {
                pos.x = bibit.lu.x;
            }
            else if (pos.x < bibit.lu.x)
            {
                pos.x = bibit.ru.x;
            }

            else if (pos.y > bibit.lo.y)
            {
                pos.y = bibit.lu.y;
            }
            else if (pos.y < bibit.lu.y)
            {
                pos.y = bibit.lo.y;
            }

            if (pos != transform.position)
            {
                transform.position = pos;
            }
        });
    }
}

public class BibitMovementSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        float dt = Time.deltaTime;
        Entities.ForEach((Bibit bibit, Transform transform, Rigidbody2D rb) =>
        {
            //move:
            float speedForce = (float) bibit.outForward.getValue();
            Vector3 forceToAdd = Bibit.SPEED * speedForce * 15 * dt * transform.up.normalized;
            float energyMagnitude = forceToAdd.magnitude;
            if (speedForce < 0)
            {
                forceToAdd *= 0.75f;
            }

            rb.AddForce(forceToAdd);

            //rotate:
            float rotateForce = (float) (bibit.outRotate.getValue() * 15 * dt);

            rb.SetRotation(rb.rotation + (float) rotateForce * Bibit.FORCE);

            if (bibit.isOnPoison)
            {
                bibit.energy -= energyMagnitude / Bibit.SPEED / 10f * bibit.ageModifier * bibit.moveCost *
                                bibit.poisonModifier;
                bibit.energy -=
                    math.abs((float) (rotateForce * bibit.ageModifier * bibit.rotationCost * bibit.poisonModifier));
                bibit.energy -= dt * 15;
            }
            else
            {
                bibit.energy -= forceToAdd.magnitude / Bibit.SPEED / 10f * bibit.ageModifier * bibit.moveCost;
                bibit.energy -= math.abs((float) (rotateForce * bibit.ageModifier * bibit.rotationCost));
            }
        });
    }
}

public class BibitReproductionSystem : ComponentSystem
{
    public List<GameObject> bibitsToSpawn;

    protected override void OnStartRunning()
    {
        bibitsToSpawn = new List<GameObject>();
    }

    protected override void OnUpdate()
    {
        Entities.ForEach((Bibit bibit) =>
        {
            if (!bibit.isDead)
            {
                //energyupdate and generationupdate
                bibit.energy -= Time.deltaTime * bibit.ageModifier;
                BibitProducer.updateMaxGeneration(bibit.generation, bibit);

                //Kill if energy<100
                if (bibit.energy < 100)
                {
                    BibitProducer.removeBibit(bibit.gameObject);
                    bibit.isDead = true;
                }

                else
                {
                    //birth:
                    float birthWish = (float) bibit.outBirth.getValue();

                    if (birthWish > 0)
                    {
                        if (bibit.energy > Bibit.STARTENERGY + Bibit.MINIMUMSURVIVALENERGY * bibit.birthCost * 1.15f)
                        {
                            bibitsToSpawn.Add(bibit.gameObject);

                            if (bibit.isOnPoison)
                            {
                                bibit.energy -= Bibit.STARTENERGY * bibit.birthCost;
                            }
                            else
                            {
                                bibit.energy -= Bibit.STARTENERGY * bibit.birthCost * bibit.poisonModifier;
                            }
                        }
                    }
                }
            }


            bibit.ageModifier = (float) (-(3 / math.pow(math.E, 0.1 * bibit.age - 10) + 1) + 3);
            bibit.ageModifier = math.clamp(bibit.ageModifier, 0.001f, 200);
            bibit.age += Time.deltaTime;
        });
        //spawnChildren:
        foreach (GameObject mother in bibitsToSpawn)
        {
            BibitProducer.spawnChild(mother);
        }

        bibitsToSpawn.Clear();
    }
}

public class BibitAttackingSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((Bibit bibit, Transform transform) =>
        {
            if (bibit.nearestBibit)
            {
                float geneticDifferenceToAttackedBibit = 0;
                if (bibit.angleToNearestBibit != null &&
                    (Vector2.Distance(transform.position, bibit.nearestBibit.transform.position) < 0.3f * 0.3f &&
                     math.abs((float) bibit.angleToNearestBibit) < 70))
                {
                    geneticDifferenceToAttackedBibit +=
                        math.abs(bibit.color.r - bibit.nearestBibit.GetComponent<SpriteRenderer>().color.r);
                    geneticDifferenceToAttackedBibit +=
                        math.abs(bibit.color.g - bibit.nearestBibit.GetComponent<SpriteRenderer>().color.g);
                    geneticDifferenceToAttackedBibit +=
                        math.abs(bibit.color.b - bibit.nearestBibit.GetComponent<SpriteRenderer>().color.b);
                    if (geneticDifferenceToAttackedBibit > 0.2f)
                    {
                        {
                            double attackWish = bibit.outAttack.getValue() * Time.deltaTime * 30;

                            bibit.nearestBibit.GetComponent<Bibit>().energy -= attackWish;
                            if (bibit.isOnPoison)
                            {
                                bibit.energy += attackWish / (bibit.attackCost * bibit.poisonModifier);
                            }
                            else
                            {
                                bibit.energy += attackWish / bibit.attackCost;
                            }

                            Debug.Log("ATTACKED!");
                        }
                    }
                }
            }
        });
    }
}

[UpdateBefore(typeof(BibitFieldMeasurementSystem))]
public class BibitNeuralNetworkSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((Bibit bibit) =>
        {
            Profiler.BeginSample("Update Brain");
            bibit.brain.invalidate();
            bibit.inBias.setValue(1);
            bibit.inEnergy.setValue(bibit.energy);
            bibit.inAge.setValue(bibit.age);
            bibit.inMemory.setValue(bibit.outMemory.getValue());
            bibit.inMemory2.setValue(bibit.outMemory2.getValue());
            bibit.inDistToNearestPoison.setValue(bibit.distToNearestPoison);
            if (bibit.angleToNearestPoison != null)
                bibit.inAngleToNearestPoison.setValue((float) bibit.angleToNearestPoison);
            bibit.inFoodAmountAtCurrentBlock.setValue(bibit.foodAmountAtCurrentBlock);
            bibit.inFoodAmountInSightRadius.setValue((double) bibit.foodAmountInSightRadius);
            bibit.inDistToMaxFoodBlockAround.setValue(bibit.distToMaxFoodBlockAround);
            if (bibit.angleToMaxFoodBlockAround != null)
                bibit.inAngleToMaxFoodBlockAround.setValue((float) bibit.angleToMaxFoodBlockAround);
            bibit.inFoodAmountOfMaxFoodBlockAround.setValue(bibit.amountOfMaxFoodBlockAround);
            bibit.inNumberOfBibitsNear.setValue(bibit.numberOfBibitsNear);
            bibit.inDistToNearestBibit.setValue(bibit.distToNearestBibit);
            if (bibit.angleToNearestBibit != null)
                bibit.inAngleToNearestBibit.setValue((double) bibit.angleToNearestBibit);
            Transform trans = bibit.transform;
            bibit.inCenterPosition.setValue(Vector3.SignedAngle(trans.up, Vector3.zero - trans.position,
                trans.forward));
            Profiler.EndSample();
        });
    }
}

public class BibitFieldMeasurementSystem : ComponentSystem
{
    private FoodStats[,] fields;
    private List<FoodStats> neighbours;
    private int offsetX;
    private int offsetY;
    private int widthBounds;
    private int heightBounds;

    protected override void OnStartRunning()
    {
        fields = FoodProducer.fieldsArray;
        widthBounds = fields.GetUpperBound(0);
        heightBounds = fields.GetUpperBound(1);
        neighbours = new List<FoodStats>();
        offsetX = math.abs(Mathf.RoundToInt(fields[0, 0].transform.position.x));
        offsetY = math.abs(Mathf.RoundToInt(fields[0, 0].transform.position.y));
    }

    protected override void OnUpdate()
    {
        Entities.ForEach((Bibit bibit, Transform transform) =>
        {
            Profiler.BeginSample("foods vergleichen");

            //resetToDefaults:
            bibit.distToNearestPoison = float.PositiveInfinity;
            bibit.distToNearestFood = float.PositiveInfinity;
            bibit.distToNearestBibit = float.PositiveInfinity;
            bibit.angleToNearestPoison = null;
            bibit.angleToNearestFood = null;
            bibit.angleToNearestBibit = null;
            bibit.nearestFood = null;
            bibit.nearestPoison = null;
            bibit.foodAmountInSightRadius = 0;
            bibit.nearestBibit = null;
            neighbours.Clear();

            Vector3 transPos = transform.position;
            int transPosX = Mathf.RoundToInt(transPos.x + offsetX) + 1;
            int transPosY = Mathf.RoundToInt(offsetY - transPos.y) + 1;


            //todo: mehrere Iterationen einbauen, for loop, sichtradius vererben

            if (transPosX <= widthBounds && transPosY <= heightBounds &&
                transPosX >= 0 && transPosY > 0)
            {
                FoodStats currentField = fields[transPosX, transPosY];
                neighbours.Add(currentField);
            }

            if (transPosX <= widthBounds && transPosY + 1 <= heightBounds &&
                transPosX >= 0 && transPosY + 1 > 0)
            {
                FoodStats aboveField = fields[transPosX, transPosY + 1];
                neighbours.Add(aboveField);
            }

            if (transPosX <= widthBounds && transPosY - 1 <= heightBounds &&
                transPosX >= 0 && transPosY - 1 > 0)
            {
                FoodStats belowField = fields[transPosX, transPosY - 1];
                neighbours.Add(belowField);
            }

            if (transPosX - 1 <= widthBounds && transPosY <= heightBounds &&
                transPosX - 1 >= 0 && transPosY > 0)
            {
                FoodStats leftField = fields[transPosX - 1, transPosY];
                neighbours.Add(leftField);
            }

            if (transPosX + 1 <= widthBounds && transPosY <= heightBounds &&
                transPosX + 1 >= 0 && transPosY > 0)
            {
                FoodStats rightField = fields[transPosX + 1, transPosY];
                neighbours.Add(rightField);
            }


            if (transPosX + 1 <= widthBounds && transPosY + 1 <= heightBounds &&
                transPosX + 1 >= 0 && transPosY + 1 > 0)
            {
                FoodStats rightAboveField = fields[transPosX + 1, transPosY + 1];
                neighbours.Add(rightAboveField);
            }

            if (transPosX + 1 <= widthBounds && transPosY - 1 <= heightBounds &&
                transPosX + 1 >= 0 && transPosY - 1 > 0)
            {
                FoodStats rightBelowField = fields[transPosX + 1, transPosY - 1];
                neighbours.Add(rightBelowField);
            }

            if (transPosX - 1 <= widthBounds && transPosY + 1 <= heightBounds &&
                transPosX - 1 >= 0 && transPosY + 1 > 0)
            {
                FoodStats leftAboveField = fields[transPosX - 1, transPosY + 1];
                neighbours.Add(leftAboveField);
            }

            if (transPosX - 1 <= widthBounds && transPosY - 1 <= heightBounds &&
                transPosX - 1 >= 0 && transPosY - 1 > 0)
            {
                FoodStats leftBelowField = fields[transPosX - 1, transPosY - 1];
                neighbours.Add(leftBelowField);
            }


            foreach (FoodStats f in neighbours)
            {
                if (f.CompareTag("poison"))
                {
                    Vector3 neighbourPos = f.transform.position;
                    float newDist = Vector2.Distance(transPos, neighbourPos);
                    if (newDist < bibit.distToNearestPoison)
                    {
                        bibit.distToNearestPoison = newDist;
                        bibit.angleToNearestPoison =
                            Vector3.SignedAngle(transform.up, neighbourPos - transPos, transform.forward);
                        bibit.nearestPoison = f.gameObject;
                    }
                }
            }

            foreach (FoodStats f in neighbours)
            {
                if (f.CompareTag("food"))
                {
                    Vector3 neighbourPos = f.transform.position;
                    bibit.foodAmountInSightRadius += f.foodAmountAvailable;
                    float newDist = Vector2.Distance(transPos, neighbourPos);
                    if (newDist < bibit.distToNearestFood)
                    {
                        bibit.nearestFood = f.gameObject;
                        bibit.distToNearestFood = newDist;
                        bibit.angleToNearestFood =
                            Vector3.SignedAngle(transform.up, neighbourPos - transPos, transform.forward);
                    }
                }
            }

            Profiler.EndSample();


           bibit.isOnPoison = bibit.distToNearestPoison < bibit.distToNearestFood;
        });
    }
}

[UpdateAfter(typeof(BibitFieldMeasurementSystem))]
public class BibitColoringSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((Bibit bibit, SpriteRenderer sr) =>
        {
            bibit.color.a = (float) ((bibit.energy - Bibit.MINIMUMSURVIVALENERGY) /
                                     (Bibit.STARTENERGY - Bibit.MINIMUMSURVIVALENERGY)
                ); //set coloralpha to healthpercentage
            bibit.GetComponent<SpriteRenderer>().color = bibit.color;
        });
    }
}

[UpdateAfter(typeof(BibitFieldMeasurementSystem))]
public class BibitEatingSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((Bibit bibit, Transform transform) =>
        {
            float eatWish = (float) (bibit.outEat.getValue() * 30);
            if (eatWish <= 0) return;
            if (bibit.nearestFood != null|| bibit.nearestPoison!= null)
            {
                if (bibit.isOnPoison)
                {
                    bibit.energy += FoodProducer.eatPoison(eatWish);
                }
                else
                {
                    bibit.energy += FoodProducer.eatFood(bibit.nearestFood, eatWish) / bibit.eatCost;
                }
            }
        });
    }
}