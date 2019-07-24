using System;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

namespace DefaultNamespace
{
    //TODO aufräumen ...
    public class Creature : MonoBehaviour
    {
        private const float CONST_EAT = 0.1f;
        private const float GAIN_EAT = 1f;
        private const float COST_PERMANENT = 0.01f;
        private const float COST_WALK = 0.05f;
        private const float COST_ROTATE = 0.05f;
        private const float AGEPERTICK = 0.01f;

        private const float MOVESPEED = 10f;

        private const float STARTENERGY = 150f;
        private const float MINIMUMSURVIVALENERGY = 100f;

        private Vector2 pos;
        private float viewAngle;

        private float feelerAngle;
        private Vector2 feelerPos;

        private float energy = 150;
        private float age = 0;

        private NeuralNetwork brain;

        private const String NAME_IN_BIAS = "bias";
        private const String NAME_IN_FOODVALUEPOSITION = "Food Value Position";
        private const String NAME_IN_FOODVALUEFEELER = "Food Value Feeler";
        private const String NAME_IN_OCCLUSIONFEELER = "Occlusion Feeler";
        private const String NAME_IN_ENERGY = "Energy";
        private const String NAME_IN_AGE = "Age";
        private const String NAME_IN_GENETICDIFFERENCE = "Genetic Difference";
        private const String NAME_IN_WASATTACKED = "Was Attacked";
        private const String NAME_IN_WATERONFEELER = "Water On Feeler";
        private const String NAME_IN_WATERONCREATURE = "Water On Creature";

        private const String NAME_OUT_BIRTH = "Birth";
        private const String NAME_OUT_ROTATE = "Rotate";
        private const String NAME_OUT_FORWARD = "Forward";
        private const String NAME_OUT_FEELERANGLE = "Feeler Angle";
        private const String NAME_OUT_ATTACK = "Attack";
        private const String NAME_OUT_EAT = "Eat";

        private InputNeuron inBias = new InputNeuron();
        private InputNeuron inFoodValuePosition = new InputNeuron();
        private InputNeuron inFoodValueFeeler = new InputNeuron();
        private InputNeuron inOcclusionFeeler = new InputNeuron();
        private InputNeuron inEnergy = new InputNeuron();
        private InputNeuron inAge = new InputNeuron();
        private InputNeuron inGeneticDifference = new InputNeuron();
        private InputNeuron inWasAttacked = new InputNeuron();
        private InputNeuron inWaterOnFeeler = new InputNeuron();
        private InputNeuron inWaterOnCreature = new InputNeuron();

        private WorkingNeuron outBirth = new WorkingNeuron();
        private WorkingNeuron outRotate = new WorkingNeuron();
        private WorkingNeuron outForward = new WorkingNeuron();
        private WorkingNeuron outFeelerangle = new WorkingNeuron();
        private WorkingNeuron outAttack = new WorkingNeuron();
        private WorkingNeuron outEat = new WorkingNeuron();

        private Color color;

        public Creature(Vector2 pos, float viewAngle)
        {
            this.pos = pos;
            this.viewAngle = viewAngle;

            inBias.setName(NAME_IN_BIAS);
            inFoodValuePosition.setName(NAME_IN_FOODVALUEPOSITION);
            inFoodValueFeeler.setName(NAME_IN_FOODVALUEFEELER);
            inOcclusionFeeler.setName(NAME_IN_OCCLUSIONFEELER);
            inEnergy.setName(NAME_IN_ENERGY);
            inAge.setName(NAME_IN_AGE);
            inGeneticDifference.setName(NAME_IN_GENETICDIFFERENCE);
            inWasAttacked.setName(NAME_IN_WASATTACKED);
            inWaterOnFeeler.setName(NAME_IN_WATERONFEELER);
            inWaterOnCreature.setName(NAME_IN_WATERONCREATURE);

            outBirth.setName(NAME_OUT_BIRTH);
            outRotate.setName(NAME_OUT_ROTATE);
            outForward.setName(NAME_OUT_FORWARD);
            outFeelerangle.setName(NAME_OUT_FEELERANGLE);
            outAttack.setName(NAME_OUT_ATTACK);
            outEat.setName(NAME_OUT_EAT);

            brain = new NeuralNetwork();
            brain.addInputNeuron(inBias);
            brain.addInputNeuron(inFoodValuePosition);
            brain.addInputNeuron(inFoodValueFeeler);
            brain.addInputNeuron(inOcclusionFeeler);
            brain.addInputNeuron(inEnergy);
            brain.addInputNeuron(inAge);
            brain.addInputNeuron(inGeneticDifference);
            brain.addInputNeuron(inWasAttacked);
            brain.addInputNeuron(inWaterOnFeeler);
            brain.addInputNeuron(inWaterOnCreature);

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

        public Creature(Creature mother)
        {
            this.pos = mother.pos;
            this.viewAngle = (float) Random.Range(0, 360) * Mathf.PI * 2;
            this.brain = mother.brain.cloneFullMesh();

            inBias = brain.getInputNeuronFromName(NAME_IN_BIAS);
            inFoodValuePosition = brain.getInputNeuronFromName(NAME_IN_FOODVALUEPOSITION);
            inFoodValueFeeler = brain.getInputNeuronFromName(NAME_IN_FOODVALUEFEELER);
            inOcclusionFeeler = brain.getInputNeuronFromName(NAME_IN_OCCLUSIONFEELER);
            inEnergy = brain.getInputNeuronFromName(NAME_IN_ENERGY);
            inAge = brain.getInputNeuronFromName(NAME_IN_AGE);
            inGeneticDifference = brain.getInputNeuronFromName(NAME_IN_GENETICDIFFERENCE);
            inWasAttacked = brain.getInputNeuronFromName(NAME_IN_WASATTACKED);
            inWaterOnFeeler = brain.getInputNeuronFromName(NAME_IN_WATERONFEELER);
            inWaterOnCreature = brain.getInputNeuronFromName(NAME_IN_WATERONCREATURE);

            outBirth = brain.getOutputNeuronFromName(NAME_OUT_BIRTH);
            outRotate = brain.getOutputNeuronFromName(NAME_OUT_ROTATE);
            outForward = brain.getOutputNeuronFromName(NAME_OUT_FORWARD);
            outFeelerangle = brain.getOutputNeuronFromName(NAME_OUT_FEELERANGLE);
            outAttack = brain.getOutputNeuronFromName(NAME_OUT_ATTACK);
            outEat = brain.getOutputNeuronFromName(NAME_OUT_EAT);

//            CalculateFeelerPos();
            for (int i = 0; i < 10; i++)
            {
                brain.RandomMutation(0.1f);
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

        public void ReadSensors()
        {
            brain.invalidate();
            inBias.setValue(1);

            inFoodValuePosition.setValue(0);
            inFoodValueFeeler.setValue(0); 
            inOcclusionFeeler.setValue(0); 
            inEnergy.setValue((energy - MINIMUMSURVIVALENERGY) / (STARTENERGY - MINIMUMSURVIVALENERGY));
            inAge.setValue(age);
            inGeneticDifference.setValue(0);
            inWasAttacked.setValue(0); 
            inWaterOnFeeler.setValue(0); 
            inWaterOnCreature.setValue(0); 
        }

        public void Act()
        {
//            Tile t = new Tile(Tile.Type.Grass);
//            Tile t = Tilemap.getTileAtWorldPosition(pos):
//            float costMult = createCostMultiplier(t);
            float costMult = 1;
//            actRotate(costMult);
//            actMove(costMult);
//            actBirth();
//            actFeelerRotate();
//            actEat(costMult,t);

            age += AGEPERTICK;

            if (energy<100)
            {
//                Kill(t);
            }
        }

        private void Kill(Tile t)
        {
//            if (t.isLand())
            {
                
            }
        }


        private SpriteRenderer sr;

        private void Start()
        {
            sr = GetComponent<SpriteRenderer>();
        }
    }
}